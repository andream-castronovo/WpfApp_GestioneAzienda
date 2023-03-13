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
            _controlloTxt = false;
        }

        private void btnGenerateDipendenti_Click(object sender, RoutedEventArgs e)
        {
            lstDipendenti.ItemsSource = _azienda.ListaDipendenti;
        }

        private void Check_Changed(object sender, RoutedEventArgs e)
        {
            CaricaInterfacciaPersona();
        }

        private void CaricaInterfacciaPersona()
        {
            string s = null;
            
            if ((bool)rdbCliente.IsChecked)
                s = "cliente";
            else if ((bool)rdbImpiegato.IsChecked)
                s = "dipendente";

            switch (s)
            {
                case "cliente":
                    grpImpiegati.Visibility = Visibility.Collapsed;
                    grpClienti.Visibility = Visibility.Visible;
                    if (!_caricato)
                        _personaCorrente = new Customer<decimal>();
                    break;
                case "dipendente":
                    grpImpiegati.Visibility = Visibility.Visible;
                    grpClienti.Visibility = Visibility.Collapsed;
                    lstAcquisti.SelectedIndex = -1;
                    cmbListaAcquisti.SelectedIndex = -1;
                    if (!_caricato)
                        _personaCorrente = new Employee<decimal>();
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

                Acquisto<decimal> a = c.ListaAcquisti[c.ListaAcquisti.Count - 1];
                lstAcquisti.Items.Add(
                    $"{a.Tipo} {a.Price:f2}€"
                    );
                
            }
            catch (Exception ex)
            {
                MessaggioErrore(ex.Message);
            }
        }
        // TODO: Per placeholder nelle TextBox vedere evento: GotFocus

        private void btnGenerateClienti_Click(object sender, RoutedEventArgs e)
        {
            lstClienti.Items.Clear();
            foreach (Customer<decimal> c in _azienda.ListaClienti)
            {
                string s = c.OttieniAcquisti("\n\t\t","€");
                lstClienti.Items.Add(c + (s.Length > 0 ? "\n\tLista acquisti:" : "") + s);
            }
        }

        private void lstClienti_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstClienti.SelectedIndex == -1)
                return;

            Customer<decimal> c = _azienda.ListaClienti[lstClienti.SelectedIndex];
            _personaCorrente = c;
            
            CaricaDati(c);

        }

        bool _controlloTxt;
        private void txtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_controlloTxt)
            {
                lstClienti.SelectedIndex = -1;
                lstDipendenti.SelectedIndex = -1;
                CaricaInterfacciaPersona();
            }
        }
        private void CaricaDati(Persona<decimal> p)
        {
            _caricato = true;
            _controlloTxt = false;

            txtNome.Text = p.Nome;
            txtCognome.Text = p.Cognome;

            if (p.GetType() == typeof(Customer<decimal>))
            {
                Customer<decimal> c = (Customer<decimal>)p;
                rdbCliente.IsChecked = true;
                if (c.ListaAcquisti.Count > 0)
                    lstAcquisti.ItemsSource = c.ListaAcquisti;
                else
                    lstAcquisti.Items.Clear();
            }
            else if (p.GetType() == typeof(Employee<decimal>))
            {
                Employee<decimal> emp = (Employee<decimal>)p;
                rdbImpiegato.IsChecked = true;
                txtStipendio.Text = emp.StipendioAnnuo + "";
            }
            _controlloTxt = true;
        }

        private void ResetInterfaccia()
        {
            txtNome.Text = "";
            txtCognome.Text = "";
            txtStipendio.Text = "";
            txtPrezzoAcquisto.Text = "";

            rdbCliente.IsChecked = true;
        }
    }
}
