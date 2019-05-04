using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse.Shared
{
    static class Util
    {
        public static List<U> CrossreferenceIncludeList<U>(List<U> baseList, List<U> filterList)
        {
            return baseList.Where(l => filterList.Contains(l)).ToList();
        }
    }

    public interface ISharedLogger
    {
        void Log(object message);
    }

    public static class SharedLogger
    {
        public static ISharedLogger CurrentLogger { get; set; }

        public static void Log(object message)
        {
            CurrentLogger.Log(message);
        }
    }

    

}
