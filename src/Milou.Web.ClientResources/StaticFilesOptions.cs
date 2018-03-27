using Microsoft.Owin.StaticFiles.ContentTypes;

namespace Milou.Web.ClientResources
{
    public class StaticFilesOptions
    {
        public StaticFilesOptions(
            string virtualBaseDir,
            string relativePhysicalContentDirectory,
            IContentTypeProvider contentTypeProvider = null)
        {
            VirtualBaseDir = virtualBaseDir;
            RelativePhysicalContentDirectory = relativePhysicalContentDirectory;
            ContentTypeProvider = contentTypeProvider;
        }

        public string VirtualBaseDir { get; }

        public string RelativePhysicalContentDirectory { get; }

        public IContentTypeProvider ContentTypeProvider { get; }
    }
}