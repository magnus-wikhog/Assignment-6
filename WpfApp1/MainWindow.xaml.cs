///<summary>
/// Namn:       Magnus Wikhög
/// Projekt:    Assignment 6
/// Inlämnad:   2019-05-15
///</summary>
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
    /// Applikationens huvudfönster
    /// </summary>
    public partial class MainWindow : Window {
        Invoice invoice;

        /// <summary>
        /// Konstruktor. 
        /// </summary>
        public MainWindow() {
            InitializeComponent();

            OnInvoiceChanged(); // Uppdatera UI (vi har ju ingen faktura ännu)
        }

        /// <summary>
        /// När användaren klickar på "Open text file" visas en dialogruta där han/hon kan välja en textfil, vilken vi sedan öppnar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuOpenInvoice_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = "Text files|*.txt";
            openDlg.Title = "Open text file";
            openDlg.ShowDialog();

            if (openDlg.FileName != "") {
                openFile(openDlg.FileName);
            }
        }

        /// <summary>
        /// Öppna den angivna filen och skapa en faktura från den samt uppdatera UI.
        /// </summary>
        /// <param name="filename"></param>
        private void openFile(string filename){
            // Skapa fakturan från den angivna textfilen
            invoice = Invoice.createFromFile(filename);
            
            if (invoice == null){
                // Om invoice är null gick något fel vid inläsning från filen
                MessageBox.Show("Error when reading file " + filename);
            }
            else{
                // Denna metod anropar vi alltid när vi ändrat något i fakturan (samt när vi nyss skapat den)
                OnInvoiceChanged();
            }
        }

        /// <summary>
        /// Låt användaren välja en bildfil som visas i fakturafönstret. Bildenfilen skrivs i nuläget inte ut.
        /// </summary>
        private void mnuLoadLogoImage_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = "Image files|*.jpg;*.png;*.jpeg";
            openDlg.Title = "Load logo image";
            openDlg.ShowDialog();

            if (openDlg.FileName != "") {
                logoImage.Source = new BitmapImage(new Uri(openDlg.FileName));

                if (invoice != null) {
                    invoice.logoImageFilename = openDlg.FileName;
                    OnInvoiceChanged(); // Vi anropar alltid denna metoden när vi ändrat något i fakturainstansen
                }
            }
        }

        /// <summary>
        /// Skriv ut fakturan.
        /// </summary>
        private void mnuPrint_Click(object sender, RoutedEventArgs e){
            // Utskriften fungerar endast om WebBrowsern har renderats först, så vi visar "förhandsgranskningen".
            tabControl.SelectedItem = printPreviewTab;

            // Utskrift kräver att vi omvandlar HTML-dokumentet till ett IHTMLDocument2
            mshtml.IHTMLDocument2 doc = webBrowser.Document as mshtml.IHTMLDocument2;

            // Startar utskriften
            doc.execCommand("Print", true, null);
        }

        /// <summary>
        /// Tillåt endast decimaltal i discount (med , som decimalskiljetecken!).
        /// </summary>
        private void DiscountEdit_PreviewTextInput(object sender, TextCompositionEventArgs e){
            decimal result;
            e.Handled = !decimal.TryParse((sender as TextBox).Text + e.Text, out result);
        }

        /// <summary>
        /// Användaren har ändrat discount
        /// </summary>
        private void DiscountEdit_TextChanged(object sender, TextChangedEventArgs e){
            decimal result;

            // Om den angivna texten är ett decimaltal så uppdaterar vi fakturan och UI
            if (decimal.TryParse((sender as TextBox).Text, out result) && invoice != null ) {
                invoice.discount = result;
                OnInvoiceChanged();
            }
        }


        /// <summary>
        /// Denna metod skall alltid anropas när något i fakturan ändrats så att vi kan uppdatera det grafiska
        /// användargränssnittet.
        /// </summary>
        private void OnInvoiceChanged(){
            // Om vi har en fakturainstans så uppdaterar vi GUI, annars döljer vi hela TabControl'en.
            if (invoice != null) {
                // Uppdatera GUI med aktuella fakturadata
                invoiceNumberLabel.Content = invoice.invoiceNumber;
                invoiceDateEdit.Text = invoice.invoiceDate.ToShortDateString();
                dueDateEdit.Text = invoice.dueDate.ToShortDateString();
                receiverDetailsLabel.Content = invoice.receiver.companyName + "\n" + invoice.receiver.contactPerson + "\n" + invoice.receiver.streetAddress + "\n" +
                    invoice.receiver.zipCode + " " + invoice.receiver.city + "\n" + invoice.receiver.country;
                senderCompanyNameLabel.Content = invoice.sender.companyName;

                // Visa samtliga orderrader
                itemList.Items.Clear();
                invoice.items.ForEach(item => itemList.Items.Add(item));

                senderCompanyNameLabel.Content = invoice.sender.companyName;
                senderAddressLine1.Content = invoice.sender.streetAddress;
                senderAddressLine2.Content = invoice.sender.zipCode + " " + invoice.sender.city + ", " + invoice.sender.country;
                senderContactPhone.Content = invoice.sender.phone;
                senderContactWebsite.Content = invoice.sender.website;

                discountEdit.Text = invoice.discount.ToString();
                totalLabel.Content = invoice.total.ToString();

                // Uppdatera "förhandsgranskningen" av HTML-dokumentet som används för utskrift
                webBrowser.NavigateToString(invoice.toHtml(Properties.Resources.invoice_template));

                // Fakturan är inläst, uppdatera UI
                tabControl.Visibility = Visibility.Visible;
                mnuPrint.IsEnabled = true;
                mnuLoadLogoImage.IsEnabled = true;
            }
            else {
                // Ingen faktura är inläst, uppdatera UI
                tabControl.Visibility = Visibility.Hidden;
                mnuPrint.IsEnabled = false;
                mnuLoadLogoImage.IsEnabled = false;
            }
        }

        /// <summary>
        /// Avslutar programmet
        /// </summary>
        private void mnuExit_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        /// <summary>
        /// Uppdatera datumet i fakturainstansen när användaren ändrar datum
        /// </summary>
        private void InvoiceDateEdit_TextChanged(object sender, TextChangedEventArgs e) {
            DateTime dt;
            if (DateTime.TryParse((sender as TextBox).Text, out dt)) {
                invoice.invoiceDate = dt;
                OnInvoiceChanged();
            }
        }

        /// <summary>
        /// Uppdatera datumet i fakturainstansen när användaren ändrar datum
        /// </summary>
        private void DueDateEdit_TextChanged(object sender, TextChangedEventArgs e) {
            DateTime dt;
            if (DateTime.TryParse((sender as TextBox).Text, out dt)) {
                invoice.dueDate = dt;
                OnInvoiceChanged();
            }
        }


        /// <summary>
        /// Säkerställ att förhandsgranskningen är uppdaterad när man byter till den fliken.
        /// </summary>
        private void OnPrintPreviewTabSelected(object sender, RoutedEventArgs e) {
            if ((sender as TabItem) != null) {
                webBrowser.NavigateToString(invoice.toHtml(Properties.Resources.invoice_template));
            }
        }
    }
}
