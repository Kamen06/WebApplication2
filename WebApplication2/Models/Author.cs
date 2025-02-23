using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class Author
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(40)]
        public string FullName { get; set; }
        [Required]
        public bool Deceased { get; set; }

        [Display(Name = "Снимка")]
        public string? ProfilePicture { get; set; }


        [Required]
        public string Biography { get; set; }
        public List<Book> Books { get; set; } = new List<Book>();
    }
}
