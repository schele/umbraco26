using System.Globalization;
using Umbraco26.Models;

namespace Umbraco26.Business.Services.Interfaces
{
	public interface IFindService
	{
		List<Hit> FindContent(string query, CultureInfo cultureInfo);
	}
}