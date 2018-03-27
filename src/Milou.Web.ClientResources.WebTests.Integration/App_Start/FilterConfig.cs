using System.Web.Mvc;

namespace Milou.Web.ClientResources.WebTests.Integration
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}