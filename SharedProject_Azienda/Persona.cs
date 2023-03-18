using System;
using System.Collections.Generic;
using Newtonsoft.Json;

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

        #region Proprietà
        [JsonProperty]
        public Guid ID 
        {
            get => _id;
        }
        
        public string Nome
        {
            get => _nome;
            set
            {
                // TODO: Fare controlli
                _nome = value;
            }
        }

        public string Cognome
        {
            get => _cognome;
            set
            {
                // TODO: Fare controlli
                _cognome = value;
            }
        }
        #endregion
        
        private static Guid GeneraGUID()
        {
            Guid id = Guid.NewGuid();
            while (_allIds.Contains(id))
                id = Guid.NewGuid();
            
            return id;
        }

        static public void CaricaID(List<Persona<T>> lst)
        {
            lst = lst ?? new List<Persona<T>>();

            foreach (Persona<T> p in lst) 
            { 
                _allIds.Add(p.ID);
            }
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
            _allIds.Add(id);
        }
        #endregion

    }
}
