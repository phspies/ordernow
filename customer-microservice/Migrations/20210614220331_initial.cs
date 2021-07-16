using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace customer_microservice.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    Address1 = table.Column<string>(type: "nvarchar(30)", nullable: true),
                    Address2 = table.Column<string>(type: "nvarchar(30)", nullable: true),
                    Address3 = table.Column<string>(type: "nvarchar(30)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(2)", nullable: true),
                    ZipCode = table.Column<int>(type: "int(5)", nullable: true),
                    CurrentAccountValue = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TotalBuyValue = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CurrentCreditValue = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CreateTimeStamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateTimeStamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "Idx_FirstName",
                table: "Customers",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "Idx_LastName",
                table: "Customers",
                column: "LastName");

            migrationBuilder.CreateIndex(
                name: "Idx_ZipCode",
                table: "Customers",
                column: "ZipCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
