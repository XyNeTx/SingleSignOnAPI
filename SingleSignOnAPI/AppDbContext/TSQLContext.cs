using Microsoft.EntityFrameworkCore;

namespace SingleSignOnAPI.AppDbContext
{
    public class TSQLContext : DbContext
    {
        public TSQLContext(DbContextOptions<TSQLContext> options) : base(options)
        {
        }

        public DbSet<T_SQL_License> T_SQL_License { get; set; }
    }
}
