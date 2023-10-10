using JustLearn1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JustLearn1.Data
{
    public class JustDbContext : IdentityDbContext
        
    {
        public JustDbContext(DbContextOptions<JustDbContext> options) : base(options)
        {          
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentFile> AssignmentFiles { get; set; }
        public DbSet<UserAssignment> UserAssignments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(

                    new Category { Id = 1, Name = "Web", DisplayOrder = 1 },
                    new Category { Id = 2, Name = "Backend", DisplayOrder = 2 },
                    new Category { Id = 3, Name = "Language", DisplayOrder = 3 }
                );
        }
    }
}
