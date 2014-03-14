using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectFilter.Objects
{
    public class DObject
    {
        public String AProperty { get; set; }

        public long BProperty { get; set; }

        public int CProperty { get; set; }

        public AObject DProperty { get; set; }
    }
}
