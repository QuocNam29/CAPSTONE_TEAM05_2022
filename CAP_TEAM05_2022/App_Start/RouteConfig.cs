using System.Web.Mvc;
using System.Web.Routing;

namespace CAP_TEAM05_2022
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "categories",
                "quan-ly-danh-muc",
                new { controller = "categories", action = "Index" }
            );

            routes.MapRoute(
               "AspNetUsers",
               "quan-ly-nhan-vien",
               new { controller = "AspNetUsers", action = "Index" }
           );

            routes.MapRoute(
                "Customers",
                "quan-ly-khach-hang",
                new { controller = "customers", action = "Customers" }
            );

            routes.MapRoute(
                "Suppliers",
                "quan-ly-cong-ty-cung-cap",
                new { controller = "customers", action = "Suppliers" }
            );

            routes.MapRoute(
                "products",
                "quan-ly-san-pham",
                new { controller = "products", action = "Index" }
            );

            routes.MapRoute(
                "sales",
                "quan-ly-don-hang",
                new { controller = "sales", action = "Index" }
            );

            routes.MapRoute(
                "import_inventory",
                "ton-kho",
                new { controller = "import_inventory", action = "Index" }
            );

            routes.MapRoute(
                "inventory_order",
                "quan-ly-nhap-hang",
                new { controller = "inventory_order", action = "Index" }
            );

            routes.MapRoute(
               "inventory_orderCreate",
               "tao-don-nhap-hang",
               new { controller = "inventory_order", action = "Create" }
           );

            routes.MapRoute(
                "Revenue",
                "doanh-thu-chi-tiet",
                new { controller = "sales", action = "Revenue" }
            );

            routes.MapRoute(
                "Revenues",
                "bao-cao-doanh-thu-theo-ngay-thang",
                new { controller = "Revenues", action = "Index" }
            );

            routes.MapRoute(
                "SumRevenue",
                "bao-cao-doanh-thu-tong-hop",
                new { controller = "Revenues", action = "SumRevenue" }
            );

            routes.MapRoute(
                "return_sale",
                "lich-su-doi-tra-don-hang",
                new { controller = "return_sale", action = "Index" }
            );

            routes.MapRoute(
                "return_supplier",
                "lich-su-doi-tra-voi-cong-ty",
                new { controller = "return_supplier", action = "Index" }
            );

            routes.MapRoute(
                "OldDebtsCustomer",
                "cong-no-cu-voi-khach-hang",
                new { controller = "Debts_Collections", action = "OldDebtsCustomer" }
            );

            routes.MapRoute(
                "CreateOldDebtCustomer",
                "tao-don-no-cu-voi-khach-hang",
                new { controller = "Debts_Collections", action = "CreateOldDebtCustomer" }
            );

            routes.MapRoute(
                "OldDebtsSupplier",
                "cong-no-cu-voi-cong-ty",
                new { controller = "Debts_Collections", action = "OldDebtsSupplier" }
            );

            routes.MapRoute(
                "Debts_Collections",
                "cong-no-voi-khach-hang",
                new { controller = "Debts_Collections", action = "Index" }
            );

            routes.MapRoute(
                "DebtsSupplier",
                "cong-no-voi-cong-ty",
                new { controller = "Debts_Collections", action = "DebtsSupplier" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "sale_details", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
