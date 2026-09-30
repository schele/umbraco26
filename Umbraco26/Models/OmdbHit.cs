using Newtonsoft.Json;

namespace Umbraco26.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Root
    {
        [JsonProperty("Search")]
        public List<OmdbMovie> Search { get; set; } = [];

        [JsonProperty("totalResults")]
        public string TotalResults { get; set; } = string.Empty;

        [JsonProperty("Response")]
        public string Response { get; set; } = string.Empty;
    }

    public class OmdbMovie
    {
        [JsonProperty("Title")]
        public string Title { get; set; } = string.Empty;

        [JsonProperty("Year")]
        public string Year { get; set; } = string.Empty;

        [JsonProperty("imdbID")]
        public string ImdbID { get; set; } = string.Empty;

        [JsonProperty("Type")]
        public string Type { get; set; } = string.Empty;

        [JsonProperty("Poster")]
        public string Poster { get; set; } = string.Empty;
    }
}
