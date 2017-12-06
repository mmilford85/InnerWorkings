using System;
using System.Collections.Generic;
using System.Linq;

using InnerWorkingsJobs.Jobs;
using InnerWorkingsJobs.Repositories;

using Moq;

using NUnit.Framework;

namespace InnerWorkingsJobsTests
{
    [TestFixture]
    public class JobRepositoryTests
    {
        private const string extraMarginString = "extra-margin";
        private const string exemptString = "exempt";

        private Mock<IFileRepository> _fileRepositoryMock;

        #region ReadJobFromFile Tests

        [SetUp]
        public void Setup()
        {
            _fileRepositoryMock = new Mock<IFileRepository>();
        }

        [Test]
        public void EmptyFileTest()
        {
            // Arrange
            _fileRepositoryMock
                .Setup(mock => mock.ReadLines(It.IsAny<string>()))
                .Returns(Enumerable.Empty<string>());

            var jobsFileRepository = new JobsFileRepository(_fileRepositoryMock.Object);

            // Act and Assert
            Assert.That(
                () => jobsFileRepository.ReadJobFromFile("fakeFilePath"),
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void OnlyExtraMarginTest()
        {
            // Arrange
            _fileRepositoryMock
                .Setup(mock => mock.ReadLines(It.IsAny<string>()))
                .Returns(new List<string> { extraMarginString });

            var jobsFileRepository = new JobsFileRepository(_fileRepositoryMock.Object);

            // Act and Assert
            Assert.That(
                () => jobsFileRepository.ReadJobFromFile("fakeFilePath"),
                Throws.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("printItems"));
        }

        [Test]
        public void MissingPrintItemCostTest()
        {
            // Arrange
            _fileRepositoryMock
                .Setup(mock => mock.ReadLines(It.IsAny<string>()))
                .Returns(new List<string> { "print-item2" });

            var jobsFileRepository = new JobsFileRepository(_fileRepositoryMock.Object);

            // Act and Assert
            Assert.That(
                () => jobsFileRepository.ReadJobFromFile("fakeFilePath"),
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void MissingPrintItemNameTest()
        {
            // Arrange
            _fileRepositoryMock
                .Setup(mock => mock.ReadLines(It.IsAny<string>()))
                .Returns(new List<string> { "50.00" });

            var jobsFileRepository = new JobsFileRepository(_fileRepositoryMock.Object);

            // Act and Assert
            Assert.That(
                () => jobsFileRepository.ReadJobFromFile("fakeFilePath"),
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void TooManyNamesTest()
        {
            // Arrange
            _fileRepositoryMock
                .Setup(mock => mock.ReadLines(It.IsAny<string>()))
                .Returns(new List<string> { "print item 50.00" });

            var jobsFileRepository = new JobsFileRepository(_fileRepositoryMock.Object);

            // Act and Assert
            Assert.That(
                () => jobsFileRepository.ReadJobFromFile("fakeFilePath"),
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void TooManyCostsTest()
        {
            // Arrange
            _fileRepositoryMock
                .Setup(mock => mock.ReadLines(It.IsAny<string>()))
                .Returns(new List<string> { "print-item1 25.25 50.00 exempt" });

            var jobsFileRepository = new JobsFileRepository(_fileRepositoryMock.Object);

            // Act and Assert
            Assert.That(
                () => jobsFileRepository.ReadJobFromFile("fakeFilePath"),
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void BasicJobTest()
        {
            // Arrange
            _fileRepositoryMock
                .Setup(mock => mock.ReadLines(It.IsAny<string>()))
                .Returns(new List<string> { "print-item1 50.00" });

            var jobsFileRepository = new JobsFileRepository(_fileRepositoryMock.Object);

            // Act
            var job = jobsFileRepository.ReadJobFromFile("fakeFilePath");

            // Assert
            var expectedOuput = Job.Create(
                false,
                new List<PrintItem> { PrintItem.Create("print-item1", 50.00m, false) });

            Assert.That(job, Is.EqualTo(expectedOuput).Using(new JobEqualityComparer()));
        }

        [Test]
        public void ExtraMarginTest()
        {
            // Arrange
            _fileRepositoryMock
                .Setup(mock => mock.ReadLines(It.IsAny<string>()))
                .Returns(new List<string> { extraMarginString, "print-item1 19.99", "print-item2 29.99" });

            var jobsFileRepository = new JobsFileRepository(_fileRepositoryMock.Object);

            // Act
            var job = jobsFileRepository.ReadJobFromFile("fakeFilePath");

            // Assert
            var expectedOuput = Job.Create(
                true,
                new List<PrintItem>
                {
                    PrintItem.Create("print-item1", 19.99m, false),
                    PrintItem.Create("print-item2", 29.99m, false)
                });

            Assert.That(job, Is.EqualTo(expectedOuput).Using(new JobEqualityComparer()));
        }

        [Test]
        public void PrintItemsExemptTest()
        {
            // Arrange
            _fileRepositoryMock
                .Setup(mock => mock.ReadLines(It.IsAny<string>()))
                .Returns(
                    new List<string>
                    {
                        $"print-item1 19.99 {exemptString}",
                        "print-item2 29.99",
                        $"print-item3 39.99 {exemptString}"
                    });

            var jobsFileRepository = new JobsFileRepository(_fileRepositoryMock.Object);

            // Act
            var job = jobsFileRepository.ReadJobFromFile("fakeFilePath");

            // Assert
            var expectedOuput = Job.Create(
                false,
                new List<PrintItem>
                {
                    PrintItem.Create("print-item1", 19.99m, true),
                    PrintItem.Create("print-item2", 29.99m, false),
                    PrintItem.Create("print-item3", 39.99m, true)
                });

            Assert.That(job, Is.EqualTo(expectedOuput).Using(new JobEqualityComparer()));
        }

        #endregion

        /*
         * Output Tests
         * Write all values
         * Write .00, .25, and .50
         */

        #region WriteInvoiceToFile Tests

        [Test]
        public void WriteFullInvoiceTest()
        {
            // Arrange
            _fileRepositoryMock
                .Setup(mock => mock.WriteLines(It.IsAny<string>(), It.IsAny<List<string>>()));

            var jobsFileRepository = new JobsFileRepository(_fileRepositoryMock.Object);

            var invoiceInput = Invoice.Create(
                new List<InvoiceItem>
                {
                    InvoiceItem.Create("item1", 10m),
                    InvoiceItem.Create("item2", 10.25m),
                    InvoiceItem.Create("item3", 10.25m)
                },
                30.50m);

            // Act
            jobsFileRepository.WriteInvoiceToFile("fakeFilePath", invoiceInput);

            // Assert
            _fileRepositoryMock.Verify(
                mock => mock.WriteLines(
                    "fakeFilePath",
                    new List<string> { "item1: $10.00", "item2: $10.25", "item3: $10.25", "total: $30.50" }));
        }

        #endregion

        private sealed class JobEqualityComparer : IEqualityComparer<Job>
        {
            public bool Equals(Job x, Job y)
            {
                return x.ExtraMargin == y.ExtraMargin 
                    && x.PrintItems.SequenceEqual(
                        y.PrintItems,
                        new NameCostTaxExemptEqualityComparer());
            }

            public int GetHashCode(Job obj)
            {
                unchecked
                {
                    return (obj.ExtraMargin.GetHashCode() * 397) ^ obj.PrintItems.GetHashCode();
                }
            }
        }

        private sealed class NameCostTaxExemptEqualityComparer : IEqualityComparer<PrintItem>
        {
            public bool Equals(PrintItem x, PrintItem y)
            {
                return string.Equals(x.Name, y.Name) && x.Cost == y.Cost && x.TaxExempt == y.TaxExempt;
            }

            public int GetHashCode(PrintItem obj)
            {
                unchecked
                {
                    var hashCode = obj.Name.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.Cost.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TaxExempt.GetHashCode();
                    return hashCode;
                }
            }
        }
    }
}
