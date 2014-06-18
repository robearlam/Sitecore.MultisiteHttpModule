using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.MultisiteHttpModule.Configuration;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.SecurityModel;
using System;
using System.Configuration;
using System.Web;

namespace Sitecore.MultisiteHttpModule.NotFound
{
    public class MultiSite404Handler : HttpRequestProcessor
    {
        private readonly MultisiteHttpModuleSettings _settings;

        public MultiSite404Handler()
        {
            _settings = ConfigurationManager.GetSection("multisiteHttpModule") as MultisiteHttpModuleSettings;
        }

        public override void Process(HttpRequestArgs args)
        {
            var context = HttpContext.Current;
            try
            {
                using (new SecurityDisabler())
                {
                    ProcessRequest(args, context);
                }
            }
            catch(Exception ex)
            {
                Log.Error(String.Format("MultiSite404Handler: Unable to process 404 for url [{0}] due to {1} {2}", context.Request.RawUrl, ex.Message, ex.StackTrace), this);
            }
        }

        private void ProcessRequest(HttpRequestArgs args, HttpContext context)
        {
            if (IsNot404Request(context, args))
            {
                return;
            }

            if (_settings.Sites[Context.Site.Name] != null)
            {
                RedirectToSiteSpecific404Page(args, context);
            }
        }

        private void RedirectToSiteSpecific404Page(HttpRequestArgs args, HttpContext context)
        {
            if (String.IsNullOrEmpty(_settings.Sites[Context.Site.Name].NotFoundPageId))
            {
                return;
            }

            var siteSpecificNotFoundPage = args.GetItem(new ID(_settings.Sites[Context.Site.Name].NotFoundPageId));
            if (siteSpecificNotFoundPage == null)
            {
                return;
            }

            context.Response.Redirect(String.Format("{0}?url={1}", LinkManager.GetItemUrl(siteSpecificNotFoundPage), context.Request.RawUrl), true);
        }

        private bool IsNot404Request(HttpContext context, HttpRequestArgs args)
        {
            return Context.Database.GetItem(args.Url.ItemPath) != null
                || Context.Site == null
                || Context.Site.Name == "shell"
                || Context.Database == null
                || IsValidAlias(args)
                || ShouldUrlBeExcluded(context.Request.RawUrl)
                || System.IO.File.Exists(context.Server.MapPath(CleanUrl(context.Request.RawUrl)));
        }

        private bool IsValidAlias(HttpRequestArgs args)
        {
            var aliasExist = Context.Database.Aliases.Exists(args.LocalPath);
            return aliasExist && HasValidTargetItem(args);
        }

        private bool HasValidTargetItem(HttpRequestArgs args)
        {
            var isValid = false;
            var targetId = Context.Database.Aliases.GetTargetID(args.LocalPath);
            var targetUrl = Context.Database.Aliases.GetTargetUrl(args.LocalPath);

            if (!targetId.IsNull)
            {
                isValid = TryGetTargetItem(args, targetId);
            }
            else if (targetUrl.Length > 0)
            {
                isValid = true;
            }

            return isValid;
        }

        private bool TryGetTargetItem(HttpRequestArgs args, ID targetId)
        {
            var item = args.GetItem(targetId);
            if (item != null)
            {
                return true;
            }

            Log.Warn(String.Format("MultiSite404Handler: Found alias [{0}] but the target item does not exist", args.LocalPath), this);
            return false;
        }

        private bool ShouldUrlBeExcluded(string url)
        {
            var shouldBeExcluded = false;
            foreach (Exclude404Rule rule in _settings.Exclude404Rules)
            {
                switch (rule.Type)
                {
                    case Settings.ExcludeRuleType.Contains: 
                        shouldBeExcluded = url.Contains(rule.Match);
                        break;
                    case Settings.ExcludeRuleType.StartsWith:
                        shouldBeExcluded = url.StartsWith(rule.Match);
                        break;
                    case Settings.ExcludeRuleType.EndsWith:
                        shouldBeExcluded = url.EndsWith(rule.Match);
                        break;
                }

                if (shouldBeExcluded)
                    break;
            }

            return shouldBeExcluded;
        }

        private string CleanUrl(string rawval)
        {
            return rawval.Replace("*", String.Empty)
                .Replace("?", String.Empty)
                .Replace("<", String.Empty)
                .Replace(">", String.Empty)
                .Replace(",", String.Empty)
                .Replace(":", String.Empty)
                .Replace(";", String.Empty)
                .Replace("'", String.Empty)
                .Replace("\"", String.Empty)
                .Replace("[", String.Empty)
                .Replace("]", String.Empty)
                .Replace("//", "/")
                .Replace("\\\\", "\\");
        }
    }
}