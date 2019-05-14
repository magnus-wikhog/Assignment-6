using System;
using System.Collections.Generic;
using System.IO;

namespace WpfApp1 {
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

        public static Invoice createFromFile(string filename) {
            StreamReader file = null;

            try {
                file = new StreamReader(filename);

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

                invoice.itemCount = int.Parse(file.ReadLine());
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
                return null;
            }
            finally {
                if( file != null )
                    file.Close();
            }
        }


        public string toHtml(string filename){
            string html = File.ReadAllText(filename);

            html = html.Replace("$invoiceNumber", invoiceNumber.ToString());
            html = html.Replace("$invoiceDate", invoiceDate.ToString());
            html = html.Replace("$dueDate", dueDate.ToString());

            html = html.Replace("$receiver/companyName", receiver.companyName);
            html = html.Replace("$receiver/contactPerson", receiver.contactPerson);
            html = html.Replace("$receiver/streetAddress", receiver.streetAddress);
            html = html.Replace("$receiver/zipCode", receiver.zipCode);
            html = html.Replace("$receiver/city", receiver.city);
            html = html.Replace("$receiver/country", receiver.country);
            html = html.Replace("$receiver/phone", receiver.phone);
            html = html.Replace("$receiver/website", receiver.website);

            string itemRows = "";
            decimal totalTotal = 0;
            decimal totalTotalTax = 0;
            items.ForEach( 
                item => {
                    decimal totalTax = item.quantity * item.price * (item.taxPercent / 100);
                    decimal total = totalTax + item.quantity * item.price;
                    totalTotalTax += totalTax;
                    totalTotal += total;

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



    class InvoiceItem {
        public string description { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
        public decimal taxPercent { get; set; }
        public decimal totalTax { get => quantity*price*taxPercent/100; }
        public decimal totalPrice { get => quantity * price + totalTax; }
    }






}
