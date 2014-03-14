using ObjectFilter.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectFilter.LinqExtensions;

namespace ObjectFilter.Filters
{
    public class AObjectSearch : Search<AObject>
    {
        public AObjectSearch(
            int limit = 10,
            int offset = 0,
            bool ascending = false,
            string orderby = null,
            IEnumerable<String> aproperties = null,
            IEnumerable<Guid> bproperties = null,
            IEnumerable<bool> cproperties = null,
            IEnumerable<bool> actives = null)
            : base(limit, offset, ascending, orderby)
        {
            this.APropertyFilters = aproperties;
            this.BPropertyFilters = bproperties;
            this.CPropertyFilters = cproperties.OptimizeBooleanFilter();
            this.ActiveFilters = actives.OptimizeBooleanFilter();
        }

        public IEnumerable<String> APropertyFilters { get; private set; }

        public IEnumerable<Guid> BPropertyFilters { get; private set; }

        public IEnumerable<bool> CPropertyFilters { get; private set; }

        public IEnumerable<bool> ActiveFilters { get; private set; }


    }
}
