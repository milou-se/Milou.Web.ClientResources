using System;
using System.IO;
using System.Threading;
using Machine.Specifications;

namespace Milou.Web.ClientResources.Tests.Integration
{
    [Subject(typeof(GlobalVersion))]
    public class when_changing_a_content_directory
    {
        private static DirectoryHashGlobalVersionCreator global_version_creator;

        private static DirectoryInfo temp_directory;

        private static string old_global_id;

        private static string new_global_version;

        private Cleanup after = () =>
        {
            if (temp_directory != null && temp_directory.Exists)
            {
                temp_directory.Delete(true);
            }
        };

        private Establish context = () =>
        {
            temp_directory =
                new DirectoryInfo(
                    Path.Combine(
                        Path.GetTempPath(),
                        $"MilouWebClientR_{DateTime.UtcNow.ToString("O").Replace(":", ".")}"));

            temp_directory.Create();

            global_version_creator = new DirectoryHashGlobalVersionCreator(temp_directory.FullName);

            GlobalVersion.Initialize(global_version_creator, UpdateMode.Allow);

            new StaticFileWatcher(Console.WriteLine).Watch(temp_directory);

            old_global_id = GlobalVersion.Current;
        };

        private It new_version_should_not_be_empty = () => new_global_version.ShouldNotBeEmpty();

        private It new_version_should_not_be_null = () => new_global_version.ShouldNotBeNull();

        private Because of = () =>
        {
            temp_directory.CreateSubdirectory("abc");

            Thread.Sleep(TimeSpan.FromMilliseconds(20));

            new_global_version = GlobalVersion.Current;
        };

        private It should_not_be_empty = () => old_global_id.ShouldNotBeEmpty();

        private It should_not_be_null = () => old_global_id.ShouldNotBeNull();

        private It should_not_be_same = () =>
        {
            Console.WriteLine(old_global_id);
            Console.WriteLine(new_global_version);

            new_global_version.ShouldNotEqual(old_global_id);
        };
    }
}