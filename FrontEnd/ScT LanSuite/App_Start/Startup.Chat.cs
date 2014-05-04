using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScT_LanSuite
{
	public partial class Startup
	{
        public void ConfigureChat(IAppBuilder app)
        {
            app.MapSignalR();
        }
	}
}