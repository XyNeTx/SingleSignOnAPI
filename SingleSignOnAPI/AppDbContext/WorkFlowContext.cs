using Microsoft.EntityFrameworkCore;
using SingleSignOnAPI.Models.EmployeeInfo;

namespace SingleSignOnAPI.AppDbContext
{
    public class WorkFlowContext : DbContext
    {
        public WorkFlowContext(DbContextOptions<WorkFlowContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable("Employee", "Organize");
            modelBuilder.Entity<HinoPersonData>().ToTable("PummSoft", "HinoPersonData");

            modelBuilder.Entity<EmployeeInfo>().HasNoKey(); // If it's a raw query with no primary key
            modelBuilder.Entity<HinoPersonData>().HasNoKey(); // If it's a raw query with no primary key

        }

        public DbSet<Employee> Employee { get; set; }
        public DbSet<HinoPersonData> HinoPersonData { get; set; }
        public DbSet<EmployeeInfo> EmployeeInfo { get; set; }
    }
}
