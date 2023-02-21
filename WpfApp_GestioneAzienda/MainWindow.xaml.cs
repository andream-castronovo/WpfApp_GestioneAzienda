using SharedProject_Azienda;
using System.Collections.Generic;
using System.Windows;

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

            _azienda = new Company<decimal>();

            _azienda.ListaDipendenti.Add(
                new Employee<decimal>("Giorgio", "Rossi", 1200.00m)
                );


            rdbCliente.IsChecked = true;
        }


        void CheckGrid()
        {
            if (rdbCliente.IsChecked == true) 
            { 
                grdCliente.Visibility = Visibility.Visible;
                grdImpiegato.Visibility = Visibility.Collapsed;
            }
            else if (rdbImpiegato.IsChecked == true)
            {
                grdCliente.Visibility = Visibility.Collapsed;
                grdImpiegato.Visibility = Visibility.Visible;
            }
        }

        private void rdb_Checked(object sender, RoutedEventArgs e)
        {
            CheckGrid();
        }
        private void btnAddPerson_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text == "")
                return;
            if (txtSurname.Text == "")
                return;

            if (rdbCliente.IsEnabled == true)
                _azienda.ListaClienti.Add(
                    new Customer<decimal>(
                        txtName.Text,
                        txtSurname.Text,
                        new List<Acquisto<decimal>>() { new Acquisto<decimal>("Bugatti", 3600.00m) }
                    )
                );


        }

        private void btnAggiungiAcquisto_Click(object sender, RoutedEventArgs e)
        {
            
        }

    }
}
