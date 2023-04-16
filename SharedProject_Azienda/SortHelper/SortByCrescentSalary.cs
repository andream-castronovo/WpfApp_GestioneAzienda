using System;
using System.Collections.Generic;
using System.Text;

namespace SharedProject_Azienda.SortHelper
{
    class SortByCrescentSalary<T> : IComparer<Employee<T>> where T : struct
    {
        // Programmato da Andrea Maria Castronovo - 4°I - Data: 17/04/2023
        public int Compare(Employee<T> x, Employee<T> y)
        {
            return (int)((dynamic)x.StipendioAnnuo - y.StipendioAnnuo);
        }
    }
}
