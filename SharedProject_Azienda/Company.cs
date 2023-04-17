using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Collections;
using Newtonsoft.Json;
using System.ComponentModel;

#pragma warning disable CS0660 // Warning disabilito perché mi consiglia di eseguire override di metodi di object (Per override degli operatori)
#pragma warning disable CS0661 // Warning disabilito perché mi consiglia di eseguire override di metodi di object (Per override degli operatori)

namespace SharedProject_Azienda
{
    [JsonObject] // Decorator che serve a dire al serializzatore di NON trattare l'oggetto come una lista, vista la presenza di IEnumerable.
                 // Fonte: https://github.com/JamesNK/Newtonsoft.Json/issues/2121
    [XmlRoot]
    public class Company<T> : IComparable<Company<T>>, IEnumerable<Persona<T>> where T : struct
    {
        // Programmato da Andrea Maria Castronovo - 4°I - Data: 17/04/2023
        
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

        /// <summary>
        /// Compara un'azienda ad un'altra in base al profitto.
        /// </summary>
        /// <param name="other">Azienda da comparare con la nostra.</param>
        /// <returns>Un numero negativo se l'oggetto corrente è minore dell'altro; 0 se i due oggetti sono uguali e un numero positivo se l'oggetto corrente è maggiore</returns>
        /// <exception cref="NotImplementedException"></exception>
        public int CompareTo(Company<T> other)
        {
            if (other == null) 
                return 1;

            return (dynamic)ProfittoTotale - other.ProfittoTotale;
        }


        
        public IEnumerator<Persona<T>> GetEnumerator()
        {
            for (int i = 0; i < _listaClienti.Count; i++)
                yield return _listaClienti[i];

            for (int i = 0; i < _listaDipendenti.Count; i++)
                yield return _listaDipendenti[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #region Operatori

        #region Uguale e diverso
        /// <summary>
        /// Restituisce true se le due aziende hanno lo stesso profitto; anche se sono entrambe null.
        /// </summary>
        /// <param name="A">Azienda 1</param>
        /// <param name="B">Azienda 2</param>
        /// <returns>true se le due aziende hanno lo stesso profitto; anche se sono entrambe null. false se le due aziende sono diverse</returns>
        public static bool operator ==(Company<T> A, Company<T> B)
        {
            if (A is null && B is null) return true;

            if (A is null || B is null) return false;

            return A.CompareTo(B) == 0;
        }

        public static bool operator !=(Company<T> A, Company<T> B) => !(A == B);
        #endregion

        

        #region Maggiore, minore e famiglia
        public static bool operator >(Company<T> A, Company<T> B)
        {
            if (A is null || B is null)
                return false;

            if (A.CompareTo(B) > 0)
                return true;
            return false;
        }

        public static bool operator <(Company<T> A, Company<T> B) => !(A == B) && !(A > B);
        public static bool operator >=(Company<T> A, Company<T> B) => (A > B) || (A == B);
        public static bool operator <=(Company<T> A, Company<T> B) => (A < B) || (A == B);
        #endregion

        #endregion


    }
}
