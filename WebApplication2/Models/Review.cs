using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }



        [MaxLength(200)]
        public string Text { get; set; }
        public int BookId { get; set; }
        public virtual Book Book { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
