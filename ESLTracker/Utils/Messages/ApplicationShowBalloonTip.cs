using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils.Messages
{
    class ApplicationShowBalloonTip
    {
        public string Title { get; }
        public string Message {get ;}

        public ApplicationShowBalloonTip(string title, string message)
        {
            this.Title = title;
            this.Message = message;
        }
    }
}
