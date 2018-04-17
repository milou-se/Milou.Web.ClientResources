using System;
using System.IO;
using Arbor.Aesculus.Core;

namespace Milou.Web.ClientResources.Tests.Integration
{
    internal class VcsTestPathHelper
    {
        public static string FindVcsRootPath()
        {
            try
            {
                string originalSolutionPath = NCrunch.Framework.NCrunchEnvironment.GetOriginalSolutionPath();

                if (!string.IsNullOrWhiteSpace(originalSolutionPath))
                {
                    DirectoryInfo parent = new DirectoryInfo(originalSolutionPath).Parent;

                    if (parent != null)
                    {
                        return VcsPathHelper.FindVcsRootPath(parent.FullName);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.IsFatal())
                {
                    throw;
                }
            }

            return VcsPathHelper.FindVcsRootPath();
        }
    }

    //NOTE: Licensed under the new BSD license, http://orchard.codeplex.com/license
    //Originally taken from http://orchard.codeplex.com/SourceControl/latest#src/Orchard/Exceptions/ExceptionExtensions.cs
}