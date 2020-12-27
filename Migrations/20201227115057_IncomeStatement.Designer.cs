﻿// <auto-generated />
using System;
using FmpDataTool;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FmpDataTool.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20201227115057_IncomeStatement")]
    partial class IncomeStatement
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("FmpDataTool.Model.Batch", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("DataTransferId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("End")
                        .HasColumnType("datetime2");

                    b.Property<string>("EndSymbol")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Start")
                        .HasColumnType("datetime2");

                    b.Property<string>("StartSymbol")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Batches");
                });

            modelBuilder.Entity("FmpDataTool.Model.DataTransfer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("End")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Start")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("DataTransfer");
                });

            modelBuilder.Entity("FmpDataTool.Model.IncomeStatement", b =>
                {
                    b.Property<string>("Symbol")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Date")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AcceptedDate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("CostAndExpenses")
                        .HasColumnType("float");

                    b.Property<double>("CostOfRevenue")
                        .HasColumnType("float");

                    b.Property<double>("DepreciationAndAmortization")
                        .HasColumnType("float");

                    b.Property<double>("Ebitda")
                        .HasColumnType("float");

                    b.Property<double>("Ebitdaratio")
                        .HasColumnType("float");

                    b.Property<double>("Eps")
                        .HasColumnType("float");

                    b.Property<double>("Epsdiluted")
                        .HasColumnType("float");

                    b.Property<string>("FillingDate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FinalLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("GeneralAndAdministrativeExpenses")
                        .HasColumnType("float");

                    b.Property<double>("GrossProfit")
                        .HasColumnType("float");

                    b.Property<double>("GrossProfitRatio")
                        .HasColumnType("float");

                    b.Property<double>("IncomeBeforeTax")
                        .HasColumnType("float");

                    b.Property<double>("IncomeBeforeTaxRatio")
                        .HasColumnType("float");

                    b.Property<double>("IncomeTaxExpense")
                        .HasColumnType("float");

                    b.Property<double>("InterestExpense")
                        .HasColumnType("float");

                    b.Property<string>("Link")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("NetIncome")
                        .HasColumnType("float");

                    b.Property<double>("NetIncomeRatio")
                        .HasColumnType("float");

                    b.Property<double>("OperatingExpenses")
                        .HasColumnType("float");

                    b.Property<double>("OperatingIncome")
                        .HasColumnType("float");

                    b.Property<double>("OperatingIncomeRatio")
                        .HasColumnType("float");

                    b.Property<double>("OtherExpenses")
                        .HasColumnType("float");

                    b.Property<string>("Period")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("ResearchAndDevelopmentExpenses")
                        .HasColumnType("float");

                    b.Property<double>("Revenue")
                        .HasColumnType("float");

                    b.Property<double>("SellingAndMarketingExpenses")
                        .HasColumnType("float");

                    b.Property<double>("TotalOtherIncomeExpensesNet")
                        .HasColumnType("float");

                    b.Property<double>("WeightedAverageShsOut")
                        .HasColumnType("float");

                    b.Property<double>("WeightedAverageShsOutDil")
                        .HasColumnType("float");

                    b.HasKey("Symbol", "Date");

                    b.ToTable("IncomeStatements");
                });

            modelBuilder.Entity("FmpDataTool.Model.Stock", b =>
                {
                    b.Property<string>("Symbol")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Exchange")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.HasKey("Symbol");

                    b.ToTable("Stocks");
                });
#pragma warning restore 612, 618
        }
    }
}