using InnerWorkingsJobs.Jobs;

namespace InnerWorkingsJobs.Jobs
{
    public interface IJobsFileRepository
    {
        Job ReadJobFromFile(string filePath);

        void WriteInvoiceToFile(string filePath, Invoice invoice);
    }
}
