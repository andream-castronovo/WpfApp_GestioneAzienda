using System;
using System.Collections.Generic;
using System.Text;

namespace SharedProject_Azienda
{
    class Customer<T> : Persona<T> where T : struct
    {
        public override T GetEconomicValue()
        {
            throw new NotImplementedException();
        }

        List<Acquisto<T>> _listaAcquisti;

        public Customer(string name, string surname) : this(name, surname, new List<Acquisto<T>>())
        {}

        public Customer(string name, string surname, List<Acquisto<T>> acquisti) : base(name, surname)
        {
            _listaAcquisti = acquisti;
        }
    }
}