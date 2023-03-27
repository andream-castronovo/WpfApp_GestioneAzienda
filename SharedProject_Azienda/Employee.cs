using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SharedProject_Azienda
{
    public class Employee<T> : Persona<T> where T : struct
    {

        
        private string _valuta = "€";
        
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
                _stipendioAnnuo = value;
            }
        }

        /// <summary>
        /// Calcolo stipendio mensile con tredicesima
        /// </summary>
        /// <returns>Stipendio mensile + tredicesima</returns>
        /// <exception cref="Exception">Parametro di tipo non numerico</exception>
        public override T GetEconomicValue()
        {
            T a;
            
            try
            {
                a = (dynamic)_stipendioAnnuo / 13; 
            }
            catch
            {
                throw new Exception("Il parametro ti tipo non è numerico.");
            }

            return a;
        }

        public override string ToString()
        {
            return base.ToString() + $"\nStipendio annuo:\n\t{_stipendioAnnuo}"+_valuta;
        }
    }
}
