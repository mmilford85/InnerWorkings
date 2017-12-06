using System;
using System.Collections.Generic;
using System.Linq;

namespace InnerWorkingsJobs.Jobs
{
    public class InvoiceStrategy : IInvoiceStrategy
    {
        private readonly decimal _salesTax;
        private readonly decimal _margin;
        private readonly decimal _extraMargin;

        public InvoiceStrategy(decimal salesTax, decimal margin, decimal extraMargin)
        {
            _salesTax = salesTax;
            _margin = margin;
            _extraMargin = extraMargin;
        }

        public Invoice CalculateInvoice(Job job)
        {
            decimal shippingCost = 0;

            var invoiceItems = new List<InvoiceItem>(job.PrintItems.Count);

            foreach (var printItem in job.PrintItems)
            {
                var saleCost = printItem.TaxExempt
                    ? printItem.Cost
                    : Math.Round(printItem.Cost + (printItem.Cost * _salesTax), 2, MidpointRounding.AwayFromZero);

                shippingCost += printItem.Cost;

                invoiceItems.Add(InvoiceItem.Create(printItem.Name, saleCost));
            }

            var jobMargin = job.ExtraMargin
                ? _margin + _extraMargin
                : _margin;

            var invoiceMarginCost = shippingCost * jobMargin;

            var totalCost = RoundToNearestEvenCent(
                invoiceItems.Sum(invoiceItem => invoiceItem.SaleCost) + invoiceMarginCost);

            return Invoice.Create(invoiceItems, totalCost);
        }

        private decimal RoundToNearestEvenCent(decimal value)
        {
            /*
             * Convert value to '2 cent' units by multiplying by 50, round value to the nearest integer
             * to get the desired '2 cent' (even cents) value, then divide back by 50 to convert back
             * into the proper units
             */
            return (0.02m / 1.00m) * decimal.Round(value * (1.00m / 0.02m), MidpointRounding.AwayFromZero);
        }
    }
}