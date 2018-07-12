using System;

namespace ESLTracker.Utils
{
    public interface IMessenger
    {
        void Register<T>(object recipient, Action<T> action);
        void Register<T>(object recipient, Action<T> action, object context);
        void Send<T>(T message);
        void Send<T>(T message, object context);
        void Unregister<T>(object recipient);
        void Unregister<T>(object recipient, object context);
    }
}