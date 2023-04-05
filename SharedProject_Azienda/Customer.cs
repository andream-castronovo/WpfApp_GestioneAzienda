﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SharedProject_Azienda
{
    public class Customer<T> : Persona<T>, IComparable<Customer<T>> where T : struct
    {
        public override T GetEconomicValue() => SpesaTotale;

        List<Acquisto<T>> _listaAcquisti;

        public Customer() : base ()
        {
            _listaAcquisti = new List<Acquisto<T>>();
        }

        public Customer(string name, string surname) : this(name, surname, new List<Acquisto<T>>())
        {}

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
                    spesa += (dynamic) a.Price;
                return spesa;
            }
        }

        public override string ToString()
        {
            return base.ToString() + (_listaAcquisti.Count > 0 ? "\nLista acquisti:" : "") + OttieniAcquisti("\n\t");
        }

        public int CompareTo(Customer<T> other) => (int)((dynamic)SpesaTotale - other.SpesaTotale);
    }
}