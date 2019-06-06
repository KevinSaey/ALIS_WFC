using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse.Shared
{
    public interface IHeuristic
    {
        Dictionary<Sample, float> ApplyOverChoices(Dictionary<Sample, float> chances, Sample picked, SampleGrid grid);
    }
}
