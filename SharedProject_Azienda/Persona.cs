using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading;
using System.Runtime.CompilerServices;

namespace SharedProject_Azienda
{
    abstract class Persona<T> where T : struct
    {
        // Non tutto il contenuto è abstract in quanto nelle classi derivate servono,
        // non mettendole abstract mi risparmio di riscrivere tutti i codici comuni.

        #region CDC
        string _nome;
        string _cognome;

        Guid _id;
        #endregion

        private static List<Guid> _allIds = new List<Guid>();

        public static void CaricaIDs(List<Persona<T>> lst)
        {
            foreach (Persona<T> p in lst)
            {
                _allIds.Add(p.ID);
            }
        }

        #region Proprietà
        
        public Guid ID 
        {
            get => _id;
            set // Serve per JSON
            {
                _allIds.Remove(_id);
                _id = value;
                _allIds.Add(value);
            }
        }
        
        private void ControllaStringa(string s, string seNull= "La stringa è null!", string seVuota="La stringa è vuota!")
        {
            if (s == null)
                throw new Exception(seNull);
            if (s == "")
                throw new Exception(seVuota);
        }

        public string Nome
        {
            get => _nome;
            set
            {
                ControllaStringa(value, "Il nome non può essere null!", "Il nome non puo essere vuoto");
                _nome = value;
            }
        }

        public string Cognome
        {
            get => _cognome;
            set
            {
                ControllaStringa(value, "Il cognome non può essere null!", "Il cognome non puo essere vuoto");
                _cognome = value;
            }
        }
        #endregion
        
        public static Guid GeneraGUID()
        {
            Guid id = Guid.NewGuid();
            while (_allIds.Contains(id))
                id = Guid.NewGuid();
            return id;
        }

        static public void RimuoviID(Persona<T> p)
        {
            if (p != null)
                _allIds.Remove(p.ID);
        }

        public abstract T GetEconomicValue();

        public override string ToString()
        {
            return $"Nominativo:\n\t{_nome} {_cognome}";
        }

        #region Costruttori
        public Persona() : this ("<no_name>","<no_surname>")
        { }
        public Persona(string nome, string cognome)
        {
            Guid id = GeneraGUID();

            _nome = nome;
            _cognome = cognome;

            _id = id;

            if (nome != "<no_name>" && cognome != "<no_surname>")
                _allIds.Add(id);
        }
        #endregion

    }
}
