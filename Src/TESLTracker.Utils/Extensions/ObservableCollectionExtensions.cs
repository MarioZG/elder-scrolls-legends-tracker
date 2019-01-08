using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TESLTracker.Utils.Extensions
{
    public static class ObservableCollectionExtensions
    {
        public static TResult DeepCopy<TResult, T>(this IEnumerable<T> list)
            where T : ICloneable
            where TResult : ObservableCollection<T>, new()
        {
            return Activator.CreateInstance(typeof(TResult), list.Select(x => x.Clone()).Cast<T>()) as TResult;
        }
    }
}
