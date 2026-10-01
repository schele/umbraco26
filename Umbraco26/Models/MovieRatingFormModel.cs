using System.ComponentModel.DataAnnotations;

namespace Umbraco26.Models
{
    public class MovieRatingFormModel
    {
        /// <summary>0 until the visitor has picked a rating.</summary>
        [Range(MovieRating.MinScore, MovieRating.MaxScore, ErrorMessage = "Slide over the stars to pick a rating.")]
        public double Score { get; set; }

        [StringLength(MovieRating.NameMaxLength)]
        public string? Name { get; set; }

        [StringLength(MovieRating.CommentMaxLength)]
        public string? Comment { get; set; }
    }
}
