using SharedProject_Azienda;
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
             _azienda = new Company<decimal>();


        }
    }
}
