using System;
using System.IO;
using Machine.Specifications;

namespace Milou.Web.ClientResources.Tests.Integration
{
    [Subject(typeof(DirectoryHashGlobalVersionCreator))]
    public class when_creating_global_id_for_a_directory_with_an_emtpy_file
    {
        private static DirectoryHashGlobalVersionCreator global_version_creator;

        private static string global_id;

        private static string absolute_base_directory_path;

        private static string test_file_path;

        private Cleanup after = () =>
        {
            if (File.Exists(test_file_path))
            {
                File.Delete(test_file_path);
            }

            if (Directory.Exists(absolute_base_directory_path))
            {
                Directory.Delete(absolute_base_directory_path);
            }
        };

        private Establish context = () =>
        {
            absolute_base_directory_path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            test_file_path = Path.Combine(absolute_base_directory_path, string.Format("{0}.tmp", Guid.NewGuid()));

            Directory.CreateDirectory(absolute_base_directory_path);

            using (File.Create(test_file_path))
            {
            }

            global_version_creator = new DirectoryHashGlobalVersionCreator(absolute_base_directory_path);
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