using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.StaticFiles.Infrastructure;

namespace Milou.Web.ClientResources
{
    public class CustomFileServerOptions : SharedOptionsBase<CustomFileServerOptions>
    {
        /// <summary>
        /// Creates a combined options class for all of the static file middleware components.
        /// </summary>
        public CustomFileServerOptions()
            : base(new SharedOptions())
        {
            StaticFileOptions = new CustomStaticFileOptions(SharedOptions);
            DirectoryBrowserOptions = new DirectoryBrowserOptions(SharedOptions);
            DefaultFilesOptions = new DefaultFilesOptions(SharedOptions);
            EnableDefaultFiles = true;
        }

        /// <summary>
        /// Options for configuring the StaticFileMiddleware.
        /// </summary>
        public CustomStaticFileOptions StaticFileOptions { get; }

        /// <summary>
        /// Options for configuring the DirectoryBrowserMiddleware.
        /// </summary>
        public DirectoryBrowserOptions DirectoryBrowserOptions { get; }

        /// <summary>
        /// Options for configuring the DefaultFilesMiddleware.
        /// </summary>
        public DefaultFilesOptions DefaultFilesOptions { get; }

        /// <summary>
        /// Directory browsing is disabled by default.
        /// </summary>
        public bool EnableDirectoryBrowsing { get; set; }

        /// <summary>
        /// Default files are enabled by default.
        /// </summary>
        public bool EnableDefaultFiles { get; set; }
    }
}