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
                public const string RobotsTxtFilename = "robotsTxtLocation";
            }

            public const string DefaultRobotsFile = "Robots.txt";
        }

        public enum ExcludeRuleType
        {
            Contains,
            StartsWith,
            EndsWith
        }
    }
}
