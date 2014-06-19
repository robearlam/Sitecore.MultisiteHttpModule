namespace Sitecore.MultisiteHttpModule
{
    public class Settings
    {
        public class Constants
        {
            public class PropertyNames
            {
                public const string NotFoundPageId = "notFoundPageId";
                public const string ErrorPagePath = "errorPagePath";
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
