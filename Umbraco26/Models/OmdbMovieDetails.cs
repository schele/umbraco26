using Newtonsoft.Json;

namespace Umbraco26.Models
{
    /// <summary>A single title from the OMDb API (<c>?i={imdbId}</c>).</summary>
    public class OmdbMovieDetails
    {
        [JsonProperty("Title")]
        public string Title { get; set; } = string.Empty;

        [JsonProperty("Year")]
        public string Year { get; set; } = string.Empty;

        [JsonProperty("Rated")]
        public string Rated { get; set; } = string.Empty;

        [JsonProperty("Released")]
        public string Released { get; set; } = string.Empty;

        [JsonProperty("Runtime")]
        public string Runtime { get; set; } = string.Empty;

        [JsonProperty("Genre")]
        public string Genre { get; set; } = string.Empty;

        [JsonProperty("Director")]
        public string Director { get; set; } = string.Empty;

        [JsonProperty("Writer")]
        public string Writer { get; set; } = string.Empty;

        [JsonProperty("Actors")]
        public string Actors { get; set; } = string.Empty;

        [JsonProperty("Plot")]
        public string Plot { get; set; } = string.Empty;

        [JsonProperty("Language")]
        public string Language { get; set; } = string.Empty;

        [JsonProperty("Country")]
        public string Country { get; set; } = string.Empty;

        [JsonProperty("Awards")]
        public string Awards { get; set; } = string.Empty;

        [JsonProperty("Poster")]
        public string Poster { get; set; } = string.Empty;

        [JsonProperty("Ratings")]
        public List<OmdbRating> Ratings { get; set; } = [];

        [JsonProperty("Metascore")]
        public string Metascore { get; set; } = string.Empty;

        [JsonProperty("imdbRating")]
        public string ImdbRating { get; set; } = string.Empty;

        [JsonProperty("imdbVotes")]
        public string ImdbVotes { get; set; } = string.Empty;

        [JsonProperty("imdbID")]
        public string ImdbID { get; set; } = string.Empty;

        [JsonProperty("Type")]
        public string Type { get; set; } = string.Empty;

        [JsonProperty("BoxOffice")]
        public string BoxOffice { get; set; } = string.Empty;

        [JsonProperty("Response")]
        public string Response { get; set; } = string.Empty;
    }

    public class OmdbRating
    {
        [JsonProperty("Source")]
        public string Source { get; set; } = string.Empty;

        [JsonProperty("Value")]
        public string Value { get; set; } = string.Empty;
    }
}
