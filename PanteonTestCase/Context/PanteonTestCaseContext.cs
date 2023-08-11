using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace PanteonTestCase.Context
{
    public class PanteonTestCaseContext : DbContext
    {

        private IConfiguration _configuration;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetSection("DbConnectionString").Value);
        }

        public PanteonTestCaseContext() : base()
        {

#if DEBUG
            IConfiguration Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            _configuration = Configuration;
#else
            IConfiguration Configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.Development.json")
               .Build();
            _configuration = Configuration;
#endif

            this.Database.SetCommandTimeout(999999);
        }

        public DbSet<User> Users { get; set; }
    }
}
