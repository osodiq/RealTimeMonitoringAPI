using Microsoft.EntityFrameworkCore;
using RealTimeMonitoringAPI.Model;
using System.Collections.Generic;

namespace RealTimeMonitoringAPI
{
    public class RealTimeMonitoringDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public RealTimeMonitoringDbContext(DbContextOptions<RealTimeMonitoringDbContext> options)
            : base(options)
        {
        }
    }
}
