namespace ExternalMergeSort
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;
    using ExternalSort.ExternalMergeSort;
    class Program
    {
        static void Main(string[] args)
        {
            var start = DateTime.Now;
            // capacity, mergeCount and unsortedFileName are initialized elsewhere
            var sorter = new TextFileOfNumbersExternalSorter(1000, 100);
            var sortedFileName = sorter.Sort("<insert-source-file-path-here>");

            var end = DateTime.Now - start;
            Console.WriteLine(string.Format("Completed. Elapsed {0} seconds", end.TotalSeconds));
            Console.ReadLine();
        }

        internal class TextFileOfNumbersExternalSorter : ExternalSorter<string>
        {
            public TextFileOfNumbersExternalSorter(int capacity, int mergeCount)
                : base(new LinesComparer(StringComparer.OrdinalIgnoreCase), capacity, mergeCount)
            {
            }

            protected override string Write(IEnumerable<string> run)
            {
                var file = Path.GetTempFileName();
                using (var writer = new StreamWriter(file))
                {
                    run.ToList().ForEach(writer.WriteLine);
                }
                return file;
            }

            protected override IEnumerable<string> Read(string name)
            {
                using (var reader = new StreamReader(name))
                {
                    while (!reader.EndOfStream)
                        yield return reader.ReadLine();
                }
                File.Delete(name);
            }
        }
    }
}
