using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArcStache.Extensions
{
    public static class IListTExtensions
    {
        public static string ToDelimitedString<T>(this IList<T> input, string delimiter)
        {
            if (input == null)
            {
                throw new ArgumentException("input");
            }

            StringBuilder sb = new StringBuilder();
            string value = null;
            foreach (T item in input)
            {
                value = (sb.Length == 0) ? item.ToString() : string.Format("{0}{1}", delimiter, item.ToString());
                sb.Append(value);
            }

            return sb.ToString();
        }
    }
}
