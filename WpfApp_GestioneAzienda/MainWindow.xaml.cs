using SharedProject_Azienda;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Windows.Media;
using System.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.IO;

namespace WpfApp_GestioneAzienda
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // TODO: Fare parte salvataggio e caricamento con JSON
        // TODO: Fare parte dei dati dell'azienda in XAML

        public MainWindow()
        {
            InitializeComponent();
        }

        const string SAVE_FILE_PATH = @"..\..\salvataggio.json";

        Company<decimal> _azienda;
        Persona<decimal> _personaCorrente;
        bool _caricato;

        #region Eventi

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Decimal è un tipo che gestisce numeri con la virgola in modo
            // più preciso rispetto a double e float; utile per le valute.
            // Per usarlo è necessario aggiungere "m" alla fine del numero in modo
            // da differenziarlo dal double (usato di default per i numeri con la virgola in c#)

            if (!File.Exists(SAVE_FILE_PATH))
            {
                new StreamWriter(SAVE_FILE_PATH).Close();

                _azienda = new Company<decimal>();

                _azienda.ListaDipendenti.Add(
                    new Employee<decimal>("Giorgio", "Rossi", 1200.00m)
                    );
                _azienda.ListaDipendenti.Add(
                    new Employee<decimal>("Annamaria", "Grondaia", 3000.90m)
                    );

                _azienda.ListaClienti.Add(
                    new Customer<decimal>("Alberto", "Giacomini", new List<Acquisto<decimal>>() { new Acquisto<decimal>(Prodotti.Resistore, 3200m) })
                    );
                _azienda.ListaClienti.Add(
                    new Customer<decimal>("Giovanni", "Giorgio", new List<Acquisto<decimal>>() { new Acquisto<decimal>(Prodotti.Condensatore, 2400m) })
                    );
            }
            else
            {
                Carica();
            }


            foreach (string s in Enum.GetNames(typeof(Prodotti)))
                cmbListaAcquisti.Items.Add(s);
            
            _caricato = false;

            lstDipendenti.ItemsSource = _azienda.ListaDipendenti;
            lstClienti.ItemsSource = _azienda.ListaClienti;



            rdbCliente.IsChecked = true;

            Reset();
            
        }

        private void rdbPerson_Changed(object sender, RoutedEventArgs e)
        {
            OttieniRuolo();
            CaricaInterfacciaPersona();
        }

        #region Bottoni

        private void btnAggiungiAllaAzienda_Click(object sender, RoutedEventArgs e)
        {
            

            try
            {
                if (IsPlaceholder(txtNome))
                    throw new Exception("Il nome è vuoto");
                if (IsPlaceholder(txtCognome))
                    throw new Exception("Il cognome è vuoto");

                _personaCorrente.Nome = ControllaStringa(txtNome.Text, seVuota:"Il nome è vuoto");
                _personaCorrente.Cognome = ControllaStringa(txtCognome.Text, seVuota:"Il cognome è vuoto");
            }
            catch (Exception ex)
            {
                MessaggioErrore(ex.Message);
                return;
            }

            if ((bool)rdbCliente.IsChecked)
            {
                Customer<decimal> c = (Customer<decimal>)_personaCorrente;

                if (c.ListaAcquisti.Count == 0)
                {
                    MessageBoxResult risposta = MessageBox.Show(
                        "Sei sicuro di voler aggiungere un cliente senza acquisti?\n(Potrai aggiungere acquisti anche dopo averlo aggiunto)",
                        "Conferma aggiunta",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question
                        );

                    if (risposta == MessageBoxResult.No)
                    {
                        MessageBox.Show(
                            "Cliente non aggiunto.",
                            "Cliente non aggiunto",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                            );
                        return;
                    }
                }

                _azienda.ListaClienti.Add(c);
                _personaCorrente = new Customer<decimal>();
            }
            else if ((bool)rdbImpiegato.IsChecked)
            {
                Employee<decimal> emp = (Employee<decimal>)_personaCorrente;
                try
                {
                    if (IsPlaceholder(txtStipendio))
                        throw new Exception("Lo stipendio è vuoto");

                    emp.StipendioAnnuo = decimal.Parse(ControllaStringa(txtStipendio.Text, seVuota:"Lo stipendio è vuoto"));
                }
                catch (Exception ex)
                {
                    MessaggioErrore(ex.Message);
                    return;
                }

                _azienda.ListaDipendenti.Add(emp);
                _personaCorrente = new Employee<decimal>();
            }

            lstDipendenti.Items.Refresh();
            lstClienti.Items.Refresh();
            Reset();
        }
        private void btnAggiungiAcquisto_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                
                Customer<decimal> c = (Customer<decimal>)_personaCorrente;
                
                if (lstAcquisti.ItemsSource == null)
                    lstAcquisti.ItemsSource = c.ListaAcquisti;
                
                if (IsPlaceholder(txtPrezzoAcquisto))
                    throw new Exception("Il prezzo è vuoto");

                c.ListaAcquisti.Add(
                    new Acquisto<decimal>(
                        (Prodotti)OttieniNumByNome(
                            ControllaStringa((string)cmbListaAcquisti.SelectedItem, "Seleziona qualcosa dalla lista di prodotti acquistabili!"),
                            typeof(Prodotti)
                        ),
                        decimal.Parse(
                            ControllaStringa(txtPrezzoAcquisto.Text, seVuota: "Il prezzo è vuoto")
                        )
                    )
                );

                lstAcquisti.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessaggioErrore(ex.Message);
            }
        }
        private void btnModifica_Click(object sender, RoutedEventArgs e)
        {
            ModalitaModifica(true);
            AccessoInput(true);
        }
        private void btnConfermaModifica_Click(object sender, RoutedEventArgs e)
        {
            Type tipoPersona = _personaCorrente.GetType();

            try
            {
                if (IsPlaceholder(txtNome))
                    throw new Exception("Il nome è vuoto");
                if (IsPlaceholder(txtCognome))
                    throw new Exception("Il cognome è vuoto");

                _personaCorrente.Nome = ControllaStringa(txtNome.Text, seVuota: "Il nome è vuoto");
                _personaCorrente.Cognome = ControllaStringa(txtCognome.Text, seVuota: "Il cognome è vuoto");
            }
            catch (Exception ex)
            {
                MessaggioErrore(ex.Message);
                return;
            }

            if (tipoPersona == typeof(Employee<decimal>))
            {
                Employee<decimal> emp = (Employee<decimal>)_personaCorrente;

                if (IsPlaceholder(txtStipendio))
                    throw new Exception("Lo stipendio è vuoto");

                try
                {
                    emp.StipendioAnnuo = decimal.Parse(ControllaStringa(txtStipendio.Text, seVuota:"Lo stipendio è vuoto"));
                }
                catch (Exception ex)
                {
                    MessaggioErrore(ex.Message);
                    return;
                }

                if (lstDipendenti.SelectedIndex == -1) // Presumo di star cambiando un cliente in dipendente
                {
                    int tmp = lstClienti.SelectedIndex;
                    _azienda.ListaClienti.RemoveAt(tmp);
                    _azienda.ListaDipendenti.Add(emp);
                    lstClienti.Items.Refresh();
                    lstDipendenti.Items.Refresh();
                    lstDipendenti.SelectedIndex = lstDipendenti.Items.Count - 1;
                }
                else
                {
                    int tmp = lstDipendenti.SelectedIndex;
                    _azienda.ListaDipendenti[tmp] = emp;
                    lstDipendenti.Items.Refresh();
                    lstDipendenti.SelectedIndex = tmp;
                }
            }
            else if (tipoPersona == typeof(Customer<decimal>))
            {
                Customer<decimal> c = (Customer<decimal>)_personaCorrente;

                if (c.ListaAcquisti.Count == 0)
                {
                    MessageBoxResult risposta = MessageBox.Show(
                        "Sei sicuro di voler modificare un cliente lasciandolo senza acquisti?",
                        "Conferma modifica",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question
                        );

                    if (risposta == MessageBoxResult.No)
                    {
                        MessageBox.Show(
                            "Cliente non modificato.",
                            "Cliente non modificato",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                            );
                        return;
                    }
                }

                if (lstClienti.SelectedIndex == -1) // Presumo di star cambiando un dipendente in cliente
                {
                    int tmp = lstDipendenti.SelectedIndex;
                    _azienda.ListaDipendenti.RemoveAt(tmp);
                    _azienda.ListaClienti.Add(c);
                    lstClienti.Items.Refresh();
                    lstDipendenti.Items.Refresh();
                    lstClienti.SelectedIndex = lstClienti.Items.Count - 1;
                }
                else
                {
                    int tmp = lstClienti.SelectedIndex;
                    _azienda.ListaClienti[tmp] = c;
                    lstClienti.Items.Refresh();
                    lstDipendenti.Items.Refresh();
                    lstClienti.SelectedIndex = tmp;
                }

            }

            
            ModalitaModifica(false);
            AccessoInput(false);

            
        }

        #endregion

        #region List Box

        private void lstClienti_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (lstClienti.SelectedIndex == -1)
            {
                Reset();
                ModalitaModifica(false);
                return;
            }

            lstDipendenti.SelectedIndex = -1;

            btnModifica.IsEnabled = true;

            Customer<decimal> c = _azienda.ListaClienti[lstClienti.SelectedIndex];
            _personaCorrente = c;

            CaricaDati(c);
            ModalitaModifica(false);
            AccessoInput(false);
        }
        private void lstDipendenti_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstDipendenti.SelectedIndex == -1)
            {
                Reset();
                ModalitaModifica(false);
                return;
            }

            lstClienti.SelectedIndex = -1;

            btnModifica.IsEnabled = true;

            Employee<decimal> emp = _azienda.ListaDipendenti[lstDipendenti.SelectedIndex];
            _personaCorrente = emp;

            CaricaDati(emp);
            ModalitaModifica(false);
            AccessoInput(false);
        }

        #endregion

        #endregion

        private void OttieniRuolo()
        {
            if ((bool)rdbCliente.IsChecked)
            {
                _personaCorrente = new Customer<decimal>();
            }
            else if ((bool)rdbImpiegato.IsChecked)
            {
                _personaCorrente = new Employee<decimal>();
            }
            else
                throw new Exception("Non è stata effettuata la scelta tra cliente e dipendente!");
        }

        private void CaricaInterfacciaPersona()
        {
            if (_personaCorrente.GetType() == typeof(Customer<decimal>))
            {
                grpImpiegati.Visibility = Visibility.Collapsed;
                grpClienti.Visibility = Visibility.Visible;

                btnAggiungiAllaAzienda.Content = "Aggiungi Cliente";
            }
            else if (_personaCorrente.GetType() == typeof(Employee<decimal>))
            {
                grpImpiegati.Visibility = Visibility.Visible;
                grpClienti.Visibility = Visibility.Collapsed;
                lstAcquisti.SelectedIndex = -1;
                cmbListaAcquisti.SelectedIndex = -1;
                btnAggiungiAllaAzienda.Content = "Aggiungi Dipendente";
            }
        }

        private void MessaggioErrore(string messaggio)
        {
            MessageBox.Show("Si è verificato un errore:\n\t" + messaggio, "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private string ControllaStringa(string s, string seNull="La stringa non deve essere null!", string seVuota="Devi scrivere qualcosa!")
        {
            if (s == null)
                throw new Exception(seNull);
            if (s == "")
                throw new Exception(seVuota);
            return s;
        }

        private int OttieniNumByNome(string Name, Type t)
        {
            string[] all = Enum.GetNames(t);

            for (int i = 0; i < all.Length; i++)
            {
                if (Name == all[i])
                    return i;
            }
            return -1;
        }

        private bool IsPlaceholder(TextBox txt)
        {
            if ((txt.Tag as string).Split('#')[1] == "active")
                return true;
            else
                return false;
        }

        private void CaricaDati(Persona<decimal> p)
        {
            RimuoviPlaceholder(txtNome);
            RimuoviPlaceholder(txtCognome);
            txtNome.Text = p.Nome;
            txtCognome.Text = p.Cognome;

            if (p.GetType() == typeof(Customer<decimal>))
            {
                rdbCliente.IsChecked = true;
                lstAcquisti.ItemsSource = ((Customer<decimal>)p).ListaAcquisti;
            }
            else if (p.GetType() == typeof(Employee<decimal>))
            {
                Employee<decimal> emp = (Employee<decimal>)p;
                rdbImpiegato.IsChecked = true;
                RimuoviPlaceholder(txtStipendio);
                txtStipendio.Text = emp.StipendioAnnuo + "";
            }
        }

        private void Reset()
        {
            txtNome.Text = "";
            txtCognome.Text = "";
            txtStipendio.Text = "";
            txtPrezzoAcquisto.Text = "";

            cmbListaAcquisti.SelectedIndex = -1;


            lstAcquisti.ItemsSource = null;

            btnModifica.IsEnabled = false;
            
            ImpostaPlaceholder(txtNome, "Inserisci nome");
            ImpostaPlaceholder(txtCognome, "Inserisci cognome");
            ImpostaPlaceholder(txtStipendio, "Inserisci stipendio annuo");
            ImpostaPlaceholder(txtPrezzoAcquisto, "$$$$");

            OttieniRuolo();
            AccessoInput(true);
        }

        private void AccessoInput(bool mod)
        {
            txtNome.IsEnabled = mod;
            txtCognome.IsEnabled = mod;
            txtStipendio.IsEnabled = mod;
            txtPrezzoAcquisto.IsEnabled = mod;
            txtStipendio.IsEnabled = mod;
            btnAggiungiAcquisto.IsEnabled = mod;
            cmbListaAcquisti.IsEnabled = mod;
            lstAcquisti.IsEnabled = mod;
            rdbCliente.IsEnabled = mod;
            rdbImpiegato.IsEnabled = mod;
        }

        private void ModalitaModifica(bool mod)
        {
            if (mod)
            {
                btnModifica.Visibility = Visibility.Collapsed;
                btnConfermaModifica.Visibility = Visibility.Visible;
            }
            else
            {
                btnModifica.Visibility = Visibility.Visible;
                btnConfermaModifica.Visibility = Visibility.Collapsed;

                btnAggiungiAllaAzienda.Visibility = Visibility.Collapsed;
                btnNuovaPersona.Visibility = Visibility.Visible;
            }
        }

        private void btnNuovaPersona_Click(object sender, RoutedEventArgs e)
        {
            // Farà da solo il metodo Reset
            lstDipendenti.SelectedIndex = -1;
            lstClienti.SelectedIndex = -1;
            ModalitaModifica(false);

            btnAggiungiAllaAzienda.Visibility = Visibility.Visible;
            btnNuovaPersona.Visibility = Visibility.Collapsed;


        }

        private void ImpostaPlaceholder(TextBox txt, string placeholder)
        {
            if (txt.Text == "")
            {
                txt.Tag = $"{placeholder}#active";
                txt.Text = placeholder;
                
                txt.Foreground = Brushes.Gray;
                txt.FontStyle = FontStyles.Italic;
            }
        }
        private void RimuoviPlaceholder(TextBox txt)
        {
            if ((txt.Tag as string).Split('#')[1] == "active")
            {
                txt.Text = "";
                txt.Foreground = Brushes.Black;
                txt.FontStyle = FontStyles.Normal;
                txt.Tag = (txt.Tag as string).Split('#')[0] + "#inactive";
            }
        }
   
        private void txtBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            RimuoviPlaceholder(txt);
        }
        private void txtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            ImpostaPlaceholder(txt, (txt.Tag as string).Split('#')[0]);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult mgb = MessageBox.Show("Salvare prima di uscire?","Proposta salvataggio",MessageBoxButton.YesNoCancel,MessageBoxImage.Question);
            if (mgb == MessageBoxResult.Yes)
            {
                Salva();
            }
            else if (mgb != MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void Salva()
        {
            JsonSerializer js = new JsonSerializer();
            js.Formatting = Formatting.Indented;
            using (StreamWriter sw = new StreamWriter(SAVE_FILE_PATH))
            {
                js.Serialize(sw, _azienda);
            }
        }
        private void Carica()
        {
            JsonSerializer js = new JsonSerializer();
            js.Formatting = Formatting.Indented;
            using (StreamReader sr = File.OpenText(SAVE_FILE_PATH))
            {
                using (JsonReader jsonReader = new JsonTextReader(sr))
                    _azienda = (Company<decimal>)js.Deserialize(jsonReader, typeof(Company<decimal>));
            }
        }
    }
}
