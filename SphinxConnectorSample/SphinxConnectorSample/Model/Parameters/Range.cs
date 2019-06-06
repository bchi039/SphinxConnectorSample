using System;

namespace SphinxConnectorSample.Model.Parameters
{
    public class Range<T> where T : struct, IComparable
    {
        public Range(T? from, T? to)
        {
            From = from;
            To = to;
        }

        public T? From { get; }
        public T? To { get; }
        
    }
}
