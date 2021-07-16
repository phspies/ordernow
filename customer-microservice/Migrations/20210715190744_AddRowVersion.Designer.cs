﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using customer_microservice.Datamodels;

namespace customer_microservice.Migrations
{
    [DbContext(typeof(CustomerDBContext))]
    [Migration("20210715190744_AddRowVersion")]
    partial class AddRowVersion
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.7");

            modelBuilder.Entity("customer_microservice.Datamodels.CustomerDataModel", b =>
                {
                    b.Property<byte[]>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("BINARY(16)");

                    b.Property<string>("Address1")
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("Address2")
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("Address3")
                        .HasColumnType("nvarchar(30)");

                    b.Property<DateTime>("CreateTimeStamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)");

                    b.Property<decimal?>("CurrentAccountValue")
                        .HasColumnType("decimal(10,2)");

                    b.Property<decimal?>("CurrentCreditValue")
                        .HasColumnType("decimal(10,2)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(30)");

                    b.Property<DateTime?>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp(6)");

                    b.Property<string>("State")
                        .HasColumnType("nvarchar(2)");

                    b.Property<decimal?>("TotalBuyValue")
                        .HasColumnType("decimal(10,2)");

                    b.Property<DateTime>("UpdateTimeStamp")
                        .ValueGeneratedOnUpdate()
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("ZipCode")
                        .HasColumnType("int(5)");

                    b.HasKey("Id")
                        .HasName("PK_Customers");

                    b.HasIndex("FirstName")
                        .HasDatabaseName("Idx_FirstName");

                    b.HasIndex("LastName")
                        .HasDatabaseName("Idx_LastName");

                    b.HasIndex("ZipCode")
                        .HasDatabaseName("Idx_ZipCode");

                    b.ToTable("Customers");
                });
#pragma warning restore 612, 618
        }
    }
}
