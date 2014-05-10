using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ScT_LanSuite
{
    public partial class Startup
    {
        public void ConfigureRequester(IAppBuilder app)
        {
            var _tokenSource = new CancellationTokenSource();
            var _token = _tokenSource.Token;
            Task t = Task.Run(() =>
            {
                while (!_token.IsCancellationRequested)
                {
                    try
                    {
                        new HttpClient().GetAsync(GetBaseUrl()).Wait(20000, _token);
                    }
                    catch (Exception)
                    {
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(300));
                }
            }, _token);
        }

        private string GetBaseUrl()
        {
            var request = HttpContext.Current.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (!string.IsNullOrWhiteSpace(appUrl)) appUrl += "/";

            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

            return baseUrl;
        }
    }
}