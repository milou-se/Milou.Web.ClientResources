using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;

namespace Milou.Web.ClientResources
{
    internal struct CustomStaticFileContext
    {
        private readonly IOwinContext _context;
        private readonly CustomStaticFileOptions _options;
        private readonly PathString _matchUrl;
        private readonly IOwinRequest _request;
        private readonly IOwinResponse _response;
        private string _method;
        private bool _isGet;
        private PathString _subPath;
        private string _contentType;
        private IFileInfo _fileInfo;
        private long _length;
        private DateTime _lastModified;
        private string _lastModifiedString;
        private string _etag;
        private string _etagQuoted;

        private PreconditionState _ifMatchState;
        private PreconditionState _ifNoneMatchState;
        private PreconditionState _ifModifiedSinceState;
        private PreconditionState _ifUnmodifiedSinceState;

        private readonly IList<Tuple<long, long>> _ranges;

        public CustomStaticFileContext(IOwinContext context, CustomStaticFileOptions options, PathString matchUrl)
        {
            _context = context;
            _options = options;
            _matchUrl = matchUrl;
            _request = context.Request;
            _response = context.Response;

            _method = null;
            _isGet = false;
            IsHeadMethod = false;
            _subPath = PathString.Empty;
            _contentType = null;
            _fileInfo = null;
            _length = 0;
            _lastModified = new DateTime();
            _etag = null;
            _etagQuoted = null;
            _lastModifiedString = null;
            _ifMatchState = PreconditionState.Unspecified;
            _ifNoneMatchState = PreconditionState.Unspecified;
            _ifModifiedSinceState = PreconditionState.Unspecified;
            _ifUnmodifiedSinceState = PreconditionState.Unspecified;
            _ranges = null;
        }

        internal enum PreconditionState
        {
            Unspecified,
            NotModified,
            ShouldProcess,
            PreconditionFailed
        }

        public bool IsHeadMethod { get; private set; }

        public bool IsRangeRequest
        {
            get { return _ranges != null; }
        }

        public bool ValidateMethod()
        {
            _method = _request.Method;
            _isGet = Helpers.IsGetMethod(_method);
            IsHeadMethod = Helpers.IsHeadMethod(_method);
            return _isGet || IsHeadMethod;
        }

        // Check if the URL matches any expected paths
        public bool ValidatePath()
        {
            return Helpers.TryMatchPath(_context, _matchUrl, false, out _subPath);
        }

        public bool LookupContentType()
        {
            if (_options.ContentTypeProvider.TryGetContentType(_subPath.Value, out _contentType))
            {
                return true;
            }

            if (_options.ServeUnknownFileTypes)
            {
                _contentType = _options.DefaultContentType;
                return true;
            }

            return false;
        }

        public bool LookupFileInfo()
        {
            bool found = _options.FileSystem.TryGetFileInfo(_subPath.Value, out _fileInfo);
            if (found)
            {
                _length = _fileInfo.Length;

                DateTime last = _fileInfo.LastModified;
                // Truncate to the second.
                _lastModified = new DateTime(last.Year,
                    last.Month,
                    last.Day,
                    last.Hour,
                    last.Minute,
                    last.Second,
                    last.Kind);
                _lastModifiedString = _lastModified.ToString(Constants.HttpDateFormat, CultureInfo.InvariantCulture);

                long etagHash = _lastModified.ToFileTimeUtc() ^ _length;
                _etag = Convert.ToString(etagHash, 16);
                _etagQuoted = '\"' + _etag + '\"';
            }
            return found;
        }

        public void ComprehendRequestHeaders()
        {
            ComputeIfMatch();

            ComputeIfModifiedSince();
        }

        private void ComputeIfMatch()
        {
            // 14.24 If-Match
            IList<string> ifMatch = _request.Headers.GetCommaSeparatedValues(Constants.IfMatch); // Removes quotes
            if (ifMatch != null)
            {
                _ifMatchState = PreconditionState.PreconditionFailed;
                foreach (string segment in ifMatch)
                {
                    if (segment.Equals("*", StringComparison.Ordinal)
                        || segment.Equals(_etag, StringComparison.Ordinal))
                    {
                        _ifMatchState = PreconditionState.ShouldProcess;
                        break;
                    }
                }
            }

            // 14.26 If-None-Match
            IList<string> ifNoneMatch = _request.Headers.GetCommaSeparatedValues(Constants.IfNoneMatch);
            if (ifNoneMatch != null)
            {
                _ifNoneMatchState = PreconditionState.ShouldProcess;
                foreach (string segment in ifNoneMatch)
                {
                    if (segment.Equals("*", StringComparison.Ordinal)
                        || segment.Equals(_etag, StringComparison.Ordinal))
                    {
                        _ifNoneMatchState = PreconditionState.NotModified;
                        break;
                    }
                }
            }
        }

        private void ComputeIfModifiedSince()
        {
            // 14.25 If-Modified-Since
            string ifModifiedSinceString = _request.Headers.Get(Constants.IfModifiedSince);
            DateTime ifModifiedSince;
            if (Helpers.TryParseHttpDate(ifModifiedSinceString, out ifModifiedSince))
            {
                bool modified = ifModifiedSince < _lastModified;
                _ifModifiedSinceState = modified ? PreconditionState.ShouldProcess : PreconditionState.NotModified;
            }

            // 14.28 If-Unmodified-Since
            string ifUnmodifiedSinceString = _request.Headers.Get(Constants.IfUnmodifiedSince);
            DateTime ifUnmodifiedSince;
            if (Helpers.TryParseHttpDate(ifUnmodifiedSinceString, out ifUnmodifiedSince))
            {
                bool unmodified = ifUnmodifiedSince >= _lastModified;
                _ifUnmodifiedSinceState = unmodified
                    ? PreconditionState.ShouldProcess
                    : PreconditionState.PreconditionFailed;
            }
        }

        public void ApplyResponseHeaders(int statusCode)
        {
            _response.StatusCode = statusCode;
            if (statusCode < 400)
            {
                // these headers are returned for 200, 206, and 304
                // they are not returned for 412 and 416
                if (!string.IsNullOrEmpty(_contentType))
                {
                    _response.ContentType = _contentType;
                }
                _response.Headers.Set(Constants.LastModified, _lastModifiedString);
                _response.ETag = _etagQuoted;
            }
            if (statusCode == Constants.Status200Ok)
            {
                // this header is only returned here for 200
                // it already set to the returned range for 206
                // it is not returned for 304, 412, and 416
                _response.ContentLength = _length;
            }
            _options.OnPrepareResponse(new CustomStaticFileResponseContext
            {
                OwinContext = _context,
                File = _fileInfo
            });
        }

        public PreconditionState GetPreconditionState()
        {
            return GetMaxPreconditionState(_ifMatchState,
                _ifNoneMatchState,
                _ifModifiedSinceState,
                _ifUnmodifiedSinceState);
        }

        private static PreconditionState GetMaxPreconditionState(params PreconditionState[] states)
        {
            var max = PreconditionState.Unspecified;
            for (int i = 0; i < states.Length; i++)
            {
                if (states[i] > max)
                {
                    max = states[i];
                }
            }
            return max;
        }

        public Task SendStatusAsync(int statusCode)
        {
            ApplyResponseHeaders(statusCode);

            return Constants.CompletedTask;
        }

        public Task SendAsync()
        {
            ApplyResponseHeaders(Constants.Status200Ok);

            string physicalPath = _fileInfo.PhysicalPath;
            var sendFile =
                _response.Get<Func<string, long, long?, CancellationToken, Task>>(Constants.SendFileAsyncKey);
            if (sendFile != null && !string.IsNullOrEmpty(physicalPath))
            {
                return sendFile(physicalPath, 0, _length, _request.CallCancelled);
            }

            Stream readStream = _fileInfo.CreateReadStream();
            var copyOperation = new StreamCopyOperation(readStream, _response.Body, _length, _request.CallCancelled);
            Task task = copyOperation.Start();
            task.ContinueWith(resultTask => readStream.Close(), TaskContinuationOptions.ExecuteSynchronously);
            return task;
        }

        // Note: This assumes ranges have been normalized to absolute byte offsets.
        private string ComputeContentRange(Tuple<long, long> range, out long start, out long length)
        {
            start = range.Item1;
            long end = range.Item2;
            length = end - start + 1;
            return string.Format(CultureInfo.InvariantCulture, "bytes {0}-{1}/{2}", start, end, _length);
        }
    }
}