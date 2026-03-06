using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 

namespace ReimbursementTrackerApp.Migrations
{

    public partial class SeedExpenseCategories : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ExpenseCategories",
                columns: new[] { "ExpenseCategoryId", "CategoryName", "CreatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Travel", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Food", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Accommodation", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }


        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ExpenseCategories",
                keyColumn: "ExpenseCategoryId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "ExpenseCategories",
                keyColumn: "ExpenseCategoryId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "ExpenseCategories",
                keyColumn: "ExpenseCategoryId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));
        }
    }
}
