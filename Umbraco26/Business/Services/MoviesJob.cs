using Hangfire.Console;
using Hangfire.Server;
using Umbraco26.Business.Services.Interfaces;

namespace Umbraco26.Business.Services
{
    public class MoviesJob : IMoviesJob
    {
        public void AddMovies(PerformContext context)
        {
            var progressbar = context.WriteProgressBar();
            var movies = new List<MyMovie>();

            for (int i = 0; i < 10000; i++)
            { 
                var movie = new MyMovie
                { 
                    Name = $"Movie {i}"
                };

                movies.Add(movie);
            }

            foreach (var movie in movies.WithProgress(progressbar, movies.Count()))
            {
                context.WriteLine($"Movie {movie.Name} added");
            }
        }
    }

    internal class MyMovie
    {
        public string Name { get; set; }
    }
}