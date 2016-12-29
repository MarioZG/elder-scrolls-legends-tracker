using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils
{
    public class DataFileException : Exception
    {
        public bool CanContinue { get; set; }

        public DataFileException(string message, bool canContinue = false) : base(message)
        {
            CanContinue = canContinue;
        }
    }
}
