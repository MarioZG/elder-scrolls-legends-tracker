using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils.Extensions
{
    //http://www.hanselman.com/blog/?date=2010-02-04
    public static class IEnumerableExtensions
    {
        public static string ToCsv<T>(this IEnumerable<T> items)
            where T : class
        {
            var csvBuilder = new StringBuilder();
            var properties = typeof(T).GetProperties();

            string header = string.Join(",", properties.Select(p => p.Name.ToCsvValue()).ToArray());
            csvBuilder.AppendLine(header);

            foreach (T item in items)
            {
                string line = string.Join(",", properties.Select(p => p.GetValue(item, null).ToCsvValue()).ToArray());
                csvBuilder.AppendLine(line);
            }
            return csvBuilder.ToString();
        }

        private static string ToCsvValue<T>(this T item)
        {
            if (item == null) return "\"\"";

            if (item is string)
            {
                return string.Format("\"{0}\"", item.ToString().Replace("\"", "\\\""));
            }
            double dummy;
            if (double.TryParse(item.ToString(), out dummy))
            {
                return string.Format("{0}", item);
            }
            return string.Format("\"{0}\"", item);
        }
    }
}
