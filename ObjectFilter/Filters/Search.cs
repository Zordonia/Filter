using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectFilter.Filters
{
    public interface Search
    {
        int Limit { get; }
        int Offset { get; }
        String OrderBy { get; }

        bool Ascending { get;  }
    }

    public class Search<TObject> : Search
    {

        public int Limit { get; private set; }

        public int Offset { get; set; }

        public string OrderBy { get; set; }

        public bool Ascending { get; set; }

        public Search() : this(10, 0, false, null) { }
        public Search(
            int limit = 10,
            int offset = 0,
            bool ascending = false,
            string orderby = null)
        {
            this.Limit = limit;
            this.Offset = offset;
            this.Ascending = ascending;
            this.OrderBy = orderby;
        }
    }
}
