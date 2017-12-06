using System;
using System.Collections.Generic;

namespace InnerWorkingsJobs.Jobs
{
    public class Job
    {
        public bool ExtraMargin { get; private set; }

        public List<PrintItem> PrintItems { get; private set; }

        public static Job Create(bool extraMargin, List<PrintItem> printItems)
        {
            if (printItems == null || printItems.Count == 0)
            {
                throw new ArgumentException("printItems list cannot be empty", nameof(printItems));
            }

            return new Job
            {
                ExtraMargin = extraMargin,
                PrintItems = printItems
            };
        }
    }
}
