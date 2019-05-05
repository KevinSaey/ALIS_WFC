using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse.Shared
{
    internal class Engine<T> where T : Sample
    {
        List<T> _sampleLibrary;
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
            while (!_grid.IsAllDetermined&&!_grid.HasConflict)
            {
                SharedLogger.Log("Executing");
                Step();

            }

        }

        

        void Step()
        {
            _counter++;
            SharedLogger.Log($"Sample number {_counter}");
            Random rnd = new Random();

            // One step of the algorithm:
            // a. Pick out the lowest entropy
            int lowestEntropyIndex = _grid.FindLowestNonZeroEntropy();
            
            List<int> lowestEntropy = _grid.PossibleSampleGrid[lowestEntropyIndex];

            // b. Apply full list of heuristics over the sample chances (what is the starting proportion of choices?)


            // c. Pick one choice according to the chances supplied by heuristics
            // for now, just select a random sample, later we'll add heuristics
            int selectedSample = lowestEntropy[rnd.Next(0, lowestEntropy.Count)];
            _grid.PossibleSampleGrid[lowestEntropyIndex] = new List<int>{selectedSample};
            

            ///FIRST THING TO DO: MAKE THE GRID A LIST OF SAMPLES!____________________________________________________________________________

            // d. Use the sample.propagate(grid) to apply over grid

        }
    }
}
