using Hotel_Booking_System_Desktop_Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Project
{
    // Your DataDbContext class
    public class DataDbContext : DbContext
    {
        // DbSet properties
        public DbSet<User> Users { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<Admindata1> Admindata { get; set; }
        public DbSet<Update> Updates { get; set; }

        // Constructor that accepts DbContextOptions
        public DataDbContext(DbContextOptions<DataDbContext> options)
            : base(options)
        {
        }

        // OnConfiguring method (optional, but useful for design-time configuration)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Configure SQLite as the database provider
                optionsBuilder.UseSqlite(@"Data Source=D:\Projects\Hotel_Booking_System_Desktop_Application\product_app.db");
            }
        }
    }

    // Design-time DbContext factory
    public class DataDbContextFactory : IDesignTimeDbContextFactory<DataDbContext>
    {
        public DataDbContext CreateDbContext(string[] args)
        {
            // Configure DbContextOptions for design-time use
            var optionsBuilder = new DbContextOptionsBuilder<DataDbContext>();
            optionsBuilder.UseSqlite(@"Data Source=D:\Projects\Hotel_Booking_System_Desktop_Application\product_app.db");

            // Return a new instance of DataDbContext
            return new DataDbContext(optionsBuilder.Options);
        }
    }
}