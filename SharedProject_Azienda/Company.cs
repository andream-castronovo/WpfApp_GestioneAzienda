using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml.Linq;


namespace SharedProject_Azienda
{
    public class Company<T> where T : struct
    {
        
        private List<Employee<T>> _listaDipendenti;
        
        
        private List<Customer<T>> _listaClienti;


        public Company() 
        { 
            _listaDipendenti = new List<Employee<T>>();
            _listaClienti = new List<Customer<T>>();
        }

        #region Proprietà

        #region Liste
        public List<Employee<T>> ListaDipendenti
        {
            get => _listaDipendenti;
            set
            {
                _listaDipendenti = value;
            }
        }

        public List<Customer<T>> ListaClienti
        {
            get => _listaClienti;
            set
            {
                _listaClienti = value;
            }
        }
        #endregion

        #region Dati economici

        /// <summary>
        /// Somma degli stipendi
        /// </summary>
        public T SpeseTotali
        {
            get
            {
                T spese = default;
                foreach (Employee<T> d in _listaDipendenti)
                {
                    spese += (dynamic)d.StipendioAnnuo;
                }

                return spese;
            }
        }

        /// <summary>
        /// Somma degli acquisti dei clienti
        /// </summary>
        public T EntrateTotali 
        { 
            get 
            {
                T entrate = default;
                foreach (Customer<T> c in _listaClienti)
                {
                    foreach (Acquisto<T> a in c.ListaAcquisti)
                    {
                        entrate += (dynamic) a.Price;
                    }
                }
                return entrate;
            } 
        }

        public T ProfittoTotale
        {
            get
            {
                return (dynamic) EntrateTotali - SpeseTotali;
            }
        }
        #endregion

        #region Indicizzatori
        public Employee<T> this[int i] 
        {
            get 
            {
                return _listaDipendenti[i]; 
            }
        }

        /// <value>
        /// Ottieni i clienti con lo stesso nome e cognome
        /// </value>
        /// <param name="name">Nome del cliente</param>
        /// <param name="cognome">Cognome del cliente</param>
        /// <returns>Lista di clienti con stesso nome e cognome (lista di 1 elemento in caso di nessun doppione)</returns>
        public List<Customer<T>> this[string name, string cognome]
        {
            get 
            {
                List<Customer<T>> lst = new List<Customer<T>>();
                foreach (Customer<T> c in _listaClienti)
                {
                    if (c.Nome == name && c.Cognome == cognome)
                    {
                        lst.Add(c);
                    }
                }

                return lst;
            }
        }
        #endregion

        #endregion
    }
}
