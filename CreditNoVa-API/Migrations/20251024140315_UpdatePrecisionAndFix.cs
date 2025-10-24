using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFund_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePrecisionAndFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreditSurveys");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CreditSurveys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CompanyType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreditScore = table.Column<int>(type: "int", nullable: true),
                    CurrentOutstandingDebt = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EducationLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Facebook = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    HadPreviousLoans = table.Column<bool>(type: "bit", nullable: false),
                    HasSavingsAccount = table.Column<bool>(type: "bit", nullable: false),
                    IdentityNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Investments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LifeInsuranceValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LoanInstitution = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    LoanLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LoanTerm = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaritalStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MonthlyIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NumberOfDependents = table.Column<int>(type: "int", nullable: false),
                    Occupation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OwnCarOrValuableVehicle = table.Column<bool>(type: "bit", nullable: false),
                    OwnHouseOrLand = table.Column<bool>(type: "bit", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SalaryPaymentMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SalarySlipImage = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    SalarySlipImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UtilityBillImage = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    YearsAtCurrentJob = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditSurveys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditSurveys_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreditSurveys_UserId",
                table: "CreditSurveys",
                column: "UserId");
        }
    }
}
