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
            _azienda.ListaDipendenti.Add(
                new Employee<decimal>("Annamaria", "Grondaia", 3000.90m)
                );
            

            _azienda.ListaClienti.Add(
                new Customer<decimal>("Alberto", "Giacomini", new List<Acquisto<decimal>>() { new Acquisto<decimal>("Bugatti",3200m) })
                );
            _azienda.ListaClienti.Add(
                new Customer<decimal>("Giovanni", "Giorgio", new List<Acquisto<decimal>>() { new Acquisto<decimal>("Ferrari", 2400m) })
                );


        }

        private void btnGenerateDipendenti_Click(object sender, RoutedEventArgs e)
        {
            lstDipendenti.ItemsSource = _azienda.ListaDipendenti;
        }
    }
}
