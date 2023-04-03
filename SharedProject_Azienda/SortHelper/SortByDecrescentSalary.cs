using System;
using System.Collections.Generic;
using System.Text;

namespace SharedProject_Azienda.SortHelper
{
    class SortByDecrescentSalary<T> : IComparer<Employee<T>> where T : struct
    {
        public int Compare(Employee<T> x, Employee<T> y)
        {
            return (dynamic) y.StipendioAnnuo - x.StipendioAnnuo;
        }
    }
}
