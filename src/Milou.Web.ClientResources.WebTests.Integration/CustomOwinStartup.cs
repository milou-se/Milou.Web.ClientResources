using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using Microsoft.Owin;
using Owin;

namespace Milou.Web.ClientResources.WebTests.Integration
{
    public static class CustomOwinStartup
    {
        public static void Configuration(IAppBuilder app)
        {
            const string Key = "urn:milou:web:global-version-id:content-directory";
            const string VirtualBaseDirectoryKey =
                "urn:milou:web:global-version-id:virtual-content-directory";

            string relativePhysicalDirectoryPath = ConfigurationManager.AppSettings[Key];

            if (string.IsNullOrWhiteSpace(relativePhysicalDirectoryPath))
            {
                throw new ConfigurationErrorsException(
                    $"The configuration property {Key} is missing ");
            }

            string virtualBaseDir = ConfigurationManager.AppSettings[VirtualBaseDirectoryKey];

            if (string.IsNullOrWhiteSpace(virtualBaseDir))
            {
                throw new ConfigurationErrorsException(
                    $"The configuration property {VirtualBaseDirectoryKey} is missing ");
            }

            var contentTypes = new Dictionary<string, string>
            {
                { ".json", "application/json" },
                { ".txt", "text/replaced" },
                { ".html", "text/html" },
                { ".abc", "text/custom" }
            };

            var provider = new ExtendableContentTypeProvider(contentTypes);

            StaticFileConfigurationResult useStaticFilesWithCaching = app.UseStaticFilesWithCaching(
                new StaticFilesOptions(virtualBaseDir, relativePhysicalDirectoryPath, provider),
                UpdateMode.Allow);

            if (useStaticFilesWithCaching.FileServerOptions != null)
            {
                string physicalDirectoryPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    relativePhysicalDirectoryPath);

                new StaticFileWatcher(
                    message => Debug.WriteLine(message),
                    onChanged: (name =>
                    {
                        useStaticFilesWithCaching.FileServerOptions.RequestPath =
                            new PathString($"/{GlobalVersion.Current}");
                    })).Watch(new DirectoryInfo(physicalDirectoryPath));
            }
        }
    }
}