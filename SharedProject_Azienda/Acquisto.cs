using System;
using System.Windows.Controls;

namespace SharedProject_Azienda
{
    class Acquisto<T> where T : struct
    {
        Prodotti _tipo;
        int quantita;
        T _costo;
        string _note;
        public Acquisto(Prodotti tipo, T costo, string note = null)
        {
            _tipo = tipo;
            _costo = costo;
            _note = note;
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
            return _tipo.ToString() + $" {_costo:f2}";
        }
    }
}