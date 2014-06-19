namespace Sitecore.MultisiteHttpModule
{
    public class Settings
    {
        public class Constants
        {
            public class PropertyNames
            {
                public const string NotFoundPageId = "notFoundPageId";
            }
        }

        public enum ExcludeRuleType
        {
            Contains,
            StartsWith,
            EndsWith
        }
    }
}
