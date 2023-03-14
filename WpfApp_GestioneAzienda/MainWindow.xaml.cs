using SharedProject_Azienda;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Windows.Media;
using System.Linq;

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

        Company<decimal> _azienda;
        Persona<decimal> _personaCorrente;
        bool _caricato;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Decimal è un tipo che gestisce numeri con la virgola in modo
            // più preciso rispetto a double e float; utile per le valute.
            // Per usarlo è necessario aggiungere "m" alla fine del numero in modo
            // da differenziarlo dal double (usato di default per i numeri con la virgola in c#)

            rdbCliente.IsChecked = true;


            _azienda = new Company<decimal>();

            _azienda.ListaDipendenti.Add(
                new Employee<decimal>("Giorgio", "Rossi", 1200.00m)
                );
            _azienda.ListaDipendenti.Add(
                new Employee<decimal>("Annamaria", "Grondaia", 3000.90m)
                );
            

            _azienda.ListaClienti.Add(
                new Customer<decimal>("Alberto", "Giacomini", new List<Acquisto<decimal>>() { new Acquisto<decimal>(Prodotti.Resistore,3200m) })
                );
            _azienda.ListaClienti.Add(
                new Customer<decimal>("Giovanni", "Giorgio", new List<Acquisto<decimal>>() { new Acquisto<decimal>(Prodotti.Condensatore, 2400m) })
                );


            foreach (string s in Enum.GetNames(typeof(Prodotti)))
                cmbListaAcquisti.Items.Add(s);
            cmbListaAcquisti.SelectedIndex = -1;
            
            _caricato = false;

            lstDipendenti.ItemsSource = _azienda.ListaDipendenti;
            lstClienti.ItemsSource = _azienda.ListaClienti;
            
        }

        private string OttieniRuolo()
        {
            if ((bool)rdbCliente.IsChecked)
            {
                _personaCorrente = new Customer<decimal>();
                return "cliente";
            }
            else if ((bool)rdbImpiegato.IsChecked)
            {
                _personaCorrente = new Employee<decimal>();
                return "dipendente";
            }
            else
                throw new Exception("Non è stata effettuata la scelta tra cliente e dipendente");
        }
        private void Check_Changed(object sender, RoutedEventArgs e)
        {
            CaricaInterfacciaPersona(OttieniRuolo());
        }

        private void CaricaInterfacciaPersona(string s)
        {
            switch (s)
            {
                case "cliente":
                    grpImpiegati.Visibility = Visibility.Collapsed;
                    grpClienti.Visibility = Visibility.Visible;
                    if (!_caricato)
                        _personaCorrente = new Customer<decimal>();
                    btnAggiungiAllaAzienda.Content = "Aggiungi Cliente";
                    break;
                case "dipendente":
                    grpImpiegati.Visibility = Visibility.Visible;
                    grpClienti.Visibility = Visibility.Collapsed;
                    lstAcquisti.SelectedIndex = -1;
                    cmbListaAcquisti.SelectedIndex = -1;
                    if (!_caricato)
                        _personaCorrente = new Employee<decimal>();
                    btnAggiungiAllaAzienda.Content = "Aggiungi Dipendente";
                    break;
            }
        }

        private void btnAggiungiAllaAzienda_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _personaCorrente.Nome = ControllaStringa(txtNome.Text);
                _personaCorrente.Cognome = ControllaStringa(txtCognome.Text);
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
                Employee<decimal> emp = (Employee<decimal>) _personaCorrente;
                try
                {
                    emp.StipendioAnnuo = decimal.Parse(ControllaStringa(txtStipendio.Text));
                }
                catch(Exception ex)
                {
                    MessaggioErrore(ex.Message);
                    return;
                }

                _azienda.ListaDipendenti.Add(emp);
                _personaCorrente = new Employee<decimal>();
            }

            lstDipendenti.Items.Refresh();
            lstClienti.Items.Refresh();
        }

        private void MessaggioErrore(string messaggio)
        {
            MessageBox.Show("Si è verificato un errore:\n\t" + messaggio, "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private string ControllaStringa(string s)
        {
            if (s == null)
                throw new Exception("La stringa non deve essere null!");
            if (s == "")
                throw new Exception("Devi scrivere qualcosa!");
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

        private void btnAggiungiAcquisto_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                Customer<decimal> c = (Customer<decimal>)_personaCorrente;

                c.ListaAcquisti.Add(
                    new Acquisto<decimal>(
                        (Prodotti)OttieniNumByNome(
                            ControllaStringa((string)cmbListaAcquisti.SelectedItem),
                            typeof(Prodotti)
                        ),
                        decimal.Parse(
                            ControllaStringa(txtPrezzoAcquisto.Text)
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

        // TODO: Per placeholder nelle TextBox vedere evento: GotFocus

        private void lstClienti_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (lstClienti.SelectedIndex == -1)
            {
                Reset();
                AccessoInput(true);
                return;
            }
            
            lstDipendenti.SelectedIndex = -1;

            Customer<decimal> c = _azienda.ListaClienti[lstClienti.SelectedIndex];
            _personaCorrente = c;
            
            CaricaDati(c);
            ModalitaModifica(false);
        }
        private void lstDipendenti_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstDipendenti.SelectedIndex == -1)
            {
                Reset();
                AccessoInput(true);
                return;
            }

            lstClienti.SelectedIndex = -1;
            
            Employee<decimal> emp = _azienda.ListaDipendenti[lstDipendenti.SelectedIndex];
            _personaCorrente = emp;

            CaricaDati(emp);
            ModalitaModifica(false);

        }

        private void CaricaDati(Persona<decimal> p)
        {
            txtNome.Text = p.Nome;
            txtCognome.Text = p.Cognome;

            if (p.GetType() == typeof(Customer<decimal>))
            {
                Customer<decimal> c = (Customer<decimal>)p;
                rdbCliente.IsChecked = true;
                lstAcquisti.ItemsSource = c.ListaAcquisti;
            }
            else if (p.GetType() == typeof(Employee<decimal>))
            {
                Employee<decimal> emp = (Employee<decimal>)p;
                rdbImpiegato.IsChecked = true;
                txtStipendio.Text = emp.StipendioAnnuo + "";
            }
        }

        private void Reset()
        {
            txtNome.Text = "";
            txtCognome.Text = "";
            txtStipendio.Text = "";
            txtPrezzoAcquisto.Text = "";

            rdbCliente.IsChecked = true;

            lstAcquisti.ItemsSource = null;

            OttieniRuolo();
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
        }

        private void ModalitaModifica(bool mod)
        {
            if (mod)
            {
                btnModifica.Visibility = Visibility.Collapsed;
                btnConfermaModifica.Visibility = Visibility.Visible;
                AccessoInput(true);
            }
            else
            {
                btnModifica.Visibility = Visibility.Visible;
                btnConfermaModifica.Visibility = Visibility.Collapsed;
                AccessoInput(false);
            }
        }
     
        private void btnModifica_Click(object sender, RoutedEventArgs e)
        {
            if (lstClienti.SelectedIndex == -1 && lstDipendenti.SelectedIndex == -1)
            {
                MessageBox.Show("Devi selezionare dalle liste un dipendente o un cliente per modificarlo!");
                return;
            }

            ModalitaModifica(true);
        }
        private void btnConfermaModifica_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
