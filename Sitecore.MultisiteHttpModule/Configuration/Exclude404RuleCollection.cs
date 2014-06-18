using System.Configuration;

namespace Sitecore.MultisiteHttpModule.Configuration
{
    public class Exclude404RuleCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new Exclude404Rule();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Exclude404Rule) element).Match;
        }

        new public Exclude404Rule this[string name]
        {
            get { return (Exclude404Rule)BaseGet(name); }
        }

        public Exclude404Rule this[int index]
        {
            get { return (Exclude404Rule)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }
    }
}
