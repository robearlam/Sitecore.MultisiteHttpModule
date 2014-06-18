using System;
using System.Configuration;

namespace Sitecore.MultisiteHttpModule.Configuration
{
    public class Exclude404Rule : ConfigurationElement
    {
        [ConfigurationProperty("type", IsRequired = true)]
        public Settings.ExcludeRuleType Type
        {
            get { return (Settings.ExcludeRuleType)this["type"]; }
            set { this["type"] = value; }
        }

        [ConfigurationProperty("match", IsRequired = true)]
        public String Match
        {
            get { return this["match"].ToString(); }
            set { this["match"] = value; }
        }
    }
}
