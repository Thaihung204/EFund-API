using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFund_API.Migrations
{
    /// <inheritdoc />
    public partial class AddTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CreditSurveys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IdentityNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaritalStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NumberOfDependents = table.Column<int>(type: "int", nullable: false),
                    EducationLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Occupation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CompanyType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    YearsAtCurrentJob = table.Column<int>(type: "int", nullable: false),
                    MonthlyIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SalaryPaymentMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SalarySlipImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwnHouseOrLand = table.Column<bool>(type: "bit", nullable: false),
                    OwnCarOrValuableVehicle = table.Column<bool>(type: "bit", nullable: false),
                    HasSavingsAccount = table.Column<bool>(type: "bit", nullable: false),
                    LifeInsuranceValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Investments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SalarySlipImage = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    UtilityBillImage = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    HadPreviousLoans = table.Column<bool>(type: "bit", nullable: false),
                    LoanInstitution = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    LoanLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrentOutstandingDebt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoanTerm = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Facebook = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreditScore = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditSurveys", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreditSurveys");
        }
    }
}
