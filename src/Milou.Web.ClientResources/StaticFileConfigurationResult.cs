namespace Milou.Web.ClientResources
{
    public class StaticFileConfigurationResult
    {
        public StaticFileConfigurationResult(CustomFileServerOptions fileServerOptions)
        {
            FileServerOptions = fileServerOptions;
        }

        public CustomFileServerOptions FileServerOptions { get; }
    }
}