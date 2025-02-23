using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<WebApplication2.Models.Book> Books { get; set; } = default!;
        public virtual DbSet<WebApplication2.Models.Author> Authors { get; set; } = default!;
        public DbSet<WebApplication2.Models.Review> Reviews { get; set; } = default!;

        public ApplicationDbContext()
        {
                
        }

    }
}
