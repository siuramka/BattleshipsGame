using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for TestModeWindow.xaml
    /// </summary>
    public partial class TestModeWindow : Window
    {
        public TestModeWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Close();
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.TestModeButton.IsEnabled = true; // Or set it to false as needed
            }
            e.Cancel = true;
        }

    }
}
