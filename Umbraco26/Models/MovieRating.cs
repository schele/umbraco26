namespace Umbraco26.Models
{
    /// <summary>A visitor's MovieFinder rating of a title, stored in the Umbraco database.</summary>
    public class MovieRating
    {
        public const double MinScore = 0.5;
        public const double MaxScore = 5;
        public const int NameMaxLength = 100;
        public const int CommentMaxLength = 2000;

        public Guid Id { get; set; }

        /// <summary>The OMDb/IMDb id, e.g. <c>tt0111161</c>.</summary>
        public string ImdbId { get; set; } = string.Empty;

        /// <summary>0.5–5 stars in steps of 0.5.</summary>
        public double Score { get; set; }

        public string? Name { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedUtc { get; set; }
    }
}
