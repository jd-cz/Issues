using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFMetadataIssue
{
    public class GroupEntity
    {
        public Guid ID { get; set; }
        public ICollection<PermissionEntity> Permissions { get; set; }
    }

    public class PermissionEntity
    {
        public Guid GroupID { get; set; }
        public Guid FunctionReference { get; set; }
    }

    public class TestDataContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Database=EFMetadataIssueDB;Trusted_Connection=True");

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PermissionEntity>(b =>
            {
                b.ToSqlServerTable("Permission");
                b.HasKey(ex => new { ex.GroupID, ex.FunctionReference });
            });

            modelBuilder.Entity<GroupEntity>(b =>
            {
                b.ToSqlServerTable("Group");
                b.HasKey(ex => ex.ID);
                b.HasMany(e => e.Permissions).WithOne().ForeignKey(e => e.GroupID);
            });      

            base.OnModelCreating(modelBuilder);
        }
    }

    public class Program
    {
        public void Main(string[] args)
        {
            TestDataContext context = new TestDataContext();
            var groups = context.Set<GroupEntity>().ToArray();

            Console.WriteLine($"OK - returned {groups.Count()} rows!");
        }
    }
}
