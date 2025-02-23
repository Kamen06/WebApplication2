using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Text;
using WebApplication2.Data;

namespace WebApplication2.Models
{
    public class Book
    {
        
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(30)]
        public string Title { get; set; }
        [Required, MaxLength(15)]
        public string Genre { get; set; }
        [Required, MaxLength(250)]
        public string Summary { get; set; }

        [Display(Name = "Снимка")]
        public string BookPicture { get; set; }

        [Required]
        public int AuthorId { get; set; }

        public virtual Author Author { get; set; }
        [Required]
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}
