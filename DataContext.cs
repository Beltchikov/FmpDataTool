using FmpDataTool.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FmpDataTool
{
    /// <summary>
    /// DataContext
    /// </summary>
    public class DataContext : DbContext
    {
        /// <summary>
        /// OnConfiguring
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Configuration.Instance["ConnectionString"]);
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Stock> Stocks { get; set; }
    }
}
