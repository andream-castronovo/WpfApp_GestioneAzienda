using System;
using System.Collections.Generic;
using System.Text;

namespace SharedProject_Azienda.SortHelper
{
    class SortBySurname<T> : IComparer<Persona<T>> where T : struct
    {
        // Programmato da Andrea Maria Castronovo - 4°I - Data: 17/04/2023
        public int Compare(Persona<T> x, Persona<T> y)
        {
            return string.Compare(x.Cognome, y.Cognome);
        }
    }
}
