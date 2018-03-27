using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using JetBrains.Annotations;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.FileSystems;
using Owin;

namespace Milou.Web.ClientResources
{
    public static class StaticFileExtensions
    {
        private const string LastModified = "Last-Modified";

        private const string CacheControl = "Cache-Control";

        private const string XCache = "X-Cache";

        private const string IfNoneMatch = "If-None-Match";

        private static bool _isInitialized;

        private static readonly object _MutexLock = new object();

        public static StaticFileConfigurationResult UseStaticFilesWithCaching(
            [NotNull] this IAppBuilder app,
            StaticFilesOptions staticFilesOptions,
            UpdateMode updateMode = UpdateMode.Deny)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (string.IsNullOrWhiteSpace(staticFilesOptions.RelativePhysicalContentDirectory))
            {
                throw new ConfigurationErrorsException("The relative physical content directory is missing");
            }

            if (string.IsNullOrWhiteSpace(staticFilesOptions.VirtualBaseDir))
            {
                throw new ConfigurationErrorsException("The virtual content base directory is missning");
            }

            CustomFileServerOptions fileServerOptions = null;

            if (!_isInitialized)
            {
                lock (_MutexLock)
                {
                    if (!_isInitialized)
                    {
                        string physicalPath = Path.Combine(
                            AppDomain.CurrentDomain.BaseDirectory,
                            staticFilesOptions.RelativePhysicalContentDirectory);

                        if (!Directory.Exists(physicalPath))
                        {
                            throw new ConfigurationErrorsException(
                                $"The directory '{staticFilesOptions.RelativePhysicalContentDirectory}' does not exist");
                        }

                        GlobalVersion.Initialize(new DirectoryHashGlobalVersionCreator(physicalPath), updateMode);

                        string contentVirtualPath = $"/{GlobalVersion.Current}";

                        string contentDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);

                        var physicalFileSystem = new PhysicalFileSystem(contentDirectoryPath);

                        fileServerOptions = new CustomFileServerOptions
                        {
                            FileSystem = physicalFileSystem,
                            RequestPath = new PathString(contentVirtualPath),
                            StaticFileOptions =
                            {
                                FileSystem = physicalFileSystem,
                                OnPrepareResponse =
                                    context =>
                                        PrepareResponse(context),
                                ServeUnknownFileTypes = false
                            }
                        };

                        if (staticFilesOptions.ContentTypeProvider != null)
                        {
                            fileServerOptions.StaticFileOptions.ContentTypeProvider =
                                staticFilesOptions.ContentTypeProvider;
                        }

                        app.Map(
                            new PathString($"/{staticFilesOptions.VirtualBaseDir}"),
                            pathConfiguration =>
                            {
                                pathConfiguration.Use(
                                    async (context, next) =>
                                    {
                                        bool modified = true;

                                        string[] values;
                                        if (context.Request.Headers.TryGetValue(IfNoneMatch, out values)
                                            && values.Length == 1)
                                        {
                                            if (values.First().Equals(GlobalVersion.Current))
                                            {
                                                context.Response.StatusCode = (int)HttpStatusCode.NotModified;

                                                modified = false;
                                            }
                                        }

                                        if (modified)
                                        {
                                            await next.Invoke();
                                        }
                                    });

                                pathConfiguration.UseFileServer(fileServerOptions);

#if DEBUG
                                pathConfiguration.Use(async (context, next) => { await next.Invoke(); });
#endif
                            });

                        app.UseStageMarker(PipelineStage.MapHandler);

                        _isInitialized = true;
                    }
                }
            }

            return new StaticFileConfigurationResult(fileServerOptions);
        }

        private static void PrepareResponse(CustomStaticFileResponseContext context)
        {
            if (context.OwinContext.Response.StatusCode != (int)HttpStatusCode.OK)
            {
                return;
            }

            if (string.IsNullOrEmpty(context.File.PhysicalPath))
            {
                context.OwinContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            bool exists = File.Exists(context.File.PhysicalPath);

            if (!exists)
            {
                context.OwinContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            string expiresUtc = DateTime.Now.AddDays(365).ToUniversalTime().ToString("R");

            string etag = HttpResponseHeader.ETag.ToString();

            context.OwinContext.Response.Headers.Append(HttpResponseHeader.Expires.ToString(), expiresUtc);

            if (context.OwinContext.Response.Headers.ContainsKey(etag))
            {
                context.OwinContext.Response.Headers.Remove(etag);
            }

            if (context.OwinContext.Response.Headers.ContainsKey(LastModified))
            {
                context.OwinContext.Response.Headers.Remove(LastModified);
            }

            context.OwinContext.Response.Headers.Append(etag, GlobalVersion.Current);
            context.OwinContext.Response.Headers.Append(CacheControl, "no-transform,public");
            context.OwinContext.Response.Headers.Append(XCache, "HIT");
        }
    }
}