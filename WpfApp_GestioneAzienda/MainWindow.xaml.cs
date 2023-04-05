using SharedProject_Azienda;
using SharedProject_Azienda.SortHelper;

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Windows.Media;
using System.Linq;
using System.IO;
using System.Xml.Serialization;

using Newtonsoft.Json;

namespace WpfApp_GestioneAzienda
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        const string SAVE_FILE_PATH_JSON = @"..\..\salvataggioJson.json";
        const string SAVE_FILE_PATH_XML = @"..\..\salvataggioJson.xml";
        const string SAVE_FILE_PATH_CSV = @"..\..\salvataggioJson.csv";

        bool _autosalvataggio;

        Company<decimal> _azienda;
        List<Acquisto<decimal>> _acquistiCorrenti;
        bool _toSave;
        
        #region Eventi

        #region Window

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Decimal è un tipo che gestisce numeri con la virgola in modo
            // più preciso rispetto a double e float; utile per le valute.
            // Per usarlo è necessario aggiungere "m" alla fine del numero in modo
            // da differenziarlo dal double (usato di default per i numeri con la virgola in c#)
            
            if (!File.Exists(SAVE_FILE_PATH_JSON))
            {
                new StreamWriter(SAVE_FILE_PATH_JSON).Close();

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
                _toSave = true;
            }
            else
            {
                CaricaJson();
                _toSave = false;
            }


            foreach (string s in Enum.GetNames(typeof(Prodotti)))
                cmbListaAcquisti.Items.Add(s);
            
            // ItemsSource serve per collegare una ListBox ad una collezione, per poi essere aggiornata in automatico;
            // Per visualizzare gli aggiornamenti lst.Items.Refresh()

            lstDipendenti.ItemsSource = _azienda.ListaDipendenti;
            lstClienti.ItemsSource = _azienda.ListaClienti;



            rdbCliente.IsChecked = true;

            Reset();

            _acquistiCorrenti = new List<Acquisto<decimal>>();
            lstAcquisti.ItemsSource = _acquistiCorrenti;
            
         }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_toSave)
            {
                MessageBoxResult mgb = MessageBox.Show("Salvare prima di uscire?", "Proposta salvataggio", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (mgb == MessageBoxResult.Yes)
                {
                    SalvaJson(); // Salvo 
                }
                else if (mgb == MessageBoxResult.Cancel)
                {
                    e.Cancel = true; // Non chiudo
                }
                // non serve condizione NO perché il programma si chiude senza salvare
            }
        }

        #endregion

        #region Radio Button
        
        private void rdbPerson_Changed(object sender, RoutedEventArgs e)
        {
            CaricaInterfacciaPersona();
        }

        #endregion

        #region Bottoni

        private void btnAggiungiAllaAzienda_Click(object sender, RoutedEventArgs e)
        {
            string nome;
            string cognome;

            try
            {
                if (IsPlaceholder(txtNome)) // per controllo placeholder
                    throw new Exception("Il nome è vuoto");
                if (IsPlaceholder(txtCognome))
                    throw new Exception("Il cognome è vuoto");

                nome = ControllaStringa(txtNome.Text, seVuota:"Il nome è vuoto");
                cognome = ControllaStringa(txtCognome.Text, seVuota:"Il cognome è vuoto");
            }
            catch (Exception ex)
            {
                MessaggioErrore(ex.Message);
                return;
            }

            if ((bool)rdbCliente.IsChecked)
            {
                if (_acquistiCorrenti.Count == 0)
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

                _azienda.ListaClienti.Add(
                    new Customer<decimal>(
                        nome,
                        cognome,
                        _acquistiCorrenti
                        )
                    );
                _acquistiCorrenti = new List<Acquisto<decimal>>();
            }
            else if ((bool)rdbImpiegato.IsChecked)
            {
                decimal stipendio;
                try
                {
                    if (IsPlaceholder(txtStipendio))
                        throw new Exception("Lo stipendio è vuoto");

                    stipendio = decimal.Parse(ControllaStringa(txtStipendio.Text, seVuota:"Lo stipendio è vuoto"));
                }
                catch (Exception ex)
                {
                    MessaggioErrore(ex.Message);
                    return;
                }

                _azienda.ListaDipendenti.Add(
                    new Employee<decimal>(
                        nome,
                        cognome,
                        stipendio
                        )
                    );
                
            }

            lstDipendenti.Items.Refresh();
            lstClienti.Items.Refresh();
            
            Reset();
            if (_autosalvataggio)
                SalvaJson();
            else
                _toSave = true;
        }

        /// <summary>
        ///  Per aggiungere acquisti
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAggiungiAcquisto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstAcquisti.ItemsSource == null)
                    lstAcquisti.ItemsSource = _acquistiCorrenti;
                
                if (IsPlaceholder(txtPrezzoAcquisto))
                    throw new Exception("Il prezzo è vuoto");

                _acquistiCorrenti.Add(
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
            string nome;
            string cognome;
            try
            {
                if (IsPlaceholder(txtNome))
                    throw new Exception("Il nome è vuoto");
                if (IsPlaceholder(txtCognome))
                    throw new Exception("Il cognome è vuoto");

                nome = ControllaStringa(txtNome.Text, seVuota: "Il nome è vuoto");
                cognome = ControllaStringa(txtCognome.Text, seVuota: "Il cognome è vuoto");
            }
            catch (Exception ex)
            {
                MessaggioErrore(ex.Message);
                return;
            }

            if ((bool)rdbImpiegato.IsChecked)
            {
                decimal stipendio;

                
                try
                {
                    if (IsPlaceholder(txtStipendio))
                        throw new Exception("Lo stipendio è vuoto");
                    stipendio = decimal.Parse(ControllaStringa(txtStipendio.Text, seVuota:"Lo stipendio è vuoto"));
                }
                catch (Exception ex)
                {
                    MessaggioErrore(ex.Message);
                    return;
                }


                if (lstDipendenti.SelectedIndex == -1) // Presumo di star cambiando un cliente in dipendente
                {
                    Employee<decimal> emp = new Employee<decimal>(
                        nome,
                        cognome,
                        stipendio
                    );
                    Persona<decimal>.RimuoviID(emp); // Tolgo l'ID del nuovo dipendente visto che sarà lo stesso del cliente

                    int tmp = lstClienti.SelectedIndex; // Salvo l'indice del cliente da cambiare

                    Guid id = _azienda.ListaClienti[tmp].ID; // Salvo il suo ID per trasferirlo
                    Persona<decimal>.RimuoviID(_azienda.ListaClienti[tmp]);

                    _azienda.ListaClienti.RemoveAt(tmp); // Rimuovo il cliente

                    emp.ID = id; // Gli imposto l'ID del cliente rimosso

                    _azienda.ListaDipendenti.Add(emp); // Aggiungo il dipendente

                    lstClienti.Items.Refresh();
                    lstDipendenti.Items.Refresh();

                    lstDipendenti.SelectedIndex = lstDipendenti.Items.Count - 1;
                }
                else
                {
                    int tmp = lstDipendenti.SelectedIndex;
                    _azienda.ListaDipendenti[tmp].Nome = nome;
                    _azienda.ListaDipendenti[tmp].Cognome = cognome;
                    _azienda.ListaDipendenti[tmp].StipendioAnnuo = stipendio;
                    lstDipendenti.Items.Refresh();
                    lstDipendenti.SelectedIndex = tmp;
                }
            }
            else if ((bool) rdbCliente.IsChecked)
            {
                if (_acquistiCorrenti.Count == 0)
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
                    Customer<decimal> c = new Customer<decimal>(
                        nome,
                        cognome,
                        _acquistiCorrenti
                    );
                    Persona<decimal>.RimuoviID(c);

                    int tmp = lstDipendenti.SelectedIndex;

                    Guid id = _azienda.ListaDipendenti[tmp].ID;
                    Persona<decimal>.RimuoviID(_azienda.ListaDipendenti[tmp]);

                    _azienda.ListaDipendenti.RemoveAt(tmp);
                    c.ID = id;

                    _azienda.ListaClienti.Add(c);

                    lstClienti.Items.Refresh();
                    lstDipendenti.Items.Refresh();
                    lstAcquisti.Items.Refresh();

                    lstClienti.SelectedIndex = lstClienti.Items.Count - 1;
                }
                else
                {
                    int tmp = lstClienti.SelectedIndex;
                    _azienda.ListaClienti[tmp].Nome = nome;
                    _azienda.ListaClienti[tmp].Cognome = cognome;
                    _azienda.ListaClienti[tmp].ListaAcquisti = _acquistiCorrenti;
                    lstClienti.Items.Refresh();
                    lstDipendenti.Items.Refresh();
                    lstAcquisti.Items.Refresh();
                    lstClienti.SelectedIndex = tmp;
                }
                _acquistiCorrenti = new List<Acquisto<decimal>>();

            }

            
            ModalitaModifica(false);
            AccessoInput(false);
            
            if (_autosalvataggio)
                SalvaJson();
            else
                _toSave = true;
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
        private void btnRimuoviPersona_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbr = MessageBox.Show(
                $"Sei sicuro di voler rimuovere \"{txtNome.Text} {txtCognome.Text}\"?",
                "Conferma rimozione",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (mbr == MessageBoxResult.No)
            {
                MessageBox.Show("Rimozione annullata.", "Rimozione annullata.",MessageBoxButton.OK,MessageBoxImage.Information);
                return;
            }

            
            if ((bool)rdbCliente.IsChecked)
            {
                Persona<decimal>.RimuoviID(_azienda.ListaClienti[lstClienti.SelectedIndex]);
                _azienda.ListaClienti.RemoveAt(lstClienti.SelectedIndex);
                _acquistiCorrenti = new List<Acquisto<decimal>>();
                lstClienti.Items.Refresh();
            }
            else if ((bool)rdbImpiegato.IsChecked)
            { 
                Persona<decimal>.RimuoviID(_azienda.ListaDipendenti[lstDipendenti.SelectedIndex]);
                _azienda.ListaDipendenti.RemoveAt(lstDipendenti.SelectedIndex);
                lstDipendenti.Items.Refresh();
            }

            if (_autosalvataggio)
                SalvaJson();
            else
                _toSave = true;
        }


        #endregion

        #region List Box

        private bool PersonaInAzienda(Persona<decimal> p)
        {
            if (_azienda.ListaClienti.Contains(p) || _azienda.ListaDipendenti.Contains(p))
                return true;
            return false;
        }

        private void lstClienti_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (lstClienti.SelectedIndex == -1)
            {
                ModalitaModifica(false);
                Reset();
                return;
            }

            lstDipendenti.SelectedIndex = -1;

            btnModifica.IsEnabled = true;
            btnRimuovi.IsEnabled = true;
            
            Customer<decimal> c = _azienda.ListaClienti[lstClienti.SelectedIndex];
            CaricaDati(
                c.GetType(), 
                c.Nome, 
                c.Cognome, 
                acquisti: c.ListaAcquisti
            );
            
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
            btnRimuovi.IsEnabled = true;

            Employee<decimal> emp = _azienda.ListaDipendenti[lstDipendenti.SelectedIndex];
            CaricaDati(
                emp.GetType(),
                emp.Nome,
                emp.Cognome,
                stipendio: emp.StipendioAnnuo
            );
            
            ModalitaModifica(false);
            AccessoInput(false);
        }

        #endregion

        #region Text Box

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
        
        #endregion
        
        #endregion

        /// <summary>
        /// Carica l'interfaccia per la persona (Vedere CaricaDati per caricare i dati)
        /// </summary>
        private void CaricaInterfacciaPersona()
        {
            if ((bool) rdbCliente.IsChecked)
            {
                grpImpiegati.Visibility = Visibility.Collapsed;
                grpClienti.Visibility = Visibility.Visible;

                btnAggiungiAllaAzienda.Content = "Aggiungi Cliente";
            }
            else if ((bool) rdbImpiegato.IsChecked)
            {
                grpImpiegati.Visibility = Visibility.Visible;
                grpClienti.Visibility = Visibility.Collapsed;
                lstAcquisti.SelectedIndex = -1;
                cmbListaAcquisti.SelectedIndex = -1;
                btnAggiungiAllaAzienda.Content = "Aggiungi Dipendente";
            }
        }

        /// <summary>
        /// Visualizza a schermo un messaggio con grafica da errore
        /// </summary>
        /// <param name="messaggio">Messaggio da visualizzare</param>
        private void MessaggioErrore(string messaggio)
        {
            MessageBox.Show("Si è verificato un errore:\n\t" + messaggio, "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Controlla che la stringa sia valida
        /// </summary>
        /// <param name="s">Stringa da controllare</param>
        /// <param name="seNull">Messaggio errore in caso la stringa sia null</param>
        /// <param name="seVuota">Messaggio errore in caso la stringa sia vuota</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private string ControllaStringa(string s, string seNull="La stringa non deve essere null!", string seVuota="Devi scrivere qualcosa!")
        {
            if (s == null)
                throw new Exception(seNull);
            if (s == "")
                throw new Exception(seVuota);
            return s;
        }

        /// <summary>
        /// Converti il nome in numero di enumerazione
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="t"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Controlla se una TextBox è un placeholder
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        private bool IsPlaceholder(TextBox txt)
        {
            if ((txt.Tag as string).Split('#')[1] == "active")
                return true;
            else
                return false;
        }

        /// <summary>
        /// Carica i dati della persona nell'interfaccia
        /// </summary>
        /// <param name="tipo">Tipo della persona da caricare</param>
        /// <param name="nome">Nome della persona da caricare</param>
        /// <param name="cognome">Cognome della persona da caricare</param>
        /// <param name="acquisti">ListaAcquisti (null se non presente) della persona da caricare</param>
        /// <param name="stipendio">Stipendio (-1 se non presente) della persona da caricare</param>
        /// <exception cref="Exception"></exception>
        private void CaricaDati(Type tipo, string nome, string cognome, List<Acquisto<decimal>> acquisti = null, decimal stipendio = -1)
        {
            RimuoviPlaceholder(txtNome);
            RimuoviPlaceholder(txtCognome);
            txtNome.Text = nome;
            txtCognome.Text = cognome;

            if (tipo == typeof(Customer<decimal>))
            {
                rdbCliente.IsChecked = true;
                _acquistiCorrenti = acquisti;
                lstAcquisti.ItemsSource = acquisti;
                lstAcquisti.Items.Refresh();
            }
            else if (tipo == typeof(Employee<decimal>))
            {
                if (stipendio < 0)
                    throw new Exception("Stipendio non valido");

                rdbImpiegato.IsChecked = true;
                RimuoviPlaceholder(txtStipendio);
                txtStipendio.Text = stipendio + "";
            }
        }
        /// <summary>
        /// Reimposta la finestra alle impostazioni iniziali
        /// </summary>
        private void Reset()
        {
            txtNome.Text = "";
            txtCognome.Text = "";
            txtStipendio.Text = "";
            txtPrezzoAcquisto.Text = "";

            cmbListaAcquisti.SelectedIndex = -1;


            lstAcquisti.ItemsSource = null;
            _acquistiCorrenti = new List<Acquisto<decimal>>();

            btnModifica.IsEnabled = false;
            btnRimuovi.IsEnabled = false;

            btnAggiungiAllaAzienda.Visibility = Visibility.Visible;
            btnNuovaPersona.Visibility = Visibility.Collapsed;
            
            btnRimuoviAcquisto.IsEnabled = false;


            ImpostaPlaceholder(txtNome, "Inserisci nome");
            ImpostaPlaceholder(txtCognome, "Inserisci cognome");
            ImpostaPlaceholder(txtStipendio, "Inserisci stipendio annuo");
            ImpostaPlaceholder(txtPrezzoAcquisto, "$$$$");

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
   
        
        #region JSON
        
        private void SalvaJson()
        {
            JsonSerializer js = new JsonSerializer();
            js.Formatting = Formatting.Indented;
            using (StreamWriter sw = new StreamWriter(SAVE_FILE_PATH_JSON))
            {
                js.Serialize(sw, _azienda);
            }
        }
        private void CaricaJson()
        {
            JsonSerializer js = new JsonSerializer
            {
                Formatting = Formatting.Indented
            };
            using (StreamReader sr = File.OpenText(SAVE_FILE_PATH_JSON))
            {
                using (JsonReader jsonReader = new JsonTextReader(sr))
                    _azienda = (Company<decimal>)js.Deserialize(jsonReader, typeof(Company<decimal>));
                
            }
            if (_azienda == null)
                _azienda = new Company<decimal>();
        }
        #endregion

        #region XML

        private void SalvaXML()
        {
            //TODO: Salvataggio XML Non funziona: 

            /*
                Eccezione non gestita di tipo 'System.InvalidOperationException' in System.Xml.dll
                Per essere XML serializzabili, i tipi che ereditano da IEnumerable devono avere un'implementazione di Add(System.Object) a 
                tutti i livelli della gerarchia di ereditarietà. 

                SharedProject_Azienda.Company`1[[System.Decimal, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]] 
            
                non implementa Add(System.Object).
            
            */
            XmlSerializer xmlSer = new XmlSerializer(typeof(Company<decimal>));


            using (StreamWriter myWriter = new StreamWriter(SAVE_FILE_PATH_XML))
                xmlSer.Serialize(myWriter, _azienda); // Scrive nel nostro file "myWriter" l'oggetto serializzato "myObject"
            
        }

        private void CaricaXML()
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(Company<decimal>));

            using (StreamReader myReader = new StreamReader(SAVE_FILE_PATH_XML))
                _azienda = (Company<decimal>)mySerializer.Deserialize(myReader);
            
            
            
        }


        #endregion

        #region CSV

        private void SalvaCSV()
        {
            throw new NotImplementedException();
        }

        private void CaricaCSV()
        {
            throw new NotImplementedException();
        }

        #endregion


        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (tcNav.SelectedIndex)
            {
                case 1:
                    lblCustomers.Content = $"Clienti totali: {_azienda.ListaClienti.Count}";
                    lblEmployees.Content = $"Dipendenti totali: {_azienda.ListaDipendenti.Count}";
                    lblExpenses.Content = $"Spese totali: {_azienda.SpeseTotali}€";
                    lblRevenue.Content = $"Entrate totali: {_azienda.EntrateTotali}€";
                    lblProfit.Content = $"Profitto: {_azienda.ProfittoTotale}€";
                    break;
            }
            
        }

        private void mntSalvaJson_Click(object sender, RoutedEventArgs e)
        {
            SalvaJson();
        }
        private void mntApriJson_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CaricaJson();
                RefreshListe();
            }
            catch
            {
                MessaggioErrore("C'è stato un problema nella lettura del file, sicuro il file esista?");
            }
        }

        private void mntSalvaXml_Click(object sender, RoutedEventArgs e)
        {
            SalvaXML();
        }
        private void mntApriXml_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CaricaXML();
            }
            catch
            {
                MessaggioErrore("C'è stato un problema nella lettura del file, sicuro il file esista?");
                return;
            }

            lstDipendenti.ItemsSource = _azienda.ListaDipendenti;
            lstClienti.ItemsSource = _azienda.ListaClienti;
            _toSave = false;

            RefreshListe();
        }
        private void RefreshListe()
        {
            lstClienti.Items.Refresh();
            lstAcquisti.Items.Refresh();
            lstDipendenti.Items.Refresh();
        }
        private void mntApriCsv_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Non ancora implementato.","Errore",MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        private void mntSalvaCsv_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Non ancora implementato.", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        private void btnRimuoviAcquisto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstAcquisti.SelectedIndex == -1)
                    throw new Exception("Devi prima selezionare un acquisto");

                _acquistiCorrenti.RemoveAt(lstAcquisti.SelectedIndex);
                lstAcquisti.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessaggioErrore(ex.Message);
            }
        }
        private void lstAcquisti_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstAcquisti.SelectedIndex == -1)
                btnRimuoviAcquisto.IsEnabled = false;
            else
                btnRimuoviAcquisto.IsEnabled = true;
        }
        private void mntAutosalvataggio_Click(object sender, RoutedEventArgs e)
        {
            _autosalvataggio = !_autosalvataggio;
        }


        #region Ordina clienti
        private void btnOrdinaClientiNome_Click(object sender, RoutedEventArgs e)
        {
            _azienda.ListaClienti.Sort(new SortByName<decimal>());
            lstClienti.Items.Refresh();
        }

        private void btnOrdinaClientiSpesaCrescente_Click(object sender, RoutedEventArgs e)
        {
            _azienda.ListaClienti.Sort();
            lstClienti.Items.Refresh();
        }

        private void btnOrdinaClientiSpesaDecrescente_Click(object sender, RoutedEventArgs e)
        {
            _azienda.ListaClienti.Sort(new SortByBillDecrescent<decimal>());
            lstClienti.Items.Refresh();
        }

        private void btnOrdinaClientiCognome_Click(object sender, RoutedEventArgs e)
        {
            _azienda.ListaClienti.Sort(new SortBySurname<decimal>());
            lstClienti.Items.Refresh();
        }
        #endregion

        #region Ordina Dipendenti
        private void btnOrdinaDipendentiNome_Click(object sender, RoutedEventArgs e)
        {
            _azienda.ListaDipendenti.Sort(new SortByName<decimal>());
            lstDipendenti.Items.Refresh();
        }

        private void btnOrdinaDipendentiCognome_Click(object sender, RoutedEventArgs e)
        {
            _azienda.ListaDipendenti.Sort(new SortBySurname<decimal>());
            lstDipendenti.Items.Refresh();
        }
        private void btnOrdinaDipendentiStipendioCrescente_Click(object sender, RoutedEventArgs e)
        {
            _azienda.ListaDipendenti.Sort(new SortByCrescentSalary<decimal>());
            lstDipendenti.Items.Refresh();
        }
        private void btnOrdinaDipendentiStipendioDecrescente_Click(object sender, RoutedEventArgs e)
        {
            _azienda.ListaDipendenti.Sort(new SortByDecrescentSalary<decimal>());
            lstDipendenti.Items.Refresh();
        }
        #endregion
        
        private void btnStampaTuttiMembri_Click(object sender, RoutedEventArgs e)
        {
            string mex = "";
            foreach (Persona<decimal> p in _azienda)
            {
                mex += p;
            }
            MessageBox.Show(mex);
        }
    }
}
