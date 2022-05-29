using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PS.Data
{
    public class MainContext : IdentityDbContext, IDisposable
    {
        bool useDefaultCnnectionString; 
        public MainContext()  { this.useDefaultCnnectionString = true; }
        public MainContext(DbContextOptions<MainContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (useDefaultCnnectionString)
            {
                string dbConnectionString = Environment.GetEnvironmentVariable("CS", 
                    EnvironmentVariableTarget.Process) ?? new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("CS");
                optionsBuilder.UseSqlServer(dbConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUser>().HasData(new IdentityUser { Id = "64178325-cad7-42c0-9474-583800141e2e", UserName = "admin@admin.com", NormalizedUserName = "ADMIN@ADMIN.COM", Email = "admin@admin.com", NormalizedEmail = "ADMIN@ADMIN.COM", EmailConfirmed = true, PasswordHash = "AQAAAAEAACcQAAAAEAm10iVbfFso9WXr1xZkHKh/t3m4AKgTOoMP4ES2FMzmgRZshGu1z7/aZ0KjdL8tIw==", SecurityStamp = "DWXAEFIDGFEDNRMJRVJNWDCY5TIAGSLH", ConcurrencyStamp = "7e75eee3-4987-44a5-8b0e-daad163d167a", LockoutEnabled = true, });
            builder.Entity<Group>().HasMany(x => x.Customers).WithOne(x => x.Group).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Group>().HasData(
                new Group[]{
                    new Group { Id = 1, Name = "Teachers", DataUsageControl = true, DataUsage = 2 * 1024, TimeUsageControl = true, TimeUsageStart = new DateTime(1900, 1, 1, 8, 0, 0), TimeUsageEnd = new DateTime(1900, 1, 1, 22, 0, 0), },
                    new Group { Id = 2, Name = "Students", DataUsageControl = true, DataUsage = 1 * 1024, TimeUsageControl = true, TimeUsageStart = new DateTime(1900, 1, 1, 8, 0, 0), TimeUsageEnd = new DateTime(1900, 1, 1, 14, 0, 0), },
                });
            builder.Entity<Customer>().HasData(
                new Customer[]{
                        new Customer { Id = 1, GroupId = 1, Name = "Teacher1", Username = "Teacher1", Password="Teacher1"},
                        new Customer { Id = 2, GroupId = 1, Name = "Teacher2", Username = "Teacher2", Password="Teacher2"},
                        new Customer { Id = 3, GroupId = 2, Name = "Student1", Username = "Student1", Password = "Student1" },
                        new Customer { Id = 4, GroupId = 2, Name = "Student2", Username = "Student2", Password = "Student2" },
                });
            builder.Entity<BlackList>().HasData(new BlackList { Id = 1, Url = "msn.com", });
        }

        public override ValueTask DisposeAsync()
        {
            return base.DisposeAsync();
        }

        public DbSet<Group> Groups { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<RequestInfo> RequestInfos { get; set; }
        public DbSet<BlackList> BlackLists { get; set; }
    }
}