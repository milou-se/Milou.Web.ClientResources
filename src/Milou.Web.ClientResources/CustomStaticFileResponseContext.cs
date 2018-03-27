using Microsoft.Owin;
using Microsoft.Owin.FileSystems;

namespace Milou.Web.ClientResources
{
    /// <summary>
    /// Contains information about the request and the file that will be served in response.
    /// </summary>
    public class CustomStaticFileResponseContext
    {
        /// <summary>
        /// The request and response information.
        /// </summary>
        public IOwinContext OwinContext { get; internal set; }

        /// <summary>
        /// The file to be served.
        /// </summary>
        public IFileInfo File { get; internal set; }
    }
}