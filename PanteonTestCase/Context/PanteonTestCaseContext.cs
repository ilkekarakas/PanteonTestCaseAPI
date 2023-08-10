using Microsoft.EntityFrameworkCore;

namespace PanteonTestCase.Context
{
    public class PanteonTestCaseContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Database=PanteonTestCase");
        }
        public DbSet<User> Users { get; set; }
    }
}
