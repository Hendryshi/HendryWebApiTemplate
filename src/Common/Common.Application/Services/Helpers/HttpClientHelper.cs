using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Common.Application.Services.Helpers
{
    public static class HttpClientHelper
    {
        private static IHostEnvironment _env;

        public static void SetHostEnvironment(IHostEnvironment env) => _env = env;

        public static HttpClient CreateClient()
        {
            var httpClientHandler = new HttpClientHandler();
            if (_env.IsDevelopment())
            {
                // when deployed under paris-dev2. the following error has been produced
                // "The remote certificate is invalid because of errors in the certificate chain"
                // to overcome this issue the following code snippet is added
                // TODO: this is a temporary fix need to find an optiomal solution.
                httpClientHandler.ServerCertificateCustomValidationCallback =
                    (message, cert, chain, sslPolicyErrors) =>
                    {
                        return true;
                    };
            }
            var client = new HttpClient(httpClientHandler);
            return client;
        }

        /// <summary>
        /// Copy headers from accessor context into request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        public static void CopyHeaders(HttpRequestMessage request, IHttpContextAccessor accessor)
        {
            foreach (var header in accessor.HttpContext.Request.Headers)
            {
                try
                {
                    // ignore the "Host" header, as it is set by the HttpClient
                    if (!HeadersToSkip.Contains(header.Key))
                        request.Headers.Add(header.Key, header.Value.ToArray());
                }
                catch { }
            }
        }

        private static readonly HashSet<string> HeadersToSkip = new(StringComparer.OrdinalIgnoreCase)
        {
            "Host",
            "Connection",
            "Content-Length",
            "Transfer-Encoding",
            "Upgrade",
            "TE",
            "Proxy-Authorization",
            "Cookie",
            "Set-Cookie"
        };
    }
}
