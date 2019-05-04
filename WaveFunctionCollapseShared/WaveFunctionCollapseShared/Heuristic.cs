using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse.Shared
{
    public interface Heuristic<T> where T : Sample
    {
        Dictionary<T, float> ApplyOverChoices(Dictionary<T, float> chances, T picked, SampleGrid<T> grid);
    }
}
