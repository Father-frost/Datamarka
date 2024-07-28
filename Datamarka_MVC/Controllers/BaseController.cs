using Datamarka_MVC.ConfigurationSections;
using Datamarka_MVC.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Datamarka_MVC.Controllers
{

    [CurrentUserFromCookieFilter]
    public class BaseController : Controller
    {
        protected readonly IConfiguration _config;
        protected readonly string BaseUrl;

        public BaseController(IConfiguration config)
        {
            _config = config;

#pragma warning disable CS8601 // Possible null reference assignment - can not happen, ensured on Startup!
            BaseUrl = _config.GetValue(
                ConfigurationFieldNames.BaseUrl,
                string.Empty);
#pragma warning restore CS8601 // Possible null reference assignment.
        }

        protected HttpClient CreateApiClient()
        {
            var client = new HttpClient();

            if (Request.Headers.TryGetValue("Cookie", out var cookieValues))
            {
                string cookies = string.Join("; ", cookieValues);
                client.DefaultRequestHeaders.Add("Cookie", cookies);
            }

            return client;
        }

        protected async Task<T?> ParseJsonBody<T>(HttpResponseMessage response)
        {
            // Read the response content as a JSON string
            string json = await response.Content.ReadAsStringAsync();

            // Deserialize the JSON into an array of User objects
            return JsonSerializer.Deserialize<T>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );
        }

        protected string? GetCurrentUserDeclaredByBrowser()
        {
            if (HttpContext.Items.TryGetValue(CurrentUserFromCookieFilterAttribute.UserNameContextItemKey, out var currentUser))
            {
                return currentUser.ToString();
            }
            return null;
        }
    }
}
