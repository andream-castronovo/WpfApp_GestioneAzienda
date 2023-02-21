using System;
using System.Collections.Generic;
using System.Text;

namespace SharedProject_Azienda
{
    class Employee<T> : Persona<T> where T : struct
    {
        private T _stipendioAnnuo;

        public Employee() : base()
        { }

        public Employee(string nome, string cognome, T stipendioAnnuo) : base(nome, cognome) 
        {
            StipendioAnnuo = stipendioAnnuo; // Uso la proprietà perché così quando implementerò controlli non sarà da cambiare.
            
        }

        public T StipendioAnnuo
        {
            get => _stipendioAnnuo;
            set
            {
                // TODO: Fare controlli
                _stipendioAnnuo = value;
            }
        }

        public override T GetEconomicValue()
        {
            // TODO
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return base.ToString() + $"\nStipendio annuo:\n\t{_stipendioAnnuo}";
        }
    }
}
