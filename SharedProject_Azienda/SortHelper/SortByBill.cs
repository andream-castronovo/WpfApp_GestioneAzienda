using System;
using System.Collections.Generic;
using System.Text;

namespace SharedProject_Azienda.SortHelper
{
    class SortByBill<T> : IComparer<Customer<T>> where T : struct
    {
        public int Compare(Customer<T> x, Customer<T> y)
        {
            return (dynamic)x.SpesaTotale - y.SpesaTotale;
        }
    }
}
