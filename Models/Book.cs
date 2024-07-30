//dataannotations for validation
using System.ComponentModel.DataAnnotations;
//dataannotations for mapping to database
using System.ComponentModel.DataAnnotations.Schema;

//namespace
namespace MomentTre.Models;

//Public class for Book
public class Book
{
    //Properties

    //Get and Set for type integer as Id
    public int Id { get; set; }

    //Get and Set for type string (not required to be a string) for Name
    //"Required" is used to make the field required
    [Required]
    public string? Name { get; set; }
    //Get and Set for type string (not required to be a string) for Description
    public string? Description { get; set; }
    //Get and Set for type string (not required to be a string) for ImageName
    public string? ImageName { get; set; }

    //Using "NotMapped" so that this field is not stored in the database
    //Setting the deisplayName for this field to be "Image"
    //Get and Set for type IFromFile (not required to be an IFormFile) for ImageFile
    [NotMapped]
    [Display(Name = "Image")]
    public IFormFile? ImageFile { get; set; }

    //Creating a has one relationship to the Author model
    public int? AuthorId { get; set; }
    public Author? Author { get; set; }
}
