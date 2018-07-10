using System;

namespace ESLTracker.Utils
{
    public interface IDateTimeProvider
    {
        DateTime DateTimeNow { get; }
    }
}