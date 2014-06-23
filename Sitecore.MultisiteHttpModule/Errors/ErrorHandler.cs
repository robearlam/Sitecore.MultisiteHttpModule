using Sitecore.Diagnostics;
using Sitecore.MultisiteHttpModule.Configuration;
using System;
using System.Configuration;
using System.Web;

namespace Sitecore.MultisiteHttpModule.Errors
{
    public class ErrorHandler
    {
        private MultisiteHttpModuleSettings _settings;

        public static void HandleError(Exception ex)
        {
            var errorHandler = new ErrorHandler();
            errorHandler.ProcessError(ex);
        }

        public void ProcessError(Exception ex)
        {
            EnsureSettingsArePopulated();
            if (!_settings.ErrorsEnabled)
            {
                return;
            }

            LogError(ex);
            RedirectUserToErrorPage();
        }

        private void EnsureSettingsArePopulated()
        {
            if (_settings == null)
            {
                _settings = ConfigurationManager.GetSection("multisiteHttpModule") as MultisiteHttpModuleSettings;
            }
        }

        private void LogError(Exception ex)
        {
            Log.Error(String.Format("Sitecore.MultisiteHttpModule.Errors: Unhandled exception caught {0} {1}", ex.Message, ex.StackTrace), this);
        }

        private void RedirectUserToErrorPage()
        {
            var errorPagePath = Context.Site.Properties[Settings.Constants.PropertyNames.ErrorPagePath];
            var targetErrorPage = !String.IsNullOrEmpty(errorPagePath) ? errorPagePath : _settings.DefaultErrorPage;

            HttpContext.Current.Server.ClearError();
            HttpContext.Current.Response.Redirect(String.Format("http://{0}{1}?aspxerrorpath={2}",
                Context.Site.TargetHostName,
                targetErrorPage,
                HttpContext.Current.Server.UrlEncode(HttpContext.Current.Request.Url.LocalPath)), true);
        }

    }
}
