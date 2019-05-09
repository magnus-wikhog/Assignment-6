using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void mnuOpen_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = "Text files|*.txt";
            openDlg.Title = "Open text file";
            openDlg.ShowDialog();

            if (openDlg.FileName != "") {
                try {
                    MessageBox.Show("TODO: Open file " + openDlg.FileName);
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.ToString(), "Error when opening file");
                }
            }

        }
    }
}
