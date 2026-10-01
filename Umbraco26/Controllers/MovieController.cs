using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco26.Business.Services.Interfaces;
using Umbraco26.Models.ViewModels;

namespace Umbraco26.Controllers
{
    public class MovieController(PageControllerDependencies dependencies, IOmdbService omdbService)
        : BasePageController<Movie, MoviePageViewModel>(dependencies)
    {
        protected override MoviePageViewModel Build(Movie page) => new() { Content = page };

        /// <summary>
        /// Umbraco routes to the action named after the template, so this takes precedence over
        /// <c>Index</c> and lets the OMDb lookup run asynchronously. The title comes from <c>?id={imdbId}</c>.
        /// </summary>
        [ActionName("Movie")]
        public async Task<IActionResult> MovieAsync(string? id)
        {
            if (CurrentPage is not Movie page || string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var details = await omdbService.GetByIdAsync(id);

            if (details == null)
            {
                return NotFound();
            }

            return PageResult(page, new MoviePageViewModel { Content = page, Movie = details });
        }
    }
}
