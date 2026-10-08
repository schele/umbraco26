namespace Umbraco26.Models
{
    /// <summary>
    /// The values of the Meta Robots property on pages with the <c>base</c> composition, as stored by the
    /// "Meta Robots" property editor (wwwroot/App_Plugins/MetaRobots).
    /// </summary>
    public static class MetaRobotsValues
    {
        /// <summary>Search engines may index the page. The default, also for pages without a value.</summary>
        public const string All = "ALL";

        /// <summary>Search engines should not index the page, so it is left out of the sitemap.</summary>
        public const string None = "NONE";
    }
}
