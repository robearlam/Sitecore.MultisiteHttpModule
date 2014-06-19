using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.SecurityModel;
using System;
using System.Web;

namespace Sitecore.MultisiteHttpModule.NotFound
{
    public class NotFoundHandler : HttpRequestProcessor
    {
        private readonly NotFoundManager _notFoundManager = new NotFoundManager();

        public override void Process(HttpRequestArgs args)
        {
            var context = HttpContext.Current;
            try
            {
                using (new SecurityDisabler())
                {
                    _notFoundManager.ProcessRequest(args, context);
                }
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Sitecore.MultiSite404Handler: Unable to process 404 for url [{0}] due to {1} {2}", context.Request.RawUrl, ex.Message, ex.StackTrace), this);
            }
        }
    }
}