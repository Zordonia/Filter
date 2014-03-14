using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectFilter.Filters
{
    public abstract class IFilterObject<TObject>
    {
        public abstract Filter<TObject> CreateFilter(Dictionary<String, String> queryParams);
    }
}
