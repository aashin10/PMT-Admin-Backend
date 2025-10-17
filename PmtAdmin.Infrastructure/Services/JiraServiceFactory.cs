using RestSharp;
using RestSharp.Authenticators.OAuth2;

namespace PmtAdmin.Infrastructure.Services
{

    public static class JiraServiceFactory
    {
        public static RestClient CreateClient(string baseUrl, string token)
        {
            var authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(token, "Bearer");

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                var options = new RestClientOptions
                {
                    Authenticator = authenticator
                };
                return new RestClient(options);
            }

            var optionsWithUrl = new RestClientOptions(baseUrl)
            {
                Authenticator = authenticator
            };

            return new RestClient(optionsWithUrl);
        }
    }
}
