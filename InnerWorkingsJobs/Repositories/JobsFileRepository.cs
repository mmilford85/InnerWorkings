using System;
using System.Linq;

using InnerWorkingsJobs.Jobs;

namespace InnerWorkingsJobs.Repositories
{
    public class JobsFileRepository : IJobsFileRepository
    {
        private const string extraMarginLine = "extra-margin";

        private const string taxExemptLine = "exempt";

        private readonly IFileRepository _fileRepository;

        public JobsFileRepository(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public Job ReadJobFromFile(string filePath)
        {
            var extraMargin = false;

            var jobLines = _fileRepository.ReadLines(filePath).ToList();

            if (jobLines == null || !jobLines.Any())
            {
                throw new InvalidOperationException($"Input file {filePath} is empty");
            }

            if (string.Equals(jobLines.First(), extraMarginLine, StringComparison.OrdinalIgnoreCase))
            {
                extraMargin = true;
            }

            var printItemLines = extraMargin
                ? jobLines.Skip(1)
                : jobLines;

            return Job.Create(
                extraMargin,
                printItemLines.Select(ReadPrintItem).ToList());
        }

        public void WriteInvoiceToFile(string filePath, Invoice invoice)
        {
            var invoiceLineItems = invoice.InvoiceItems
                .Select(invoiceItem => $"{invoiceItem.Name}: {invoiceItem.SaleCost:C2}")
                .ToList();

            invoiceLineItems.Add($"total: ${invoice.Total}");

            _fileRepository.WriteLines(filePath, invoiceLineItems);
        }

        private PrintItem ReadPrintItem(string line)
        {
            var printItemParts = line.Split(' ');

            var lineParts = printItemParts.Length;

            if (lineParts != 2 && lineParts != 3)
            {
                throw new InvalidOperationException($"Print item line {line} does not contain enough data");
            }

            decimal itemCost;

            if (!decimal.TryParse(printItemParts[1], out itemCost))
            {
                throw new InvalidOperationException($"Cannot parse the cost for item {line}");
            }

            var exempt = lineParts == 3 && string.Equals(
                printItemParts[2],
                taxExemptLine,
                StringComparison.OrdinalIgnoreCase);

            //TO DO
            return PrintItem.Create(
                printItemParts[0],
                itemCost,
                exempt);
        }
    }
}
