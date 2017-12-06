namespace InnerWorkingsJobs.Jobs
{
    public class PrintItem
    {
        public string Name { get; private set; }

        public decimal Cost { get; private set; }

        public bool TaxExempt { get; private set; }

        public static PrintItem Create(string name, decimal cost, bool taxExempt)
        {
            return new PrintItem
            {
                Name = name,
                Cost = cost,
                TaxExempt = taxExempt
            };
        }
    }
}
