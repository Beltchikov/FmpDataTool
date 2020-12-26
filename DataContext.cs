using FmpDataTool.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FmpDataTool
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> option) : base(option)
        {
        }

        public DbSet<Stock> Stocks { get; set; }
    }
}
