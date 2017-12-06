using System.Collections.Generic;

using InnerWorkingsJobs.Jobs;

using NUnit.Framework;

namespace InnerWorkingsJobsTests
{
    [TestFixture]
    public class InvoiceStrategyTests
    {
        private IInvoiceStrategy _invoiceStrategy;

        [OneTimeSetUp]
        public void SetUp()
        {
            _invoiceStrategy = new InvoiceStrategy(0.07m, 0.11m, 0.05m);
        }

        [Test]
        public void SingleItemJob()
        {
            // Arrange
            var job = Job.Create(
                false,
                new List<PrintItem>
                {
                    PrintItem.Create("Test", 200m, false)
                });

            // Act
            var invoice = _invoiceStrategy.CalculateInvoice(job);

            // Assert
            Assert.That(invoice.Total, Is.EqualTo(236));
            AssertPrintItemIsCorrect(invoice.InvoiceItems[0], "Test", 214);
        }

        //Job with no items tax exempt, no extra margin
        [Test]
        public void MultiItemsNoMarginNoExemptJob()
        {
            // Arrange
            var job = Job.Create(
                false,
                new List<PrintItem>
                {
                    PrintItem.Create("Test1", 225.99m, false),
                    PrintItem.Create("Test2", 199.99m, false),
                    PrintItem.Create("Test3", 59.99m, false)
                });

            // Act
            var invoice = _invoiceStrategy.CalculateInvoice(job);

            // Assert
            Assert.That(invoice.Total, Is.EqualTo(573.44m));
            AssertPrintItemIsCorrect(invoice.InvoiceItems[0], "Test1", 241.81m);
            AssertPrintItemIsCorrect(invoice.InvoiceItems[1], "Test2", 213.99m);
            AssertPrintItemIsCorrect(invoice.InvoiceItems[2], "Test3", 64.19m);
        }

        //Job with all items tax exempt, with extra margin

        [Test]
        public void MultiItemsAllExemptExtraMarginJob()
        {
            // Arrange
            var job = Job.Create(
                true,
                new List<PrintItem>
                {
                    PrintItem.Create("Test1", 123.45m, true),
                    PrintItem.Create("Test2", 56.78m, true)
                });

            // Act
            var invoice = _invoiceStrategy.CalculateInvoice(job);

            // Assert
            Assert.That(invoice.Total, Is.EqualTo(209.06m));
            AssertPrintItemIsCorrect(invoice.InvoiceItems[0], "Test1", 123.45m);
            AssertPrintItemIsCorrect(invoice.InvoiceItems[1], "Test2", 56.78m);
        }

        // Job with only one item tax exempt, with extra margin
        [Test]
        public void MultiItemsOneExemptExtraMarginJob()
        {
            // Arrange
            var job = Job.Create(
                true,
                new List<PrintItem>
                {
                    PrintItem.Create("Test1", 29.99m, false),
                    PrintItem.Create("Test2", 39.99m, false),
                    PrintItem.Create("Test3", 49.99m, true),
                    PrintItem.Create("Test4", 59.99m, false)
                });

            // Act
            var invoice = _invoiceStrategy.CalculateInvoice(job);

            // Assert
            Assert.That(invoice.Total, Is.EqualTo(217.86m));
            AssertPrintItemIsCorrect(invoice.InvoiceItems[0], "Test1", 32.09m);
            AssertPrintItemIsCorrect(invoice.InvoiceItems[1], "Test2", 42.79m);
            AssertPrintItemIsCorrect(invoice.InvoiceItems[2], "Test3", 49.99m);
            AssertPrintItemIsCorrect(invoice.InvoiceItems[3], "Test4", 64.19m);
        }

        [Test]
        public void TotalRoundsDownToNearestEvenCent()
        {
            // Arrange
            var job = Job.Create(
                false,
                new List<PrintItem>
                {
                    PrintItem.Create("Test", 19.99m, false)
                });

            // Act
            var invoice = _invoiceStrategy.CalculateInvoice(job);

            // Assert
            //Before rounding: 23.5882
            Assert.That(invoice.Total, Is.EqualTo(23.58m));
        }

        [Test]
        public void TotalRoundsUpToNearestEvenCent()
        {
            // Arrange
            var job = Job.Create(
                false,
                new List<PrintItem>
                {
                    PrintItem.Create("Test", 22.75m, true)
                });

            // Act
            var invoice = _invoiceStrategy.CalculateInvoice(job);

            // Assert
            // Before rounding: 25.2525
            Assert.That(invoice.Total, Is.EqualTo(25.26m));
        }

        private void AssertPrintItemIsCorrect(InvoiceItem invoiceItem, string name, decimal salesCost)
        {
            Assert.That(invoiceItem.Name, Is.EqualTo(name));
            Assert.That(invoiceItem.SaleCost, Is.EqualTo(salesCost));
        }
    }
}
