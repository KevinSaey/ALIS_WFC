using System.Collections.Generic;
using System.Linq;

namespace WaveFunctionCollapse.Shared
{
    public interface ICloneable<T>
    {
        T Clone();
    }

    public static class IClonableExtensions
    {
        public static List<T> Clone<T>(this IEnumerable<ICloneable<T>> list)
        {
            return list.Select(e => e.Clone()).ToList();
        }
    }
}