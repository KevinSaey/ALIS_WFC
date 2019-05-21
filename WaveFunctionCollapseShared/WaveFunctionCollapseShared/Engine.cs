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

        public List<int> Step(int amount)
        {
            List<int> setSamples = new List<int>();
            for (int i = 0; i < amount; i++)
            {
                if (!_grid.IsAllDetermined && !_grid.HasConflict)
                {
                    setSamples.AddRange(Step());
                    if (_grid.IsAllDetermined) SharedLogger.Log("Grid is all determined");
                }
            }
            return setSamples;
        }


        List<int>  Step()
        {
            List<int> setSamples = new List<int>();
            _counter++;
            int lowestEntropyIndex;
            if (_counter == 1)
            {
                //start from a random sample
                lowestEntropyIndex = UtilShared.RandomNR.Next(0, _grid.PossibleSamples.Count - 1);
            }
            else
            {
                lowestEntropyIndex = _grid.FindLowestNonZeroEntropy();
            }
            //SharedLogger.Log($"Step number {_counter}");

            // One step of the algorithm:
            // a. Pick out the lowest entropy

            /*------------------------------------------------- RANDOM SAMPLE WITH LOWEST ENTROPY, Causes conflicts
            List<int> lowestSamples = new List<int>();
            for (int i = 0; i < _grid.PossibleSamples.Count; i++)
            {
                if (_grid.Entropy(_grid.PossibleSamples[i]) == _grid.Entropy(_grid.PossibleSamples[lowestEntropyIndex]))
                {
                    lowestSamples.Add(i);
                }
            }

            BitArray lowestEntropy = _grid.PossibleSamples[UtilShared.RandomNR.Next(0, lowestSamples.Count - 1)];
            */

            BitArray lowestEntropy = _grid.PossibleSamples[lowestEntropyIndex];


            // b. Apply full list of heuristics over the sample chances (what is the starting proportion of choices?)


            // c. Pick one choice according to the chances supplied by heuristics

            List<int> possibleSamples = UtilShared.ToIntegerList(lowestEntropy);
            // for now, just select a random sample, later we'll add heuristics

            int heuristicSelection = UtilShared.RandomNR.Next(2);
            int selectedSample = 0;
            if (heuristicSelection == 0)
            {
                selectedSample = SelectRandom(possibleSamples);
            }
            else if (heuristicSelection == 1)
            {
                selectedSample = SelectLeastUsed(possibleSamples, _grid.SelectedSamples);
            }

            _grid.SetSample(lowestEntropyIndex, selectedSample);
            setSamples.Add(lowestEntropyIndex);

            // d. Use the sample.propagate(grid) to apply over grid
            _sampleLibrary[selectedSample].Propagate(_grid, lowestEntropyIndex);

            return setSamples;
        }

        int SelectLeastUsed(List<int> possibleSamples, List<int> selectedSamples)
        {
            int smallesAmount = int.MaxValue;
            int leastUsed = int.MinValue;
            foreach (var sample in possibleSamples)
            {
                int amount = selectedSamples.Count(s => s == sample);
                if (amount < smallesAmount)
                {
                    smallesAmount = amount;
                    leastUsed = sample;
                }
            }

            return leastUsed;
        }

        int SelectRandom(List<int> possibleSamples)
        {
            Random rnd = new Random();
            int nextSampleIndex = rnd.Next(1, possibleSamples.Count);
            int selectedSample = possibleSamples[nextSampleIndex];

            return selectedSample;
        }
    }
}
