namespace InnerWorkingsJobs.Jobs
{
    public interface IJobsService
    {
        void CreateInvoice(string inputFilePath, string outPutFilePath);
    }
}
