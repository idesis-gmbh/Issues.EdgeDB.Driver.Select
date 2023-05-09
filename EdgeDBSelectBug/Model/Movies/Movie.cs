
namespace DB.Model.Movies;

public class Movie : HasImage
{
    public string Title { get; set; } = "<Production Title>";
    public Int32 Year { get; set; }
    public string? Description { get; set; }
    public float AvgRating { get; set; }
    public List<Actor> Actors { get; set; } = new();
    public List<Director> Directors { get; set; } = new();
}

