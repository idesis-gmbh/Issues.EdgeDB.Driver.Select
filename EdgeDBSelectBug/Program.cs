using EdgeDB;
using DB.Model.Movies;
using System.Collections.Generic;

namespace SelectBugDemo;

public class Program
{
    private readonly EdgeDBClient _Client;

    public static async Task Main(string[] args)
    {
        try
        {
            Program demo = new();
            await demo.checkDB();

            IReadOnlyCollection<Movie?> movies = await demo.selectMovies();
            foreach (var movie in movies)
                PrintMovie(movie);
        }
        catch (Exception e)
        {
            if (e.Source != null)
                Console.WriteLine($"Exception source: {e.Source}");
            Console.WriteLine(e.ToString());
        }
    }

    public Program()
    {
        _Client = CreateClient(CreateConnection(), CreatePoolConfig());
    }

    #region DB connection stuff

    private static EdgeDBConnection CreateConnection()
    {
        var connection = EdgeDBConnection.FromInstanceName("SelectBug");
        return connection;
    }

    private static EdgeDBClientPoolConfig CreatePoolConfig() =>
        new()
        {
            ConnectionTimeout = 5000u,
            SchemaNamingStrategy = INamingStrategy.SnakeCaseNamingStrategy,
        };

    private static EdgeDBClient CreateClient(EdgeDBConnection connection, EdgeDBClientPoolConfig config) =>
        new(connection, config);

    private async Task checkDB()
    {
        int answer = await selectAddition();
        if (answer != 42)
            throw new ConfigurationException("Please check your database connection.");
        // OK, database connection is working
    }

    #endregion
    #region Queries
    private async Task<IReadOnlyCollection<Movie?>> selectMovies()
    {
        var queryOK = $$"""
            select Movie {
                id,
                title,
                year,
                image,
                description,
                actors: {
                    id,
                    full_name,
                    middle_name,
                    first_name,
                    last_name,
                    image,
                    dob,
                    dod,
                    bio,
                    @list_order
                },
                directors: {
                    id,
                    full_name,
                    first_name,
                    middle_name,
                    last_name,
                    image,
                    dob,
                    dod,
                    bio,
                    @list_order
                },
                # avg_rating
            };
            """;

        // To fix the following query you can remove one of the dob or dod fields
        // of the actor or director structure. OR you can move e.g.
        // movie.actors.middle_name one position up.
        //
        // There are many workarounds, but it is unclear why some work and some
        // don't. Note: The error message says that the driver wants to convert
        // a type director to actor. But it doesn't say why it wants to do that.
        var queryFail = $$"""
            select Movie {
                id,
                title,
                year,
                description,
                actors: {
                    id,
                    full_name,
                    first_name,
                    middle_name,
                    last_name,
                    image,
                    dod,
                    dob,
                    bio,
                    @list_order
                },
                directors: {
                    id,
                    full_name,
                    first_name,
                    middle_name,
                    last_name,
                    image,
                    dod,
                    dob,
                    bio,
                    @list_order
                },
                # avg_rating
            };
            """;

        var query = queryOK;

        Console.WriteLine(query);
        return await _Client.QueryAsync<Movie?>(query);
    }

    private async Task<int> selectAddition() =>
        await _Client.QuerySingleAsync<int>("select (40 + 2);");

    #endregion
    #region Output

    private static void PrintMovie(Movie? movie)
    {
        if (movie is null)
            return;

        Console.WriteLine($"{movie.Title} / {movie.Year}, Rated {movie.AvgRating}");
        Console.WriteLine($"{movie.Image}");
        Console.WriteLine($"{movie.Description}");

        PrintDirectors(movie.Directors);
        PrintActors(movie.Actors);
    }

    private static void PrintDirectors(List<Director> directors) =>
        PrintCrew(directors, "Directed by:");

    private static void PrintActors(List<Actor> actors) =>
        PrintCrew(actors, "With:");

    private static void PrintCrew<T>(List<T> crew, string header) where T : Crew
    {
        if (crew is null || crew.Count == 0)
        {
            return;
        }

        Console.WriteLine(header);

        // TODO order (intentionally) by list_order
        foreach (var member in crew)
        {
            Console.WriteLine($"{member.ListOrder}) {member.FullName}");
            // Console.WriteLine($"   {member.FirstName} {member.MiddleName} {member.LastName}");
            Console.WriteLine($"   {member.Image}");
            Console.WriteLine($"   {member.Dob.DateTimeOffset.Date.ToShortDateString()} - {member.Dod.DateTimeOffset.Date.ToShortDateString()}");
            Console.WriteLine($"   {member.Bio}");
        }
    }
    #endregion

}
