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
        List<IHeuristic<T>> _heuristics;
        SampleGrid<T> _grid;
        int _counter = 0;

        public Engine( List<IHeuristic<T>> heuristics, SampleGrid<T> grid)
        {
            _heuristics = heuristics;
            _grid = grid;
            
        }

        public void Execute()
        {
            //SetBoundryCondition(1, true, true, false, false, true, true);
            //PropogateDomains();
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
            //_grid.LogEntropy();
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

            //lowestEntropyIndex = _grid.FindLowestNonZeroEntropy();
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

            bool[] lowestEntropy = _grid.PossibleSamples[lowestEntropyIndex];


            // b. Apply full list of heuristics over the sample chances (what is the starting proportion of choices?)


            // c. Pick one choice according to the chances supplied by heuristics

            List<int> possibleSamples = UtilShared.ToIntegerList(lowestEntropy);
            // for now, just select a random sample, later we'll add heuristics

            int heuristicSelection = UtilShared.RandomNR.Next(2);

            //heuristicSelection = 1; // overwrite, delete for actual random selection
            T selectedSample=_grid.SampleLibrary[0];
            if (heuristicSelection == 0)
            {
                selectedSample = _grid.SampleLibrary[SelectRandom(possibleSamples)];
            }
            else if (heuristicSelection == 1)
            {
                selectedSample = SelectLeastUsed(possibleSamples);
            }

            _grid.SetSample(lowestEntropyIndex, selectedSample);
            setSamples.Add(lowestEntropyIndex);

            // d. Use the sample.propagate(grid) to apply over grid
            //_sampleLibrary[selectedSample].Propagate(_grid, lowestEntropyIndex);
            //_grid.LogEntropy();
            return setSamples;
        }

        T SelectLeastUsed(List<int> possibleSamples)
        {
            int smallesAmount = int.MaxValue;
            T leastUsed = _grid.SampleLibrary[0];
            foreach (var sample in possibleSamples.Select(s => _grid.SampleLibrary[s]))
            {
                int amount = _grid.SelectedSamples.Count(s => s == sample);
                if (amount < smallesAmount)
                {
                    smallesAmount = amount;
                    leastUsed = sample;
                }
            }
            if (smallesAmount != int.MaxValue) return leastUsed;
            return null;
        }

        int SelectRandom(List<int> possibleSamples)
        {
            int nextSampleIndex = UtilShared.RandomNR.Next(possibleSamples.Count);
            int selectedSample = possibleSamples[nextSampleIndex];

            return selectedSample;
        }

        void SetBoundryCondition(int boundrySample, bool minX, bool plusX, bool minY, bool plusY, bool minZ, bool plusZ)
        {
            var boundryDomain = new Domain("Boundry", 1, new List<int> { boundrySample });
            
            for (int i = 0; i < _grid.PossibleSamples.Count; i++)
            {
                if (minX && _grid.GetIndexOfPossibleSample(i).x == 0) boundryDomain.AddTileIndex(i);
                if (minY && _grid.GetIndexOfPossibleSample(i).y == 0) boundryDomain.AddTileIndex(i);
                if (minZ && _grid.GetIndexOfPossibleSample(i).z == 0) boundryDomain.AddTileIndex(i);

                if (plusX && _grid.GetIndexOfPossibleSample(i).x == _grid.Dimensions.x-1) boundryDomain.AddTileIndex(i);
                if (plusY && _grid.GetIndexOfPossibleSample(i).y == _grid.Dimensions.y-1) boundryDomain.AddTileIndex(i);
                if (plusZ && _grid.GetIndexOfPossibleSample(i).z == _grid.Dimensions.z-1) boundryDomain.AddTileIndex(i);
            }

            // check inside polyline condition to add empty

            _grid.Domains.Add(boundryDomain);
        }

        /*void PropogateDomains()
        {
            foreach (var domain in _grid.Domains)
            {
                foreach (var tileIndex in domain.TileIndices)
                {
                    _grid.PossibleSamples[tileIndex]=_grid.PossibleSamples[tileIndex].And(UtilShared.ToBoolArray(domain.PossibleSamples.ToList(),_grid.SampleLibrary.Count));
                    if (domain.PossibleSamples.Count == 1) _grid.SetSample(tileIndex, domain.PossibleSamples[0]);
                }
            }
        }*/
    }
}
