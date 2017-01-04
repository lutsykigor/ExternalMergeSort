namespace ExternalMergeSort
{
    using System.Collections.Generic;
    public class LinesComparer : IComparer<string>
    {
        private const char SEPARATOR = '.';
        private readonly IComparer<string> baseComparer;
        public LinesComparer(IComparer<string> baseComparer)
        {
            this.baseComparer = baseComparer;
        }

        public int Compare(string x, string y)
        {
            var splitX = x.IndexOf(SEPARATOR) + 1;
            if (splitX == 0)
            {
                return 1;
            }

            var splitY = y.IndexOf(SEPARATOR) + 1;
            if (splitY == 0)
            {
                return -1;
            }

            var textX = x.Substring(splitX, x.Length - splitX);
            var textY = y.Substring(splitY, y.Length - splitY);

            var baseRes = baseComparer.Compare(textX, textY);

            if (baseRes == 0)
            {
                var numStrX = x.Substring(0, splitX);
                var numStrY = y.Substring(0, splitY);
                int numX, numY;
                if (int.TryParse(numStrX, out numX) && int.TryParse(numStrY, out numY))
                {
                    return numX.CompareTo(numY);
                }
            }

            return baseRes;
        }
    }
}
