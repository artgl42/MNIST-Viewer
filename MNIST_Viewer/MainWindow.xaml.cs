using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MNIST_Viewer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            loadFlyout.IsOpen = true;
            //optionsFlyout.IsOpen = false;
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            //optionsFlyout.IsOpen = true;
            loadFlyout.IsOpen = false;
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            filterFlyout.IsOpen = filterFlyout.IsOpen ? false : true;
        }

        
    }
}
