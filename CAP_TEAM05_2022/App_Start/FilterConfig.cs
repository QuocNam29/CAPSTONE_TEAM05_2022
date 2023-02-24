using System;
using System.Web;
using System.Web.Mvc;

namespace CAP_TEAM05_2022
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // filters.Add(new HandleErrorAttribute());
            filters.Add(new HandleErrorAttribute()
            {
                ExceptionType = typeof(Exception),
                View = "Error"
            });
        }
    }
}
