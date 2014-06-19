using System.Configuration;

namespace Sitecore.MultisiteHttpModule.Configuration
{
    public class MultisiteHttpModuleSettings : ConfigurationSection
    {
        [ConfigurationProperty("defaultErrorPage", DefaultValue = "/error.html", IsRequired = true)]
        public string DefaultErrorPage
        {
            get { return base["defaultErrorPage"].ToString(); }
            set { base["defaultErrorPage"] = value; }
        }

        [ConfigurationProperty("errorsEnabled", DefaultValue = true, IsRequired = true)]
        public bool ErrorsEnabled
        {
            get { return (bool)base["errorsEnabled"]; }
            set { base["errorsEnabled"] = value; }
        }

        [ConfigurationProperty("notFoundEnabled", DefaultValue = true, IsRequired = true)]
        public bool NotFoundEnabled
        {
            get { return (bool)base["notFoundEnabled"]; }
            set { base["notFoundEnabled"] = value; }
        }

        [ConfigurationProperty("exclude404Rules", IsDefaultCollection = false)]
        public Exclude404RuleCollection Exclude404Rules
        {
            get { return base["exclude404Rules"] as Exclude404RuleCollection; }
        }
    }
}
