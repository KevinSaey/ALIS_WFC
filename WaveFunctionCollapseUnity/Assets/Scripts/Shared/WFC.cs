using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse.Shared
{
    public class WFC
    {

        public Dictionary<int, Sample> SampleLibrary
        {
            get { return _sampleGrid.SampleLibrary; }
            set { _sampleGrid.SampleLibrary = value; }
        }
        public float Progress
        {
            get { return _sampleGrid.Progress; }
        }
        public int NrSetSamples
        {
            get { return _sampleGrid.NrSetSamples; }
        }


        List<IHeuristic> _heuristics;

        public List<IHeuristic> Heuristics
        {
            get { return _heuristics; }
            set { _heuristics = value; }
        }

        public bool IsAllDetermined
        {
            get
            {
                return _sampleGrid.IsAllDetermined;
            }
        }
        public bool HasContradiction
        {
            get
            {
                return _sampleGrid.HasContradiction;
            }
        }

        SampleGrid _sampleGrid;
        Engine _engine;


        public WFC()
        {

        }

        public WFC(int xDimension, int yDimension, int zDimension, Dictionary<int, Sample> samples)
        {
            SharedLogger.Log("Instantiating WFC");
            _sampleGrid = new SampleGrid(samples, xDimension, yDimension, zDimension);
            SampleLibrary = samples;

            _engine = new Engine(_heuristics, _sampleGrid);
            SharedLogger.Log("WFC Instantiated");
        }

        public void Step(int amount)
        {
            _engine.Step(amount);
        }

        public void Execute()
        {
            _engine.Execute();
        }

        public void Reset()
        {
            _sampleGrid.Reset();
            _engine.Reset();
        }

        public void SetSample(Tile tile, Sample selectedSample)
        {
            _sampleGrid.SetSample(tile, selectedSample);
        }
       
        public Vector3IntShared GetIndexOfPossibleSample(int i)
        {
            return _sampleGrid.GetIndexOfPossibleSample(i);
        }

        public List<Tile> Tiles
        {
            get
            {
                return _sampleGrid.Tiles;
            }
        }
        public void DisableTiles(HashSet<int>tilesToDisable)
        {
            _engine.DisableTiles(tilesToDisable);
        }
    }
}
