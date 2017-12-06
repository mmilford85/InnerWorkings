using System.Collections.Generic;
using System.IO;

namespace InnerWorkingsJobs.Repositories
{
    public class FileRepository : IFileRepository
    {
        public IEnumerable<string> ReadLines(string filePath)
        {
            return File.ReadLines(filePath);
        }

        public void WriteLines(string filePath, IEnumerable<string> lines)
        {
            File.WriteAllLines(filePath, lines);
        }
    }
}