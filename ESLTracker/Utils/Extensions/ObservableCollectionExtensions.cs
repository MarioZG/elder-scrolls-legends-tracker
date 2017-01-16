using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils.Extensions
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
