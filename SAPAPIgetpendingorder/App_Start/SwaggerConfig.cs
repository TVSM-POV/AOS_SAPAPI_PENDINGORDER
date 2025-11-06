using System.Web.Http;
using WebActivatorEx;
using SAPAPIgetpendingorder; // your namespace
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace SAPAPIgetpendingorder
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "SAPAPIgetpendingorder");
                })
                .EnableSwaggerUi(c =>
                {
                    c.DisableValidator();
                });
        }
    }
}
