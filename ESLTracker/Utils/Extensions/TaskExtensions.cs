using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils.Extensions
{
    public static class TaskExtensions
    {
        //https://blogs.msdn.microsoft.com/pfxteam/2009/06/01/tasks-and-unhandled-exceptions/
        public static Task FailFastOnException(this Task task)
        {
            task.ContinueWith(c => Environment.FailFast("Task faulted", c.Exception),
                TaskContinuationOptions.OnlyOnFaulted |
                TaskContinuationOptions.ExecuteSynchronously |
                TaskContinuationOptions.DenyChildAttach);
            return task;
        }
    }
}
