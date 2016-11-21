using System;

namespace ESLTracker.Utils.IOWrappers
{
    public interface IWrapperProvider
    {
        object GetWrapper(Type type);
    }
}