using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ObjectFilter.Filters
{
    public class ObjectSpecFilter<T>
    {
        public Expression<Func<T, bool>> Filter { get; set; }

        public ObjectSpecFilter(Expression<Func<T, bool>> filter)
        {
            Filter = filter;
        }
        public ObjectSpecFilter() { }
    }

    public static class ObjectSpecFilterEx
    {
        public static ObjectSpecFilter<T> Or<T>(this ObjectSpecFilter<T> @this, params ObjectSpecFilter<T>[] filters)
        {
            return new ObjectSpecFilter<T>(@this.Filter.Or(filters.Select(x => x.Filter).ToArray()));
        }

        public static ObjectSpecFilter<T> And<T>(this ObjectSpecFilter<T> @this, params ObjectSpecFilter<T>[] filters)
        {
            return new ObjectSpecFilter<T>(@this.Filter.And(filters.Select(x => x.Filter).ToArray()));
        }

        public static IEnumerable<T> YieldOne<T>(this T @this)
        {
            return new List<T>() { @this }.AsEnumerable();
        }
    }
}
