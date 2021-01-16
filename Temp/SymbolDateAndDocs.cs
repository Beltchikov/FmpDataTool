using FmpDataContext.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FmpDataContext.Temp
{
    /// <summary>
    /// SymbolDateAndDocs
    /// </summary>
    public class SymbolDateAndDocs
    {
        private DataContext _dataContext;

        SymbolDateAndDocs() { }

        /// <summary>
        /// SymbolDateAndDocs
        /// </summary>
        /// <param name="dataContext"></param>
        public SymbolDateAndDocs(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        /// <summary>
        /// SymbolDate
        /// </summary>
        public string SymbolDate { get; set; }

        /// <summary>
        /// IncomeStatement
        /// </summary>
        public IncomeStatement IncomeStatement { get; set; }

        /// <summary>
        /// BalanceSheet
        /// </summary>
        public BalanceSheet BalanceSheet { get; set; }

        /// <summary>
        /// CashFlowStatement
        /// </summary>
        public CashFlowStatement CashFlowStatement { get; set; }

        /// <summary>
        /// Completed
        /// </summary>
        public bool Completed
        {
            get
            {
                return IncomeStatement != null && BalanceSheet != null && CashFlowStatement != null;
            }
        }

        /// <summary>
        /// Persisted
        /// </summary>
        public bool Persisted { get; set; }

        /// <summary>
        /// PersistenceFailed
        /// </summary>
        public bool PersistenceFailed { get; set; }

        /// <summary>
        /// Symbol
        /// </summary>
        public string Symbol
        {
            get
            {
                return SymbolDateAsArray()[0];
            }
        }

        /// <summary>
        /// Date
        /// </summary>
        public string Date
        {
            get
            {
                return SymbolDateAsArray()[1];
            }
        }

        /// <summary>
        /// SaveInDatabase
        /// </summary>
        public string SaveInDatabase()
        {
            if (_dataContext.Set<IncomeStatement>().Any(i => i.Symbol == IncomeStatement.Symbol && i.Date == IncomeStatement.Date))
            {
                return $"Income statement for {Symbol} {Date} exist already in database.";
            }
            if (_dataContext.Set<BalanceSheet>().Any(i => i.Symbol == BalanceSheet.Symbol && i.Date == BalanceSheet.Date))
            {
                return $"Balance sheet for {Symbol} {Date} exist already in database.";
            }
            if (_dataContext.Set<CashFlowStatement>().Any(i => i.Symbol == CashFlowStatement.Symbol && i.Date == CashFlowStatement.Date))
            {
                return $"Cash flow statement for {Symbol} {Date} exist already in database.";
            }

            try
            {
                _dataContext.Set<IncomeStatement>().Add(IncomeStatement);
                _dataContext.Set<BalanceSheet>().Add(BalanceSheet);
                _dataContext.Set<CashFlowStatement>().Add(CashFlowStatement);
                _dataContext.SaveChanges();

                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.ToString();
            }
        }

        /// <summary>
        /// SymbolDateAsArray
        /// </summary>
        /// <returns></returns>
        private string[] SymbolDateAsArray()
        {
            if (string.IsNullOrWhiteSpace(SymbolDate))
            {
                throw new ApplicationException("Unexpected");
            }

            var splitted = SymbolDate.Split("_");
            if (splitted.Length != 2)
            {
                throw new ApplicationException("Unexpected");
            }
            return splitted;
        }
    }
}
