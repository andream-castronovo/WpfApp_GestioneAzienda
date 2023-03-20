using System;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace SharedProject_Azienda
{
    class Acquisto<T> where T : struct
    {
        Prodotti _tipo;

        int quantita;
        
        [JsonProperty]
        T _costo;
        
        string _note;

        [JsonProperty]
        string _valuta;

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