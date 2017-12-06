namespace InnerWorkingsJobs.Jobs
{
    public class InvoiceItem
    {
        public string Name { get; private set; }

        public decimal SaleCost { get; private set; }

        public static InvoiceItem Create(string name, decimal saleCost)
        {
            return new InvoiceItem
            {
                Name = name,
                SaleCost = saleCost
            };
        }
    }
}