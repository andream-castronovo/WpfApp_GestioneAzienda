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

        List<string> _listaAcquisti;

        public Customer() : base() 
        {
            _listaAcquisti = new List<string>();
        }
        public Customer(string nome, string cognome) : this(nome, cognome, new List<string>()) { }

        public Customer(string nome, string cognome, List<string> acquisti)
        {
            _listaAcquisti = acquisti;
        }
    }
}
