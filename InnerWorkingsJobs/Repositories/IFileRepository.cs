using System.Collections.Generic;

namespace InnerWorkingsJobs.Repositories
{
    public interface IFileRepository
    {
        IEnumerable<string> ReadLines(string filePath);

        void WriteLines(string filePath, IEnumerable<string> lines);
    }
}
