using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectFilter.LinqExtensions
{
    public static class LinqEx
    {
        public static IList<String> ConvertToList(this String strList)
        {
            IList<String> result;
            if (strList.Contains(","))
            {
                result = strList.Split(',').ToList();
            }
            else
            {
                result = new List<String>() { strList }.ToList();
            }
            return result;
        }

        public static bool IsBool(this String value)
        {
            bool result = false;
            return bool.TryParse(value, out result);
        }

        public static bool IsDouble(this String value)
        {
            Double result ;
            return Double.TryParse(value, out result);
        }

        public static bool IsLong(this String value)
        {
            long result;
            return long.TryParse(value, out result);
        }

        public static bool IsInt(this String value)
        {
            int result;
            return int.TryParse(value, out result);
        }

        public static bool IsGuid(this String value)
        {
            Guid result;
            return Guid.TryParse(value, out result);
        }

        public static String ConvertToString<TObject>(this IEnumerable<TObject> values, 
            char delimiter = ',')
        {
            String result = "";
            return values.Select(x => x.ToString()).Aggregate((a, b) => a.ToString() + delimiter+ b.ToString());
        }

        public static IEnumerable<TObject> YieldOne<TObject>(this TObject item)
        {
            yield return item;
        }

        public static IEnumerable<TObject> YieldNull<TObject>(this TObject item)
        {
            if (item == null) yield break;
            yield return item;
        }
        public static IEnumerable<bool> OptimizeBooleanFilter(this IEnumerable<bool> values)
        {
            IEnumerable<bool> result;
            if (values == null || (values.Any(x => x) && values.Any(x => !x)))
            {
                result = Enumerable.Empty<bool>();
            }
            else
            {
                result = values.FirstOrDefault().YieldOne();
            }
            return result;
        }
}
}
