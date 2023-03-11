using SharedProject_Azienda;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Windows.Media;

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
        }

        private void btnGenerateDipendenti_Click(object sender, RoutedEventArgs e)
        {
            lstDipendenti.ItemsSource = _azienda.ListaDipendenti;
        }

        private void Check_Changed(object sender, RoutedEventArgs e)
        {
            RadioButton rdb = (RadioButton)sender;

            switch (rdb.Tag)
            {
                case "cliente":
                    grpImpiegati.Visibility = Visibility.Collapsed;
                    grpClienti.Visibility = Visibility.Visible;
                    break;
                case "dipendente":
                    grpImpiegati.Visibility = Visibility.Visible;
                    grpClienti.Visibility = Visibility.Collapsed;
                    lstAcquisti.SelectedIndex = -1;
                    cmbListaAcquisti.SelectedIndex = -1;
                    break;
            }
        }

        private void btnAggiungiAllaAzienda_Click(object sender, RoutedEventArgs e)
        {
            string nome;
            string cognome;
            
            try
            {
                nome = ControllaStringa(txtNome.Text);
                cognome = ControllaStringa(txtCognome.Text);
            }
            catch (Exception ex)
            {
                MessaggioErrore(ex.Message);
                return;
            }

            Persona<decimal> p = null;
            if ((bool)rdbCliente.IsChecked)
            {
                if (lstAcquisti.Items.Count == 0)
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
                
                List<Acquisto<decimal>> acquistos = null;
                foreach (string item in lstAcquisti.Items)
                {
                    
                }


                p = new Customer<decimal>(nome, cognome);
                _azienda.ListaClienti.Add((Customer<decimal>) p);
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

        private void btnAggiungiAcquisto_Click(object sender, RoutedEventArgs e)
        {
            lstAcquisti.Items.Add(ControllaStringa((string)cmbListaAcquisti.SelectedItem));
        }


        private void PlaceholderTextBox(TextBox txt, string s)
        {
            if (txt.Text == "")
            {
                txt.Text = s;
                txt.Foreground = Brushes.Gray;
            }
            else if (txt.Text == s)
                txt.Text = "";
        }

        private void txtPrezzoAcquisto_TextChanged(object sender, TextChangedEventArgs e)
        {
            PlaceholderTextBox((TextBox)sender, "$");
        }
    }
}
