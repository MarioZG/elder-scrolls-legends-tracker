using ESLTracker.Utils;
using ESLTracker.Utils.Extensions;
using ESLTracker.Utils.SimpleInjector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.ViewModels
{
    /// <summary>
    /// from https://msdn.microsoft.com/magazine/dn605875
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class NotifyTaskCompletion<TResult> : INotifyPropertyChanged
    {
        private ILogger logger = MasserContainer.Container.GetInstance<ILogger>();

        public Task<TResult> Task { get; private set; }
        public TResult Result
        {
            get
            {
                return (Task.Status == TaskStatus.RanToCompletion) ?
                        Task.Result : default(TResult);
            }
        }
        public Task TaskCompletion { get; private set; }
        public TaskStatus Status { get { return Task.Status; } }
        public bool IsCompleted { get { return Task.IsCompleted; } }
        public bool IsNotCompleted { get { return !Task.IsCompleted; } }
        public bool IsSuccessfullyCompleted
        {
            get
            {
                return Task.Status == TaskStatus.RanToCompletion;
            }
        }
        public bool IsCanceled { get { return Task.IsCanceled; } }
        public bool IsFaulted { get { return Task.IsFaulted; } }
        public AggregateException Exception { get { return Task.Exception; } }
        public Exception InnerException
        {
            get
            {
                return (Exception == null) ? null : Exception.InnerException;
            }
        }
        public string ErrorMessage
        {
            get
            {
                return (InnerException == null) ? null : InnerException.Message;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public NotifyTaskCompletion(Task<TResult> task)
        {
            logger.Debug($"NotifyTaskCompletion constructor {task} ");
            Task = task;
           // if (!task.IsCompleted)
            {
                TaskCompletion = WatchTaskAsync(task);
            }
        }

        private async Task WatchTaskAsync(Task task)
        {
            try
            {
                logger.Debug($"NotifyTaskCompletion WatchTaskAsync before await {task}. IsNotCompleted={IsNotCompleted} ");
                await task;
                logger.Debug($"NotifyTaskCompletion WatchTaskAsync await finished {task}. IsNotCompleted={IsNotCompleted} ");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            var propertyChanged = PropertyChanged;
            logger.Debug($"NotifyTaskCompletion WatchTaskAsync raise prop changes {propertyChanged} ");
            if (propertyChanged == null)
                return;
            propertyChanged(this, new PropertyChangedEventArgs("Status"));
            propertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));
            propertyChanged(this, new PropertyChangedEventArgs("IsNotCompleted"));
            if (task.IsCanceled)
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsCanceled"));
            }
            else if (task.IsFaulted)
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsFaulted"));
                propertyChanged(this, new PropertyChangedEventArgs("Exception"));
                propertyChanged(this,
                  new PropertyChangedEventArgs("InnerException"));
                propertyChanged(this, new PropertyChangedEventArgs("ErrorMessage"));
            }
            else
            {
                propertyChanged(this,
                  new PropertyChangedEventArgs("IsSuccessfullyCompleted"));
                propertyChanged(this, new PropertyChangedEventArgs("Result"));
            }
            logger.Debug($"All props change event triggered");
        }
 
    }
}
