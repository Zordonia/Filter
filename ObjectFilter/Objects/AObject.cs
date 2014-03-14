using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectFilter.LinqExtensions;
using ObjectFilter.Filters;

namespace ObjectFilter.Objects
{
    public partial class AObject
    {
        public String AProperty { get; set; }

        public Guid BProperty { get; set; }

        public bool CProperty { get; set; }

        public bool Active { get; set; }
        public override bool Equals(object obj)
        {
            AObject extObj = obj as AObject;
            if (extObj == null)
            {
                return false;
            }
            return this.AProperty == extObj.AProperty && BProperty == extObj.BProperty && CProperty == extObj.CProperty;
        }
        public static Filter<AObject> CreateFilter(Dictionary<string, string> queryParams)
        {
            Filters.Filter<AObject> filter = Filters.Filter<AObject>.WhereIn(a => a.Active, true);
            foreach (var kvp in queryParams)
            {
                Filters.Filter<AObject> currFilter = null;
                IList<String> values = kvp.Value.ConvertToList();
                switch (kvp.Key.ToUpperInvariant())
                {
                    case "APROPERTY":
                        currFilter = Filters.Filter<AObject>.WhereIn(a => a.AProperty, values.ToArray());
                        break;
                    case "BPROPERTY":
                        currFilter = Filters.Filter<AObject>.WhereIn(a => a.BProperty, values.
                            Where(x => x.IsGuid()).Select(x => Guid.Parse(x)).ToArray());
                        break;
                    case "CPROPERTY":
                        currFilter = Filters.Filter<AObject>.WhereIn(a => a.CProperty, values.
                            Where(x => x.IsBool()).Select(x => bool.Parse(x)).ToArray());
                        break;
                    default:
                        break;
                }
                filter = filter.And(currFilter);
            }
            return filter;
        }
    }
}
