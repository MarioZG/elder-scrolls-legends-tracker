using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.Decks.DeckImports
{
    public class MissingSPCodeException : Exception
    {
        public MissingSPCodeException()
        {
        }

        public MissingSPCodeException(string message) : base(message)
        {
        }

        public MissingSPCodeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissingSPCodeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
