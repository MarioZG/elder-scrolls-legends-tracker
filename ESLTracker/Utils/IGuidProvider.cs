using System;

namespace ESLTracker.Utils
{
    public interface IGuidProvider
    {
        Guid GetNewGuid();
    }
}