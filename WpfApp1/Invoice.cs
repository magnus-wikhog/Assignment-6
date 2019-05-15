///<summary>
/// Namn:       Magnus Wikhög
/// Projekt:    Assignment 6
/// Inlämnad:   2019-05-15
///</summary>
using System;
using System.Collections.Generic;
using System.IO;


/// <summary>
/// Klasser för faktura, orderrader och företagsinformation. Obs att jag använder decimal överallt istället för double, eftersom 
/// decimal inte riskerar att avrundas på samma sätt som float eller double, vilket är viktigt när det gäller finansiell information!
/// </summary>
namespace WpfApp1 {
    


    /// <summary>
    /// Klass som representerar en faktura. Klassen kan själv hantera inläsning av faktura från fil 
    /// och formatering till HTML via en extern mall.
    /// </summary>
    class Invoice {
        public int invoiceNumber;
        public DateTime invoiceDate;
        public DateTime dueDate;
        public CompanyDetails receiver;
        public CompanyDetails sender;
        public int itemCount;
        public List<InvoiceItem> items = new List<InvoiceItem>();
        public decimal discount;
        public decimal total{
            get{
                decimal total = 0;
                items.ForEach(item => total+=item.totalPrice);
                return total-discount;
            }
        }
        public string logoImageFilename;

        /// <summary>
        /// Skapar en ny faktura genom att läsa in en textfil. Returnerar en Invoice-instans eller null om ett fel uppstod
        /// </summary>
        /// <param name="filename">Sökväg till textfilen</param>
        public static Invoice createFromFile(string filename) {
            // Vi använder en StreamReader för att läsa från textfilen
            StreamReader file = null;

            // För att hantera fel som kan uppstå vid filläsning använder vi try-catch-finally
            try {
                // Öppna filen
                file = new StreamReader(filename);

                // Skapa en ny Invoiceinstans och läs rad för rad från textfilen
                Invoice invoice = new Invoice();
                invoice.invoiceNumber = int.Parse(file.ReadLine());
                invoice.invoiceDate = DateTime.Parse(file.ReadLine());
                invoice.dueDate = DateTime.Parse(file.ReadLine());

                invoice.receiver = new CompanyDetails();
                invoice.receiver.companyName = file.ReadLine();
                invoice.receiver.contactPerson = file.ReadLine();
                invoice.receiver.streetAddress = file.ReadLine();
                invoice.receiver.zipCode = file.ReadLine();
                invoice.receiver.city = file.ReadLine();
                invoice.receiver.country = file.ReadLine();

                // Vi tar först reda på hur många orderrader det finns...
                invoice.itemCount = int.Parse(file.ReadLine());

                // ...och sedan läser vi in så många ordrar
                for(int i=0; i<invoice.itemCount; i++) {
                    InvoiceItem item = new InvoiceItem();
                    item.description = file.ReadLine();
                    item.quantity = int.Parse(file.ReadLine());
                    item.price = decimal.Parse(file.ReadLine().Replace('.', ','));
                    item.taxPercent = decimal.Parse(file.ReadLine().Replace('.', ','));
                    invoice.items.Add(item);
                }

                invoice.sender = new CompanyDetails();
                invoice.sender.companyName = file.ReadLine();
                invoice.sender.streetAddress = file.ReadLine();
                invoice.sender.zipCode = file.ReadLine();
                invoice.sender.city = file.ReadLine();
                invoice.sender.country = file.ReadLine();
                invoice.sender.phone = file.ReadLine();
                invoice.sender.website = file.ReadLine();

                return invoice;
            }
            catch{
                // Något gick fel, returnera null
                return null;
            }
            finally {
                // Vi stänger alltid filen innan vi returnerar resultatet (om den är öppen)
                if( file != null )
                    file.Close();
            }
        }

        /// <summary>
        /// Ersätter placeholders i den angivna HTML-koden med värden från Invoice-instansen.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public string toHtml(string html){           
            html = html.Replace("$invoiceNumber", invoiceNumber.ToString());
            html = html.Replace("$invoiceDate", invoiceDate.ToShortDateString());
            html = html.Replace("$dueDate", dueDate.ToShortDateString());

            html = html.Replace("$receiver/companyName", receiver.companyName);
            html = html.Replace("$receiver/contactPerson", receiver.contactPerson);
            html = html.Replace("$receiver/streetAddress", receiver.streetAddress);
            html = html.Replace("$receiver/zipCode", receiver.zipCode);
            html = html.Replace("$receiver/city", receiver.city);
            html = html.Replace("$receiver/country", receiver.country);
            html = html.Replace("$receiver/phone", receiver.phone);
            html = html.Replace("$receiver/website", receiver.website);

            // Orderraderna måste skapas här - eftersom vi inte vet hur många orderrader det är på förhand så innehåller HTML-mallen
            // endast en placeholder för samtliga rader, inte en för varje rad.
            string itemRows = "";
            decimal totalTotal = 0;
            decimal totalTotalTax = 0;
            items.ForEach( 
                item => {
                    decimal totalTax = item.quantity * item.price * (item.taxPercent / 100);
                    decimal total = totalTax + item.quantity * item.price;
                    totalTotalTax += totalTax;
                    totalTotal += total;

                    // Skapa HTML för orderraden och infoga aktuella värden
                    itemRows += string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td></tr>",
                        item.description,
                        item.quantity,
                        item.price,
                        item.taxPercent,
                        totalTax,
                        total
                    );
                }
            );
            html = html.Replace("$itemRows", itemRows);

            html = html.Replace("$totalTotalTax", totalTotalTax.ToString());
            html = html.Replace("$totalTotal", totalTotal.ToString());
            html = html.Replace("$discount", discount.ToString());
            html = html.Replace("$payableAmount", (totalTotal-discount).ToString());

            html = html.Replace("$sender/companyName", sender.companyName);
            html = html.Replace("$sender/contactPerson", sender.contactPerson);
            html = html.Replace("$sender/streetAddress", sender.streetAddress);
            html = html.Replace("$sender/zipCode", sender.zipCode);
            html = html.Replace("$sender/city", sender.city);
            html = html.Replace("$sender/country", sender.country);
            html = html.Replace("$sender/phone", sender.phone);
            html = html.Replace("$sender/website", sender.website);


            return html;
        }


    }


    /// <summary>
    /// Klass som representerar vanlig information om ett företag
    /// </summary>
    class CompanyDetails {
        public string companyName { get; set; }
        public string contactPerson;
        public string streetAddress;
        public string zipCode;
        public string city;
        public string country;
        public string phone;
        public string website;
    }


    /// <summary>
    /// Klass som representerar en orderrad för en order
    /// </summary>
    class InvoiceItem {
        public string description { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
        public decimal taxPercent { get; set; }
        public decimal totalTax { get => quantity*price*taxPercent/100; } // Räkna ut total skatt för orderraden
        public decimal totalPrice { get => quantity * price + totalTax; } // Räkna ut totalt pris inkl. skatt för orderraden
    }






}
