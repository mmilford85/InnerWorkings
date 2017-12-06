using System;
using System.Collections.Generic;

namespace InnerWorkingsJobs.Jobs
{
    public class Invoice
    {
        public List<InvoiceItem> InvoiceItems { get; private set; }

        public decimal Total { get; private set; }

        public static Invoice Create(List<InvoiceItem> invoiceItems, decimal total)
        {
            if (invoiceItems == null || invoiceItems.Count == 0)
            {
                throw new ArgumentException("invoiceItems list cannot be empty", nameof(invoiceItems));
            }

            return new Invoice
            {
                InvoiceItems = invoiceItems,
                Total = total
            };
        }
    }
}
