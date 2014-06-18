using System.Configuration;

namespace Sitecore.MultisiteHttpModule.Configuration
{
    public class Site : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return this["name"].ToString(); }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("notFoundPageId", IsRequired = true)]
        public string NotFoundPageId
        {
            get { return this["notFoundPageId"].ToString(); }
            set { this["notFoundPageId"] = value; }
        }

        [ConfigurationProperty("errorPage", IsRequired = true)]
        public string ErrorPage
        {
            get { return this["errorPage"].ToString(); }
            set { this["errorPage"] = value; }
        }

        [ConfigurationProperty("robotsFile", IsRequired = true)]
        public string RobotsFile
        {
            get { return this["robotsFile"].ToString(); }
            set { this["robotsFile"] = value; }
        }
    }
}
