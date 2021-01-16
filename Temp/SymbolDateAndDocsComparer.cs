using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FmpDataContext.Temp
{
    /// <summary>
    /// SymbolDateAndDocsComparer
    /// </summary>
    public class SymbolDateAndDocsComparer : IEqualityComparer<SymbolDateAndDocs>
    {
        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(SymbolDateAndDocs x, SymbolDateAndDocs y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.SymbolDate.ToUpper() == y.SymbolDate.ToUpper();
        }

        /// <summary>
        /// GetHashCode
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode([DisallowNull] SymbolDateAndDocs obj)
        {
            int hash = Encoding.Unicode.GetBytes(obj.SymbolDate.ToUpper()).Aggregate((r, n) => (byte)(r + n));
            return hash;
        }
    }
}
