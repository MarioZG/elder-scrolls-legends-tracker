using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.Cards
{
    public class DoubleCardException : Exception
    {
        public DoubleCardException(string message) : base(message)
        {
        }
    }
}
