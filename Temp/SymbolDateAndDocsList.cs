using FmpDataContext.Model;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FmpDataContext.Temp
{
    /// <summary>
    /// SymbolDateAndDocsList
    /// </summary>
    public class SymbolDateAndDocsList
    {
        private HashSet<SymbolDateAndDocs> _symbolDateAndDocsList;
        private DataContext _dataContext;

        /// <summary>
        /// SymbolDateAndDocsList
        /// </summary>
        public SymbolDateAndDocsList(DataContext dataContext)
        {
            _symbolDateAndDocsList = new HashSet<SymbolDateAndDocs>(9);
            _dataContext = dataContext;
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="document"></param>
        public void Add(object document)
        {
            if (document is IncomeStatement)
            {
                var symbolDate = ((IncomeStatement)document).Symbol + "_" + ((IncomeStatement)document).Date;
                this[symbolDate].IncomeStatement = (IncomeStatement)document;
            }
            else if (document is BalanceSheet)
            {
                var symbolDate = ((BalanceSheet)document).Symbol + "_" + ((BalanceSheet)document).Date;
                this[symbolDate].BalanceSheet = (BalanceSheet)document;
            }
            else if (document is CashFlowStatement)
            {
                var symbolDate = ((CashFlowStatement)document).Symbol + "_" + ((CashFlowStatement)document).Date;
                this[symbolDate].CashFlowStatement = (CashFlowStatement)document;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// SymbolDateAndDocs
        /// </summary>
        /// <param name="symbolDate"></param>
        /// <returns></returns>
        public SymbolDateAndDocs this[string symbolDate]
        {
            get
            {
                if (!_symbolDateAndDocsList.Any(sdd => sdd.SymbolDate == symbolDate))
                {
                    _symbolDateAndDocsList.Add(new SymbolDateAndDocs(_dataContext) { SymbolDate = symbolDate });
                }
                return _symbolDateAndDocsList.First(sdd => sdd.SymbolDate == symbolDate);
            }
        }

        public List<SymbolDateAndDocs> CompletedButNotSaved
        {
            get
            {
                return _symbolDateAndDocsList.Where(sdd => sdd.Completed && !sdd.Persisted).ToList();
            }
        }
    }
}
