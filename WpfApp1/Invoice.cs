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
                    item.price = double.Parse(file.ReadLine().Replace('.', ','));
                    item.taxPercent = double.Parse(file.ReadLine().Replace('.', ','));
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

    }



    class CompanyDetails {
        public string companyName;
        public string contactPerson;
        public string streetAddress;
        public string zipCode;
        public string city;
        public string country;
        public string phone;
        public string website;
    }



    class InvoiceItem {
        public string description;
        public int quantity;
        public double price;
        public double taxPercent;
    }

}
