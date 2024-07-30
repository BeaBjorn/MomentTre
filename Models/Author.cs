//dataannotations for validation
using System.ComponentModel.DataAnnotations;

//Namespace
namespace MomentTre.Models;

//Public class for Author
public class Author
{
    //Properties
    //Get and Set for type integer as Id
    public int Id { get; set; }

    //Get and Set for type string (not required to be a string) for Name
    //"Required" is used to make the field required
    [Required]
    public string? Name { get; set; }

    //Creating a has many relationship to the Book model
    public List<Book>? Books { get; set; }
}
