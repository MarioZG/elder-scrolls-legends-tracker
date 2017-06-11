using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ESLTracker.Utils.NLog
{
    public class DismissableMessage
    {
        private string message;
        public string Message { get { return message; } }

        private ICommand dismissCommand;
        public ICommand DismissCommand { get { return dismissCommand; } }

        public Dictionary<string, string> ClickableUrls { get; }

        public DismissableMessage(string message, ICommand dismissCommand)
        {
            this.message = message;
            this.dismissCommand = dismissCommand;
        }

        public DismissableMessage(string message, ICommand dismissCommand, Dictionary<string, string> clicableUrls)
        {
            this.message = message;
            this.dismissCommand = dismissCommand;
            this.ClickableUrls = clicableUrls;
        }
    }
}
