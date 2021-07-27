using customer_microservice.Datamodels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace customer_microservice.Datamodels
{ 
    public class DBContext : DbContext
    {
        public DbSet<CustomerDataModel> Customers { get; set; }
        public DbSet<AddressDataModel> Address { get; set; }

        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map entities to tables  
            modelBuilder.Entity<CustomerDataModel>().ToTable("customers");

            // Configure Primary Keys  
            modelBuilder.Entity<CustomerDataModel>().HasKey(ug => ug.Id).HasName("pk_Customers");

            // Configure indexes  
            modelBuilder.Entity<CustomerDataModel>().HasIndex(u => u.FirstName).HasDatabaseName("idx_FirstName");
            modelBuilder.Entity<CustomerDataModel>().HasIndex(u => u.LastName).HasDatabaseName("idx_LastName");

            // Configure columns  
            modelBuilder.Entity<CustomerDataModel>().Property(ug => ug.Id).HasColumnType("BINARY(16)").IsRequired();
            modelBuilder.Entity<CustomerDataModel>().Property(ug => ug.FirstName).HasColumnType("nvarchar(30)").IsRequired();
            modelBuilder.Entity<CustomerDataModel>().Property(ug => ug.LastName).HasColumnType("nvarchar(30)").IsRequired();

            modelBuilder.Entity<CustomerDataModel>().Property(ug => ug.CurrentAccountValue).HasColumnType("decimal(10,2)").IsRequired(false);
            modelBuilder.Entity<CustomerDataModel>().Property(ug => ug.TotalBuyValue).HasColumnType("decimal(10,2)").IsRequired(false);
            modelBuilder.Entity<CustomerDataModel>().Property(ug => ug.CurrentCreditValue).HasColumnType("decimal(10,2)").IsRequired(false);
            modelBuilder.Entity<CustomerDataModel>().Property(ug => ug.RowVersion).IsRowVersion();
            modelBuilder.Entity<CustomerDataModel>().Property(ug => ug.RowVersion).IsConcurrencyToken().ValueGeneratedOnAddOrUpdate();
            modelBuilder.Entity<CustomerDataModel>().Property(ug => ug.UpdateTimeStamp).ValueGeneratedOnAddOrUpdate();
            modelBuilder.Entity<CustomerDataModel>().Property(ug => ug.CreateTimeStamp).ValueGeneratedOnAdd();


            modelBuilder.Entity<CustomerDataModel>().HasOne(a => a.Address);

            //Addresses
            // Map entities to tables  
            modelBuilder.Entity<AddressDataModel>().ToTable("addresses");

            // Configure Primary Keys  
            modelBuilder.Entity<AddressDataModel>().HasKey(ug => ug.Id).HasName("pk_addresses");

            // Configure indexes  


            // Configure columns
            modelBuilder.Entity<AddressDataModel>().Property(ug => ug.Id).HasColumnType("BINARY(16)").IsRequired();
            modelBuilder.Entity<AddressDataModel>().Property(u => u.Address1).HasColumnType("nvarchar(30)");
            modelBuilder.Entity<AddressDataModel>().Property(u => u.Address2).HasColumnType("nvarchar(30)");
            modelBuilder.Entity<AddressDataModel>().Property(u => u.Address3).HasColumnType("nvarchar(30)");
            modelBuilder.Entity<AddressDataModel>().Property(u => u.ZipCode).HasColumnType("nvarchar(10)");
            modelBuilder.Entity<AddressDataModel>().Property(u => u.State).HasColumnType("nvarchar(30)");
            modelBuilder.Entity<AddressDataModel>().Property(ug => ug.RowVersion).IsRowVersion();
            modelBuilder.Entity<AddressDataModel>().Property(ug => ug.RowVersion).IsConcurrencyToken().ValueGeneratedOnAddOrUpdate();
            modelBuilder.Entity<AddressDataModel>().Property(ug => ug.UpdateTimeStamp).ValueGeneratedOnAddOrUpdate();
            modelBuilder.Entity<AddressDataModel>().Property(ug => ug.CreateTimeStamp).ValueGeneratedOnAdd();


            // Configure relationships  

        }

    }
}