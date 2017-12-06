namespace InnerWorkingsJobs.Jobs
{
    public interface IInvoiceStrategy
    {
        Invoice CalculateInvoice(Job job);
    }
}
