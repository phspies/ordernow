using order_microservice.Datamodels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace order_microservice.Datamodels
{
    public class DBContext : DbContext
    {
        public DbSet<CustomerDataModel> Customers { get; set; }
        public DbSet<AddressDataModel> Address { get; set; }
        public DbSet<OrderDataModel> Orders { get; set; }

        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }
        public bool Exists()
        {
            return (this.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists();
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
            modelBuilder.Entity<CustomerDataModel>().HasMany(a => a.Orders);

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



            //            public Guid Id { get; set; }
            //public Guid CustomerId { get; set; }
            //public Guid SupplierId { get; set; }
            //public int StatusCode { get; set; }
            //public DateTime OrderPlaced { get; set; }
            //public DateTime OrderFilled { get; set; }
            //public string Memo { get; set; }
            // Configure relationships  


            //Addresses
            // Map entities to tables  
            modelBuilder.Entity<OrderDataModel>().ToTable("orders");

            // Configure Primary Keys  
            modelBuilder.Entity<OrderDataModel>().HasKey(ug => ug.Id).HasName("pk_orders");

            // Configure indexes  


            // Configure columns
            modelBuilder.Entity<OrderDataModel>().Property(ug => ug.Id).HasColumnType("BINARY(16)").IsRequired();
            modelBuilder.Entity<OrderDataModel>().Property(u => u.StatusCode).HasColumnType("TINYINT");
            modelBuilder.Entity<OrderDataModel>().Property(u => u.OrderPlaced);
            modelBuilder.Entity<OrderDataModel>().Property(u => u.OrderFilled);
            modelBuilder.Entity<OrderDataModel>().Property(u => u.Memo);
            modelBuilder.Entity<OrderDataModel>().Property(ug => ug.RowVersion).IsRowVersion();
            modelBuilder.Entity<OrderDataModel>().Property(ug => ug.RowVersion).IsConcurrencyToken().ValueGeneratedOnAddOrUpdate();
            modelBuilder.Entity<OrderDataModel>().Property(ug => ug.UpdateTimeStamp).ValueGeneratedOnAddOrUpdate();
            modelBuilder.Entity<OrderDataModel>().Property(ug => ug.CreateTimeStamp).ValueGeneratedOnAdd();

            modelBuilder.Entity<OrderDataModel>().HasOne(a => a.Customer);
            modelBuilder.Entity<OrderDataModel>().HasOne(a => a.Supplier);

        }

    }
}