using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils
{
    public class GuidProvider : IGuidProvider
    {
        public Guid GetNewGuid()
        {
            return Guid.NewGuid();
        }
    }
}
