namespace ExternalSort.Extensions
{
    using PriorityQueue;
    using System.Collections.Generic;
    using System.Linq;

    public static class OrderedMergeExtension
    {
        public static IEnumerable<T> OrderedMerge<T>(
           this IEnumerable<IEnumerable<T>> sources,
           IComparer<T> comparer)
        {
            // Precondition checking is done outside of iterator because
            // of its lazy nature
            return OrderedMergeHelper(sources, comparer);
        }

        private static IEnumerable<T> OrderedMergeHelper<T>(
          IEnumerable<IEnumerable<T>> sources,
          IComparer<T> elementComparer)
        {
            // Each sequence is expected to be ordered according to
            // the same comparison logic as elementComparer provides
            var enumerators = sources.Select(e => e.GetEnumerator());

            // The code below holds the following loop invariant:
            // - Priority queue contains enumerators that positioned at
            // sequence element
            // - The queue at the top has enumerator that positioned at
            // the smallest element of the remaining elements of all
            // sequences

            // Ensures that only non empty sequences participate  in merge
            var nonEmpty = enumerators.Where(e => e.MoveNext());
            // Current value of enumerator is its priority
            var comparer = new EnumeratorComparer<T>(elementComparer);
            // Use priority queue to get enumerator with smallest
            // priority (current value)
            var queue = new PriorityQueue<IEnumerator<T>>(nonEmpty, comparer);

            // The queue is empty when all sequences are empty
            while (queue.Count > 0)
            {
                // Dequeue enumerator that positioned at element that
                // is next in the merged sequence
                var min = queue.Dequeue();
                yield return min.Current;
                // Advance enumerator to next value
                if (min.MoveNext())
                {
                    // If it has value that can be merged into resulting
                    // sequence put it into the queue
                    queue.Enqueue(min);
                }
            }
            enumerators = null;
        }

        // Provides comparison functionality for enumerators
        private class EnumeratorComparer<T> : Comparer<IEnumerator<T>>
        {
            private readonly IComparer<T> m_comparer;

            public EnumeratorComparer(IComparer<T> comparer)
            {
                m_comparer = comparer;
            }

            public override int Compare(
               IEnumerator<T> x, IEnumerator<T> y)
            {
                return m_comparer.Compare(x.Current, y.Current);
            }
        }
    }
}
