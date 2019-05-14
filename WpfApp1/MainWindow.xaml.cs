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
        Invoice invoice;

        public MainWindow() {
            InitializeComponent();
            this.DataContext = invoice;
            openFile(@"D:\Magnus\Temp\test\visual-studio\projects\Assignment-6\WpfApp1\invoiceDemo1.txt");
        }

        private void mnuOpen_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = "Text files|*.txt";
            openDlg.Title = "Open text file";
            openDlg.ShowDialog();

            if (openDlg.FileName != "") {
                openFile(openDlg.FileName);
            }
        }


        private void openFile(string filename){
            invoice = Invoice.createFromFile(filename);

            if (invoice == null)
                MessageBox.Show("Error when reading file " + filename);
            else{
                //webBrowser.NavigateToString(invoice.toHtml(@"D:\Magnus\Temp\test\visual-studio\projects\Assignment-6\WpfApp1\invoice_template.html"));

                invoiceNumberLabel.Content = invoice.invoiceNumber;
                invoiceDateEdit.Text = invoice.invoiceDate.ToShortDateString();
                dueDateEdit.Text = invoice.dueDate.ToShortDateString();
                receiverDetailsLabel.Content = invoice.receiver.companyName + "\n" + invoice.receiver.contactPerson + "\n" + invoice.receiver.streetAddress + "\n" +
                    invoice.receiver.zipCode + " " + invoice.receiver.city + "\n" + invoice.receiver.country;
                senderCompanyNameLabel.Content = invoice.sender.companyName;


                itemList.Items.Clear();
                invoice.items.ForEach( item => itemList.Items.Add(item) );
                OnInvoiceChanged();
            }
        }


        private void mnuPrint_Click(object sender, RoutedEventArgs e){
            mshtml.IHTMLDocument2 doc = webBrowser.Document as mshtml.IHTMLDocument2;
            doc.execCommand("Print", true, null);
        }

        private void DiscountEdit_PreviewTextInput(object sender, TextCompositionEventArgs e){
            decimal result;
            e.Handled = !decimal.TryParse((sender as TextBox).Text + e.Text, out result);
        }

        private void DiscountEdit_TextChanged(object sender, TextChangedEventArgs e){
            decimal result;
            if (decimal.TryParse((sender as TextBox).Text, out result) && invoice != null ) {
                invoice.discount = result;
                OnInvoiceChanged();
            }
        }


        private void OnInvoiceChanged(){
            if (invoice != null){
                discountEdit.Text = invoice.discount.ToString();
                totalLabel.Content = invoice.total.ToString();
            }
        }

    }
}
