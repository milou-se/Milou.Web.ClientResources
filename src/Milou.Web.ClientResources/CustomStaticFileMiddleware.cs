using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;

namespace Milou.Web.ClientResources
{
    /// <summary>
    /// Enables serving static files for a given request path
    /// </summary>
    public class CustomStaticFileMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> _next;
        private readonly CustomStaticFileOptions _options;

        /// <summary>
        /// Creates a new instance of the StaticFileMiddleware.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="options">The configuration options.</param>
        public CustomStaticFileMiddleware(Func<IDictionary<string, object>, Task> next, CustomStaticFileOptions options)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (options.ContentTypeProvider == null)
            {
                throw new ArgumentNullException(nameof(options.ContentTypeProvider));
            }
            if (options.FileSystem == null)
            {
                options.FileSystem = new PhysicalFileSystem("." + options.RequestPath.Value);
            }

            _next = next;
            _options = options;
        }

        /// <summary>
        /// Processes a request to determine if it matches a known file, and if so, serves it.
        /// </summary>
        /// <param name="environment">OWIN environment dictionary which stores state information about the request, response and relevant server state.</param>
        /// <returns></returns>
        public Task Invoke(IDictionary<string, object> environment)
        {
            IOwinContext context = new OwinContext(environment);

            var fileContext = new CustomStaticFileContext(context, _options, _options.RequestPath);
            if (fileContext.ValidateMethod()
                && fileContext.ValidatePath()
                && fileContext.LookupContentType()
                && fileContext.LookupFileInfo())
            {
                fileContext.ComprehendRequestHeaders();

                switch (fileContext.GetPreconditionState())
                {
                    case CustomStaticFileContext.PreconditionState.Unspecified:
                    case CustomStaticFileContext.PreconditionState.ShouldProcess:
                        if (fileContext.IsHeadMethod)
                        {
                            return fileContext.SendStatusAsync((int)HttpStatusCode.OK);
                        }
                        return fileContext.SendAsync();

                    case CustomStaticFileContext.PreconditionState.NotModified:
                        return fileContext.SendStatusAsync((int)HttpStatusCode.NotModified);

                    case CustomStaticFileContext.PreconditionState.PreconditionFailed:
                        return fileContext.SendStatusAsync((int)HttpStatusCode.PreconditionFailed);

                    default:
                        throw new NotImplementedException(fileContext.GetPreconditionState().ToString());
                }
            }

            return _next(environment);
        }
    }
}