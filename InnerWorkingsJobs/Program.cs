using System;
using System.IO;

namespace InnerWorkingsJobs
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Incorrect number of args - please specify an input file path and an output file path");

                return;
            }

            var inputPath = args[0];

            if (!File.Exists(inputPath))
            {
                Console.WriteLine($"{inputPath} does not point to an existing file");

                return;
            }

            var outputPath = args[1];

            try
            {
                outputPath = Path.GetFullPath(outputPath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{outputPath} is not a valid path: {e.Message}");

                return;
            }

            try
            {
                Infrastructure.GetJobsService().CreateInvoice(inputPath, outputPath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to calculate and create invoice {e.Message}");

                throw;
            }

            Console.WriteLine($"Job description read from {inputPath}, invoice created and written to {outputPath}");
        }
    }
}
