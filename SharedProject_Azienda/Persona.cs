using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedProject_Azienda
{
    abstract class Persona
    {
        string _nome;
        string _cognome;
        Guid _id;

        static List<Guid> _allIds = new List<Guid>();

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

        private Guid GeneraGUID()
        {
            Guid id = Guid.NewGuid();
            bool valido = true;
            do
            {
                if (_allIds.Contains(id))
                    valido = false;
            } while (!valido);
            return id;
        }
    }
}
