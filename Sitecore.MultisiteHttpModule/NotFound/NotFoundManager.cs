﻿using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.MultisiteHttpModule.Configuration;
using Sitecore.Pipelines.HttpRequest;
using System;
using System.Configuration;
using System.Web;

namespace Sitecore.MultisiteHttpModule.NotFound
{
    public class NotFoundManager
    {
        private MultisiteHttpModuleSettings _settings;

        public void ProcessRequest(HttpRequestArgs args, HttpContext context)
        {
            EnsureSettingsArePopulated();
            if (RequestShouldBeProcessedAs404(args, context))
            {
                SwitchTo404Item();
            }          
        }

        private void EnsureSettingsArePopulated()
        {
            if (_settings == null)
            {
                _settings = ConfigurationManager.GetSection("multisiteHttpModule") as MultisiteHttpModuleSettings;
            }
        }

        private bool RequestShouldBeProcessedAs404(HttpRequestArgs args, HttpContext context)
        {
            return _settings.NotFoundEnabled 
                && !IsValidUrlRequest(context, args)
                && IsSitecoreContextAvailable();
        }

        private void SwitchTo404Item()
        {
            var notFoundPageId = Context.Site.Properties[Settings.Constants.PropertyNames.NotFoundPageId];
            if (string.IsNullOrEmpty(notFoundPageId))
            {
                Log.Warn(String.Format("Sitecore.MultisiteHttpModule.NotFound: 404 page id not populated for site [{0}]", Context.Site.Name), this);
                return;
            }

            if (ID.IsID(notFoundPageId) || notFoundPageId.StartsWith(Constants.ContentPath))
            {
                Context.Item = Context.Site.Database.GetItem(notFoundPageId);
            }
        }

        private bool IsValidUrlRequest(HttpContext context, HttpRequestArgs args)
        {
            return IsContextItemPopulated()
                || IsSitecoreCmsClientRequest()
                || IsValidAlias(args)
                || ShouldUrlBeExcluded(context.Request.RawUrl)
                || IsRequestForPhysicalFile(context);
        }

        private bool IsSitecoreContextAvailable()
        {
            return Context.Site != null
                   && Context.Database != null;
        }

        private bool IsContextItemPopulated()
        {
            return Context.Item != null;
        }

        private bool IsSitecoreCmsClientRequest()
        {
            return Context.Site != null
                && Context.Site.Name == Constants.ShellSiteName;
        }

        private bool IsRequestForPhysicalFile(HttpContext context)
        {
            return System.IO.File.Exists(context.Server.MapPath(CleanUrl(context.Request.RawUrl)));
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

            Log.Warn(String.Format("Sitecore.MultisiteHttpModule.NotFound: Found alias [{0}] but the target item does not exist", args.LocalPath), this);
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
