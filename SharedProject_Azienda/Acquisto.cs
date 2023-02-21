using System;

namespace SharedProject_Azienda
{
    class Acquisto<T> where T : struct
    {
        string _name;
        T _costo;

        public Acquisto(string name, T costo)
        {
            _name = name;
            _costo = costo;
        }

        public T Price
        {
            get => _costo;
        }

        public string Name
        {
            get => _name;
        }
        
    }
}