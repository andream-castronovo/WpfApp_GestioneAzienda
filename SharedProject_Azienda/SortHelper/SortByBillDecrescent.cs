using System;
using System.Collections.Generic;
using System.Text;

namespace SharedProject_Azienda.SortHelper
{
    class SortByBillDecrescent<T> : IComparer<Customer<T>> where T : struct
    {
        // Programmato da Andrea Maria Castronovo - 4°I - Data: 17/04/2023
        public int Compare(Customer<T> x, Customer<T> y)
        {
            return (int)((dynamic) y.SpesaTotale - x.SpesaTotale);
        }
    }
}
