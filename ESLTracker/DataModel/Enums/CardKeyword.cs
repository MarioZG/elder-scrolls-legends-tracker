using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.DataModel.Enums
{
    public enum CardKeyword
    {
        None = 0,
        Breakthrough = 1,
        Drain = 2,
        Prophecy = 4,
        Guard = 8,
        Charge = 16,
        Lethal = 32,
        Ward = 64,
        Regenerate = 128,
        Rally = 256,
    }
}
