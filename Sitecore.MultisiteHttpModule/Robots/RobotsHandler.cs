using Sitecore.Diagnostics;
using Sitecore.MultisiteHttpModule.Configuration;
using System;
using System.Linq;
using System.Configuration;
using System.IO;
using System.Web;
using Sitecore.Sites;

namespace Sitecore.MultisiteHttpModule.Robots
{
    public class RobotsHandler : IHttpHandler
    {
        private MultisiteHttpModuleSettings _settings;
       
        public void ProcessRequest(HttpContext context)
        {
            EnsureSettingsArePopulated();

            var site = GetSiteFromDomain();
            var robotsFileLocation = GetRobotsFileLocation(site);
            SendRobotsResponse(context, site, robotsFileLocation);
        }

        private void SendRobotsResponse(HttpContext context, SiteContext site, string robotsFileLocation)
        {
            try
            {
                using (var robotsFile = new StreamReader(context.Server.MapPath("~/" + robotsFileLocation)))
                {
                    var fileContents = robotsFile.ReadToEnd();
                    context.Response.ContentType = "text/plain";
                    context.Response.Write(fileContents);
                }
            }
            catch(Exception ex)
            {
                Log.Error(String.Format("Sitecore.MultiSite404Handler: Unable to process robots for site [{0}] due to {1} {2}", site.Name, ex.Message, ex.StackTrace), this);
            }
        }

        private string GetRobotsFileLocation(SiteContext site)
        {
            if (site != null)
            {
                var robotsFileLocation = site.Properties[Settings.Constants.PropertyNames.RobotsTxtFilename];
                if (!String.IsNullOrEmpty(robotsFileLocation))
                {
                    return robotsFileLocation;
                }
            }
            return Settings.Constants.DefaultRobotsFile;
        }

        private SiteContext GetSiteFromDomain()
        {
            var sites = Sitecore.Configuration.Factory.GetSiteInfoList();
            var site = sites.FirstOrDefault(x => x.HostName.Contains(HttpContext.Current.Request.Url.DnsSafeHost));
            return site != null ? new SiteContext(site) : null;
        }

        private void EnsureSettingsArePopulated()
        {
            if (_settings == null)
            {
                _settings = ConfigurationManager.GetSection("multisiteHttpModule") as MultisiteHttpModuleSettings;
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
