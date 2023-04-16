using System;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SharedProject_Azienda
{
    public class Acquisto<T> where T : struct
    {
        // Programmato da Andrea Maria Castronovo - 4°I - Data: 17/04/2023
        Prodotti _tipo;

        [JsonProperty]
        
        T _costo;
        
        string _note;

        [JsonProperty]
        string _valuta = "€";

        public Acquisto()
        {}

        public Acquisto(Prodotti tipo, T costo, string valuta="€" ,string note = null)
        {
            _tipo = tipo;
            _costo = costo;
            _note = note;
            _valuta = valuta;
        }


        public T Price
        {
            get => _costo;
            set
            {
                if ((dynamic) _costo == default(T))
                    _costo = value;
            }
        }

        public Prodotti Tipo
        {
            get => _tipo;
        }

        /// <summary>
        /// Può essere null.
        /// </summary>
        public string Note
        {
            get => _note;
        }

        public override string ToString()
        {
            return _tipo.ToString() + $" {_costo:f2}" + _valuta;
        }
    }
}