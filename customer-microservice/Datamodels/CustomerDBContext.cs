using customer_microservice.Datamodels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace customer_microservice.Datamodels
{ 
    public class CustomerDBContext : DbContext
    {
        public DbSet<CustomerDataModel> Customers { get; set; }

        public CustomerDBContext(DbContextOptions<CustomerDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map entities to tables  
            modelBuilder.Entity<CustomerDataModel>().ToTable("Customers");

            // Configure Primary Keys  
            modelBuilder.Entity<CustomerDataModel>().HasKey(ug => ug.Id).HasName("PK_Customers");

            // Configure indexes  
            modelBuilder.Entity<CustomerDataModel>().HasIndex(u => u.FirstName).HasDatabaseName("Idx_FirstName");
            modelBuilder.Entity<CustomerDataModel>().HasIndex(u => u.LastName).HasDatabaseName("Idx_LastName");
            modelBuilder.Entity<CustomerDataModel>().HasIndex(u => u.ZipCode).HasDatabaseName("Idx_ZipCode");

            // Configure columns  
            modelBuilder.Entity<CustomerDataModel>().Property(ug => ug.Id).HasColumnType("BINARY(16)").IsRequired();
            modelBuilder.Entity<CustomerDataModel>().Property(ug => ug.FirstName).HasColumnType("nvarchar(30)").IsRequired();
            modelBuilder.Entity<CustomerDataModel>().Property(ug => ug.LastName).HasColumnType("nvarchar(30)").IsRequired();
            modelBuilder.Entity<CustomerDataModel>().Property(ug => ug.Address1).HasColumnType("nvarchar(30)").IsRequired(false);
            modelBuilder.Entity<CustomerDataModel>().Property(ug => ug.Address2).HasColumnType("nvarchar(30)").IsRequired(false);
            modelBuilder.Entity<CustomerDataModel>().Property(ug => ug.Address3).HasColumnType("nvarchar(30)").IsRequired(false);
            modelBuilder.Entity<CustomerDataModel>().Property(ug => ug.State).HasColumnType("nvarchar(2)").IsRequired(false);
            modelBuilder.Entity<CustomerDataModel>().Property(ug => ug.ZipCode).HasColumnType("int(5)").IsRequired(false);

            modelBuilder.Entity<CustomerDataModel>().Property(ug => ug.CurrentAccountValue).HasColumnType("decimal(10,2)").IsRequired(false);
            modelBuilder.Entity<CustomerDataModel>().Property(ug => ug.TotalBuyValue).HasColumnType("decimal(10,2)").IsRequired(false);
            modelBuilder.Entity<CustomerDataModel>().Property(ug => ug.CurrentCreditValue).HasColumnType("decimal(10,2)").IsRequired(false);
            modelBuilder.Entity<CustomerDataModel>().Property(ug => ug.RowVersion).IsRowVersion();
            modelBuilder.Entity<CustomerDataModel>().Property(ug => ug.RowVersion).IsConcurrencyToken().ValueGeneratedOnAddOrUpdate();
            modelBuilder.Entity<CustomerDataModel>().Property(ug => ug.UpdateTimeStamp).ValueGeneratedOnAddOrUpdate();
            modelBuilder.Entity<CustomerDataModel>().Property(ug => ug.CreateTimeStamp).ValueGeneratedOnAdd();


            // Configure relationships  
            //modelBuilder.Entity<User>().HasOne<UserGroup>().WithMany().HasPrincipalKey(ug => ug.Id).HasForeignKey(u => u.UserGroupId).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_Users_UserGroups");

        }

    }
}