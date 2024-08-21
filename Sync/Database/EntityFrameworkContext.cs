using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Sync.Database
{
    internal class EntityFrameworkContext : DbContext, IEntityFrameworkContext
    {
        public EntityFrameworkContext(IConfiguration config) : base(config.GetValue("DBConnectionString"))
        {
        }

        public DbSet<AbbreviatedContact> Contacts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AbbreviatedContact>()
                .ToTable("Contacts")
                .HasKey(c => c.Id)
                .Property(c => c.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("Id");

            base.OnModelCreating(modelBuilder);
        }
    }
}
