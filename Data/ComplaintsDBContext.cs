using EtacToKiSync.Entities.Ki;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtacToKiSync.Data
{
    public class ComplaintsDBContext : DbContext
    {
        public ComplaintsDBContext(DbContextOptions<ComplaintsDBContext> options)
           : base(options)
        {
            this.Database.SetCommandTimeout(200);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

      /*      modelBuilder.Entity<Company>(entity =>
            {
                entity.ToTable(nameof(Company), "dbo");
                entity.HasKey(x => x.Id);
                entity.HasMany<Branch>(c => c.Branches)
                    .WithOne(b => b.company);
            });

            modelBuilder.Entity<Branch>(entity =>
            {
                entity.ToTable(nameof(Branch), "dbo");
                entity.HasKey(x => x.id);
                entity.HasOne<Company>(b => b.company)
                .WithMany(c => c.Branches)
                .HasForeignKey(b => b.companyId);
                entity.HasOne(b => b.address)
                .WithOne(a => a.Branch)
                .HasForeignKey<Branch>(b => b.addressId);
            });


            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable(nameof(Address), "dbo");
                entity.HasKey(x => x.id);
                entity.HasOne<Branch>()
                .WithOne(b => b.address);
            }); */

           base.OnModelCreating(modelBuilder);
        }
        public DbSet<Company> Company { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<Branch> Branch { get; set; }
        public DbSet<Part> Part { get; set; }
    }
}
