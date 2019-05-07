using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse.Shared
{

    internal class Engine<T> where T : Sample
    {
        List<T> _sampleLibrary;
        List<Connection> _connections;
        List<IHeuristic<T>> _heuristics;
        SampleGrid<T> _grid;
        int _counter = 0;

        public Engine(List<T> samples, List<IHeuristic<T>> heuristics, SampleGrid<T> grid)
        {
            _sampleLibrary = samples;
            _heuristics = heuristics;
            _grid = grid;
        }

        public void Execute()
        {
            while (!_grid.IsAllDetermined && !_grid.HasConflict)
            {
                Step();
                if (_grid.IsAllDetermined) SharedLogger.Log("Grid is all determined");
            }

        }

        public void Step(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                if (!_grid.IsAllDetermined && !_grid.HasConflict)
                {
                    Step();
                    if (_grid.IsAllDetermined) SharedLogger.Log("Grid is all determined");
                }
            }

        }


        void Step()
        {
            _counter++;
            //SharedLogger.Log($"Step number {_counter}");
            Random rnd = new Random(Guid.NewGuid().GetHashCode());

            // One step of the algorithm:
            // a. Pick out the lowest entropy
            int lowestEntropyIndex = _grid.FindLowestNonZeroEntropy();
            BitArray lowestEntropy = _grid.PossibleSamples[lowestEntropyIndex];


            // b. Apply full list of heuristics over the sample chances (what is the starting proportion of choices?)


            // c. Pick one choice according to the chances supplied by heuristics
            // for now, just select a random sample, later we'll add heuristics
            List<int> possibleSamples = UtilShared.ToIntegerList(lowestEntropy);

            int nextSampleIndex = rnd.Next(1, possibleSamples.Count);
            int selectedSample = possibleSamples[nextSampleIndex];
            _grid.SelectedSamples[lowestEntropyIndex] = selectedSample;

            SharedLogger.Log($"Sample {nextSampleIndex} assigned");
            UtilShared.SetFalseBut(_grid.PossibleSamples[lowestEntropyIndex], 0); //0 is always an empty connection



            // d. Use the sample.propagate(grid) to apply over grid
            _sampleLibrary[selectedSample].Propagate(_grid, lowestEntropyIndex);

        }
    }
}
