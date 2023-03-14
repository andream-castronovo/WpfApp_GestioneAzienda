using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SharedProject_Azienda
{
    class Company<T> where T : struct
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
                // TODO: Fare controlli
                _listaDipendenti = value;
            }
        }

        public List<Customer<T>> ListaClienti
        {
            get => _listaClienti;
            set
            {
                // TODO: Fare controlli
                _listaClienti = value;
            }
        }
        #endregion

        #region Dati economici
        public T SpeseTotali
        {
            get
            {
                T spese = default;
                // TODO: Da completare
                foreach (Employee<T> d in _listaDipendenti)
                {
                    spese += (dynamic)d.StipendioAnnuo;
                }

                return spese; // HACK: Cambiare il valore restituito
            }
        }

        public T EntrateTotali 
        { 
            get 
            {
                return default(T); // HACK: Cambiare il valore restituito
            } 
        }

        public T ProfittoTotale
        {
            get
            {
                return default(T); // HACK: Cambiare il valore restituito
            }
        }
        #endregion

        #region Indicizzatori
        public Employee<T> this[int i] 
        {
            get 
            { 
                throw new NotImplementedException(); 
            }
        }

        /// <value>
        /// Ottieni i clienti con lo stesso nome e cognome
        /// </value>
        /// <param name="name">Nome del cliente</param>
        /// <param name="cognome">Cognome del cliente</param>
        /// <returns>Lista di clienti con stesso nome e cognome (lista di 1 elemento in caso di nessun doppione)</returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<Customer<T>> this[string name, string cognome]
        {
            get 
            { 
                throw new NotImplementedException();
            }
        }
        #endregion
        
        #endregion
    }
}
