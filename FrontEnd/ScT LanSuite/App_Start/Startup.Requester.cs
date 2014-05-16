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
            var url = "http://sct-lansuite.apphb.com";
            Task t = Task.Run(() =>
            {
                while (!_token.IsCancellationRequested)
                {
                    try
                    {
                        new HttpClient().GetAsync(url).Wait(20000, _token);
                    }
                    catch (Exception)
                    {
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(300));
                }
            }, _token);
            
        }
    }
}