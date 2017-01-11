using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTrackerTests
{
    public class ReferenceComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            return Object.ReferenceEquals(x, y) ? 0 : 1;
        }
    }
}
