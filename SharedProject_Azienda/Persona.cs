using System;
using System.Collections.Generic;
using System.Text;

namespace SharedProject_Azienda
{
    abstract class Persona
    {
        string _nome;
        string _cognome;

        public Guid ID { get; set; }
        
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

        public Persona()
        {
            _nome = "<no_name>"
            _cognome = "<no_surname>"
        }
    }
}
