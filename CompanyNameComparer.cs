using FmpDataContext.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FmpDataTool
{
    public class CompanyNameComparer : IEqualityComparer<Stock>
    {
        public bool Equals(Stock x, Stock y)
        {
            if(x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.Name.ToUpper() == y.Name.ToUpper();
        }

        public int GetHashCode([DisallowNull] Stock obj)
        {
            int hash = Encoding.Unicode.GetBytes(obj.Name.ToUpper()).Aggregate((r, n) => (byte)(r + n));
            return hash;
        }
    }
}
