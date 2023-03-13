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

        public Customer() : this ("<no_name>","<no_surname>")
        {}

        public Customer(string name, string surname) : this(name, surname, new List<Acquisto<T>>())
        {}

        public Customer(string name, string surname, List<Acquisto<T>> acquisti) : base(name, surname)
        {
            _listaAcquisti = acquisti;
        }

        public List<Acquisto<T>> ListaAcquisti
        {
            get => _listaAcquisti;
        }

        public string OttieniAcquisti(string prefix, string valuta)
        {
            string s = "";
            foreach (Acquisto<T> a in _listaAcquisti)
            {
                s += prefix + a + valuta;
            }
            return s;
        }
    }
}