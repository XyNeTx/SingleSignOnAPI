using Microsoft.EntityFrameworkCore;

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

        }

        public DbSet<Employee> Employee { get; set; }
        public DbSet<HinoPersonData> HinoPersonData { get; set; }
    }
}
