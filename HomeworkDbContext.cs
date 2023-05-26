using System.Data.Entity;
using Homework_17.Models;

namespace Homework_17
{
    public class HomeworkDbContext : DbContext
    {
        public HomeworkDbContext() : base("DbConnection") { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }

    }
}
