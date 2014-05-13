using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScT_LanSuite
{
    public class RestrictToAjaxAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAjaxRequest())
            {
                HttpSessionStateBase session = filterContext.HttpContext.Session;
                Controller controller = filterContext.Controller as Controller;

                if (controller != null)
                {
                        controller.HttpContext.Response.Redirect("./index");
                }

                base.OnActionExecuting(filterContext);
                //throw new InvalidOperationException("This action is only available via ajax");
            }
        }
    }
}