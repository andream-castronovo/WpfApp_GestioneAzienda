using System;
using System.Collections.Generic;
using System.Text;

namespace SharedProject_Azienda.SortHelper
{
    class SortByBillDecrescent<T> : IComparer<Customer<T>> where T : struct
    {
        public int Compare(Customer<T> x, Customer<T> y)
        {
            return (int)((dynamic) y.SpesaTotale - x.SpesaTotale);
        }
    }
}
