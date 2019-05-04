using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    static class Util
    {
        public static List<U> CrossreferenceIncludeList<U>(List<U> baseList, List<U> filterList)
        {
            return baseList.Where(l => filterList.Contains(l)).ToList();
        }
    }
}
