using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SharedProject_Azienda
{
    #pragma warning disable CS0660 // Warning disabilito perché mi consiglia di eseguire override di metodi di object (Per override degli operatori)
    #pragma warning disable CS0661 // Warning disabilito perché mi consiglia di eseguire override di metodi di object (Per override degli operatori)
    public class Customer<T> : Persona<T>, IComparable<Customer<T>> where T : struct
    {
        // Programmato da Andrea Maria Castronovo - 4°I - Data: 17/04/2023
        public override T GetEconomicValue() => SpesaTotale;

        List<Acquisto<T>> _listaAcquisti;

        public Customer() : base()
        {
            _listaAcquisti = new List<Acquisto<T>>();
        }

        public Customer(string name, string surname) : this(name, surname, new List<Acquisto<T>>())
        { }

        public Customer(string name, string surname, List<Acquisto<T>> acquisti) : base(name, surname)
        {
            _listaAcquisti = acquisti;
        }

        public List<Acquisto<T>> ListaAcquisti
        {
            get => _listaAcquisti;
            set => _listaAcquisti = value;
        }

        public string OttieniAcquisti(string prefix)
        {
            string s = "";
            foreach (Acquisto<T> a in _listaAcquisti)
            {
                s += prefix + a;
            }
            return s;
        }

        public T SpesaTotale
        {
            get
            {
                T spesa = default;
                foreach (Acquisto<T> a in ListaAcquisti)
                    spesa += (dynamic)a.Price;
                return spesa;
            }
        }

        public override string ToString()
        {
            return base.ToString() + (_listaAcquisti.Count > 0 ? "\nLista acquisti:" : "") + OttieniAcquisti("\n\t");
        }

        public int CompareTo(Customer<T> other) => (int)((dynamic)SpesaTotale - other.SpesaTotale);

        public static bool operator ==(Customer<T> c1, Customer<T> c2)
        {
            if (c1 is null && c2 is null) return true;
            
            if (c1 is null || c2 is null) return false;

            return c1.CompareTo(c2) == 0;
        }
        public static bool operator !=(Customer<T> c1, Customer<T> c2) => !(c1 == c2);

        public static bool operator >(Customer<T> c1, Customer<T> c2)
        {
            if (c1 is null || c2 is null) return false;

            return c1.CompareTo(c2) > 0;
        }
        public static bool operator >=(Customer<T> c1, Customer<T> c2) => (c1 == c2) || (c1 > c2);
        public static bool operator <(Customer<T> c1, Customer<T> c2) => !(c1==c2) && !(c1 > c2);
        public static bool operator <=(Customer<T> c1, Customer<T> c2) => (c1 < c2) || (c1 == c2);
    }
}