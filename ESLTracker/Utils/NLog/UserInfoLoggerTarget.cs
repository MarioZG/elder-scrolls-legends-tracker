using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.ViewModels;
using NLog;
using NLog.Targets;

namespace ESLTracker.Utils.NLog
{
    [Target("UserInfoLogger")]
    public sealed class UserInfoLoggerTarget : TargetWithLayout
    {
        public ObservableCollection<DismissableMessage> Logs { get; } = new ObservableCollection<DismissableMessage>();

        protected override void Write(LogEventInfo logEvent)
        {
            string logMessage = this.Layout.Render(logEvent);
            Logs.Add(new DismissableMessage(
                logEvent.FormattedMessage,
                new RealyAsyncCommand<object>(DismissMessage),
                logEvent.Parameters?[0] as Dictionary<string, string>
                ));


        }

        private Task<object> DismissMessage(object arg)
        {
            Logs.Remove(arg as DismissableMessage);
            return null;
        }
    }
}
