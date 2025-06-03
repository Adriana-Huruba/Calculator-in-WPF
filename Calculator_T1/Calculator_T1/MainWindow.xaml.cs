using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calculator_T1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext=new CalculatorViewModel();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (DataContext is CalculatorViewModel viewModel)
            {
                viewModel.KeyPressCommand.Execute(e); // Apeleaza metoda din ViewModel
            }
        }
    }
}