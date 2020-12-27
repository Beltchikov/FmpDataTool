using Microsoft.EntityFrameworkCore.Migrations;

namespace FmpDataTool.Migrations
{
    public partial class IncomeStatement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IncomeStatements",
                columns: table => new
                {
                    Date = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FillingDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcceptedDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Period = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Revenue = table.Column<double>(type: "float", nullable: false),
                    CostOfRevenue = table.Column<double>(type: "float", nullable: false),
                    GrossProfit = table.Column<double>(type: "float", nullable: false),
                    GrossProfitRatio = table.Column<double>(type: "float", nullable: false),
                    ResearchAndDevelopmentExpenses = table.Column<double>(type: "float", nullable: false),
                    GeneralAndAdministrativeExpenses = table.Column<double>(type: "float", nullable: false),
                    SellingAndMarketingExpenses = table.Column<double>(type: "float", nullable: false),
                    OtherExpenses = table.Column<double>(type: "float", nullable: false),
                    OperatingExpenses = table.Column<double>(type: "float", nullable: false),
                    CostAndExpenses = table.Column<double>(type: "float", nullable: false),
                    InterestExpense = table.Column<double>(type: "float", nullable: false),
                    DepreciationAndAmortization = table.Column<double>(type: "float", nullable: false),
                    Ebitda = table.Column<double>(type: "float", nullable: false),
                    Ebitdaratio = table.Column<double>(type: "float", nullable: false),
                    OperatingIncome = table.Column<double>(type: "float", nullable: false),
                    OperatingIncomeRatio = table.Column<double>(type: "float", nullable: false),
                    TotalOtherIncomeExpensesNet = table.Column<double>(type: "float", nullable: false),
                    IncomeBeforeTax = table.Column<double>(type: "float", nullable: false),
                    IncomeBeforeTaxRatio = table.Column<double>(type: "float", nullable: false),
                    IncomeTaxExpense = table.Column<double>(type: "float", nullable: false),
                    NetIncome = table.Column<double>(type: "float", nullable: false),
                    NetIncomeRatio = table.Column<double>(type: "float", nullable: false),
                    Eps = table.Column<double>(type: "float", nullable: false),
                    Epsdiluted = table.Column<double>(type: "float", nullable: false),
                    WeightedAverageShsOut = table.Column<double>(type: "float", nullable: false),
                    WeightedAverageShsOutDil = table.Column<double>(type: "float", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinalLink = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomeStatements", x => new { x.Symbol, x.Date });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncomeStatements");
        }
    }
}
