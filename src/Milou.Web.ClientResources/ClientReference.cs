using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Milou.Web.ClientResources
{
    public static class ClientReference
    {
        public static string CreateCssLinkHtmlElements(
            string relativeFilePath,
            IDictionary<string, string> htmlAttributes = null,
            string filePartSeparator = "__",
            string absoluteBaseDirectory = null,
            string virtualRootPathForStaticFiles = "vstatic",
            bool normalizeHrefs = true,
            string applicationRootPath = "")
        {
            if (string.IsNullOrWhiteSpace(relativeFilePath))
            {
                throw new ArgumentNullException(nameof(relativeFilePath));
            }

            if (string.IsNullOrWhiteSpace(filePartSeparator))
            {
                throw new ArgumentNullException(nameof(filePartSeparator));
            }

            if (string.IsNullOrWhiteSpace(virtualRootPathForStaticFiles))
            {
                throw new ArgumentNullException(nameof(virtualRootPathForStaticFiles));
            }

            string version = GlobalVersion.Current;

            string usedBaseDirectory = string.IsNullOrWhiteSpace(absoluteBaseDirectory)
                ? AppDomain.CurrentDomain.BaseDirectory
                : absoluteBaseDirectory;

            string absolutePath = Path.Combine(usedBaseDirectory, relativeFilePath);

            Console.WriteLine(absolutePath);

            var fileInfo = new FileInfo(absolutePath);

            DirectoryInfo directory = fileInfo.Directory;

            if (directory == null)
            {
                throw new InvalidOperationException($"The directory for '{relativeFilePath}' is null");
            }

            if (!directory.Exists)
            {
                throw new InvalidOperationException($"The directory '{directory.FullName}' does not exist");
            }

            string relativeDirectory = "";

            if (relativeFilePath.Contains("/"))
            {
                relativeDirectory = relativeFilePath.Substring(
                    0,
                    relativeFilePath.LastIndexOf("/", StringComparison.InvariantCulture));
            }

            string fileName = Path.GetFileName(relativeFilePath);

            List<FileInfo> files = directory.EnumerateFiles(fileName, SearchOption.TopDirectoryOnly).ToList();

            var filesToCreateLinksFor = new List<string>();

            if (files.Any())
            {
                filesToCreateLinksFor.AddRange(
                    files.OrderBy(file => file.Name).Select(file => CreateFullRelativePath(relativeDirectory, file)));
            }

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

            string partsPattern = $"{fileNameWithoutExtension}{filePartSeparator}*";

            List<FileInfo> partFiles = directory.EnumerateFiles(partsPattern, SearchOption.TopDirectoryOnly).ToList();

            filesToCreateLinksFor.AddRange(
                partFiles.OrderBy(file => file.Name).Select(file => CreateFullRelativePath(relativeDirectory, file)));

            string rootPath =
                (string.IsNullOrWhiteSpace(applicationRootPath) ? "/" : $"{applicationRootPath}/").Replace("//", "/");

            List<string> htmlElements =
                filesToCreateLinksFor.Select(
                    file =>
                        CreateHtmlElement(
                            file,
                            virtualRootPathForStaticFiles,
                            rootPath,
                            htmlAttributes,
                            version,
                            normalizeHrefs)).ToList();

            string combined = string.Join(Environment.NewLine, htmlElements);

            return combined;
        }

        public static string CreateHtmlElement(
            string filePath,
            string absoluteOrRelativeBaseUrlPath,
            string rootPath,
            IDictionary<string, string> htmlAttributes,
            string globalVersion = "",
            bool normalizeHrefs = true)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (string.IsNullOrWhiteSpace(absoluteOrRelativeBaseUrlPath))
            {
                throw new ArgumentNullException(nameof(absoluteOrRelativeBaseUrlPath));
            }

            string resultHtmlElement;

            string path = $"{rootPath}{absoluteOrRelativeBaseUrlPath}/{globalVersion}/{filePath}".Replace("//", "/");

            string attributes = string.Join(
                " ",
                htmlAttributes?.Select(CreateHtmlAttribute) ?? Enumerable.Empty<string>());

            string html =
                $"<link type=\"{ContentType.Css.RegisteredContentType}\" href=\"{path}\" rel=\"stylesheet\" {attributes} />";

            if (normalizeHrefs)
            {
                string normalized = html.ToLowerInvariant();

                resultHtmlElement = normalized;
            }
            else
            {
                resultHtmlElement = html;
            }

            return resultHtmlElement;
        }

        public static string CreateScriptTag(
            string relativeFilePath,
            string absoluteBaseDirectory = null,
            string virtualRootPathForStaticFiles = "vstatic",
            bool normalizeHrefs = true,
            string applicationRootPath = "")
        {
            if (string.IsNullOrWhiteSpace(relativeFilePath))
            {
                throw new ArgumentNullException(nameof(relativeFilePath));
            }

            if (string.IsNullOrWhiteSpace(virtualRootPathForStaticFiles))
            {
                throw new ArgumentNullException(nameof(virtualRootPathForStaticFiles));
            }

            string version = GlobalVersion.Current;

            string usedBaseDirectory = string.IsNullOrWhiteSpace(absoluteBaseDirectory)
                ? AppDomain.CurrentDomain.BaseDirectory
                : absoluteBaseDirectory;

            string absolutePath = Path.Combine(usedBaseDirectory, relativeFilePath);

            Console.WriteLine(absolutePath);

            var fileInfo = new FileInfo(absolutePath);

            DirectoryInfo directory = fileInfo.Directory;

            if (directory == null)
            {
                throw new InvalidOperationException($"The directory for '{relativeFilePath}' is null");
            }

            if (!directory.Exists)
            {
                throw new InvalidOperationException($"The directory '{directory.FullName}' does not exist");
            }

            string path = $"{virtualRootPathForStaticFiles}/{version}/{relativeFilePath}".Replace("//", "/");

            string rootPath =
                (string.IsNullOrWhiteSpace(applicationRootPath) ? "/" : $"{applicationRootPath}/").Replace("//", "/");

            string html =
                $"<script type=\"{ContentType.JavaScript.RegisteredContentType}\" src=\"{rootPath}{path}\"></script>";

            string resultHtmlElement;
            if (normalizeHrefs)
            {
                string normalized = html.ToLowerInvariant();

                resultHtmlElement = normalized;
            }
            else
            {
                resultHtmlElement = html;
            }

            return resultHtmlElement;
        }

        public static string CreateModuleScriptTag(
            string relativeFilePath,
            string absoluteBaseDirectory = null,
            string virtualRootPathForStaticFiles = "vstatic",
            bool normalizeHrefs = true,
            string applicationRootPath = "")
        {
            if (string.IsNullOrWhiteSpace(relativeFilePath))
            {
                throw new ArgumentNullException(nameof(relativeFilePath));
            }

            if (string.IsNullOrWhiteSpace(virtualRootPathForStaticFiles))
            {
                throw new ArgumentNullException(nameof(virtualRootPathForStaticFiles));
            }

            string version = GlobalVersion.Current;

            string usedBaseDirectory = string.IsNullOrWhiteSpace(absoluteBaseDirectory)
                ? AppDomain.CurrentDomain.BaseDirectory
                : absoluteBaseDirectory;

            string absolutePath = Path.Combine(usedBaseDirectory, relativeFilePath);

            Console.WriteLine(absolutePath);

            var fileInfo = new FileInfo(absolutePath);

            DirectoryInfo directory = fileInfo.Directory;

            if (directory == null)
            {
                throw new InvalidOperationException($"The directory for '{relativeFilePath}' is null");
            }

            if (!directory.Exists)
            {
                throw new InvalidOperationException($"The directory '{directory.FullName}' does not exist");
            }

            string path = $"{virtualRootPathForStaticFiles}/{version}/{relativeFilePath}".Replace("//", "/");

            string rootPath =
                (string.IsNullOrWhiteSpace(applicationRootPath) ? "/" : $"{applicationRootPath}/").Replace("//", "/");

            string html =
                $"<script type=\"{ContentType.JavaScriptModule.RegisteredContentType}\" src=\"{rootPath}{path}\"></script>";

            string resultHtmlElement;
            if (normalizeHrefs)
            {
                string normalized = html.ToLowerInvariant();

                resultHtmlElement = normalized;
            }
            else
            {
                resultHtmlElement = html;
            }

            return resultHtmlElement;
        }

        private static string CreateFullRelativePath(string relativeDirectory, FileInfo file)
        {
            return $"{relativeDirectory}/{file.Name}";
        }

        private static string CreateHtmlAttribute(KeyValuePair<string, string> attribute)
        {
            return $"{attribute.Key}=\"{attribute.Value}\"";
        }
    }
}