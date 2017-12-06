using InnerWorkingsJobs.Repositories;

namespace InnerWorkingsJobs.Jobs
{
    public class JobsService : IJobsService
    {
        private readonly IJobsFileRepository _jobsFileRepository;
        private readonly IInvoiceStrategy _invoiceStrategy;

        public JobsService(IJobsFileRepository jobsFileRepository, IInvoiceStrategy invoiceStrategy)
        {
            _jobsFileRepository = jobsFileRepository;
            _invoiceStrategy = invoiceStrategy;
        }

        public void CreateInvoice(string inputFilePath, string outPutFilePath)
        {
            var job = _jobsFileRepository.ReadJobFromFile(inputFilePath);

            var invoice = _invoiceStrategy.CalculateInvoice(job);

            _jobsFileRepository.WriteInvoiceToFile(outPutFilePath, invoice);
        }
    }
}
