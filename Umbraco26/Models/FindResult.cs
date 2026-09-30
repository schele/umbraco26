namespace Umbraco26.Models
{
	/// <summary>One page of search hits, plus the total number of hits across all pages.</summary>
	public record FindResult(IReadOnlyList<Hit> Hits, long TotalCount)
	{
		public static FindResult Empty { get; } = new([], 0);
	}
}
