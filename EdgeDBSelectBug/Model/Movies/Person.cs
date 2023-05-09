namespace DB.Model.Movies;

public class Person : HasImage
{
    public string FirstName { get; set; } = "";
    public string MiddleName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string FullName { get; init; } = "";

    public EdgeDB.DataTypes.DateTime Dob { get; set; }
    public EdgeDB.DataTypes.DateTime Dod { get; set; }

    public string Bio { get; set; } = "";
}
