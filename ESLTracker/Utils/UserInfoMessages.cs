using ESLTracker.Utils.NLog;
using ESLTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils
{
    public class UserInfoMessages : ObservableCollection<DismissableMessage>
    {


        public void AddMessage(string message, Dictionary<string, string> clickableUrls)
        {
            Add(new DismissableMessage(
                    message,
                    new RealyAsyncCommand<object>(DismissMessage),
                    clickableUrls
                )
            );
        }

        public void AddMessage(string message)
        {
            AddMessage(message,null);
        }

        private Task<object> DismissMessage(object arg)
        {
            Remove(arg as DismissableMessage);
            return null;
        }
    }
}
