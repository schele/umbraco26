using System.Globalization;
using Umbraco26.Models;

namespace Umbraco26.Business.Services.Interfaces
{
	public interface IFindService
	{
		/// <summary>Searches published pages in <paramref name="cultureInfo"/>; <paramref name="page"/> is 1-based.</summary>
		FindResult FindContent(string query, CultureInfo cultureInfo, int page, int pageSize);
	}
}