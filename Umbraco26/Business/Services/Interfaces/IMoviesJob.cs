using Hangfire.Server;

namespace Umbraco26.Business.Services.Interfaces
{
    public interface IMoviesJob
    {
        void AddMovies(PerformContext context);
    }
}