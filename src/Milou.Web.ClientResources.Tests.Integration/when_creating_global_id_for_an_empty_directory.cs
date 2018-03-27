using System;
using System.IO;
using Machine.Specifications;

namespace Milou.Web.ClientResources.Tests.Integration
{
    [Subject(typeof(DirectoryHashGlobalVersionCreator))]
    public class when_creating_global_id_for_an_empty_directory
    {
        private static DirectoryHashGlobalVersionCreator global_version_creator;

        private static string global_id;

        private static string _absoluteBaseDirectoryPath;

        private Cleanup after = () =>
        {
            if (Directory.Exists(_absoluteBaseDirectoryPath))
            {
                Directory.Delete(_absoluteBaseDirectoryPath);
            }
        };

        private Establish context = () =>
        {
            _absoluteBaseDirectoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            Directory.CreateDirectory(_absoluteBaseDirectoryPath);

            global_version_creator = new DirectoryHashGlobalVersionCreator(_absoluteBaseDirectoryPath);
        };

        private Because of = () =>
        {
            global_id = global_version_creator.CreateGlobalId();

            Console.WriteLine(global_id);
        };

        private It should_not_be_empty = () => { global_id.ShouldNotBeNull(); };

        private It should_not_be_null = () => { global_id.ShouldNotBeNull(); };
    }
}