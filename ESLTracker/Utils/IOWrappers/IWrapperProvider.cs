using System;

namespace ESLTracker.Utils.IOWrappers
{
    public interface IWrapperProvider
    {
        T GetWrapper<T>();
    }
}