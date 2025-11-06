using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace SAPAPIgetpendingorder.Filters
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            try
            {
                if (actionContext.Request.Headers.Authorization == null)
                {
                    SetUnauthorizedResponse(actionContext, "Missing Authorization header");
                    return;
                }

                if (actionContext.Request.Headers.Authorization.Scheme != "Basic")
                {
                    SetUnauthorizedResponse(actionContext, "Authorization scheme must be Basic");
                    return;
                }

                var authToken = actionContext.Request.Headers.Authorization.Parameter;
                var decodedAuthToken = Encoding.UTF8.GetString(Convert.FromBase64String(authToken));
                var credentials = decodedAuthToken.Split(':');

                if (credentials.Length != 2)
                {
                    SetUnauthorizedResponse(actionContext, "Invalid credentials format");
                    return;
                }

                var username = credentials[0];
                var password = credentials[1];

                if (!IsValidUser(username, password))
                {
                    SetUnauthorizedResponse(actionContext, "Invalid username or password");
                    return;
                }
            }
            catch (Exception ex)
            {
                SetUnauthorizedResponse(actionContext, $"Authentication error: {ex.Message}");
            }
        }

        private void SetUnauthorizedResponse(HttpActionContext actionContext, string message)
        {
            actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.Unauthorized);
            actionContext.Response.Headers.Add("WWW-Authenticate", "Basic");
            actionContext.Response.Content = new StringContent(message);
        }

        private bool IsValidUser(string username, string password)
        {
            var storedUsername = ConfigurationManager.AppSettings["BasicAuth:Username"];
            var storedPassword = ConfigurationManager.AppSettings["BasicAuth:Password"];

            return username == storedUsername && password == storedPassword;
        }
    }
}
