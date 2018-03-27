using System;
using System.IO;
using Machine.Specifications;

namespace Milou.Web.ClientResources.Tests.Integration
{
    [Subject(typeof(DirectoryHashGlobalVersionCreator))]
    public class when_creating_global_id
    {
        private static DirectoryHashGlobalVersionCreator global_version_creator;

        private static string global_id;

        private Establish context = () =>
        {
            string sourcePath = Path.Combine(VcsTestPathHelper.FindVcsRootPath(), "src");

            global_version_creator = new DirectoryHashGlobalVersionCreator(sourcePath);
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