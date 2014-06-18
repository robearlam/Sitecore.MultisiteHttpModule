using System.Configuration;

namespace Sitecore.MultisiteHttpModule.Configuration
{
    public class SiteCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new Site();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Site) element).Name;
        }

        new public Site this[string name]
        {
            get { return (Site)BaseGet(name); }
        }
    }
}
