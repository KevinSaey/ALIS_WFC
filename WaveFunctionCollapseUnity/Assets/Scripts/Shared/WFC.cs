using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse.Shared
{
    public class WFC
    {

        public Dictionary<int,Sample> SampleLibrary
        {
            get { return SampleGrid.SampleLibrary; }
            set { SampleGrid.SampleLibrary = value; }
        }


        /*public List<Connection> Connections
        {
            get { return _sampleGrid.Connections; }
            set { _sampleGrid.Connections = value; }
        }*/

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
                return SampleGrid.IsAllDetermined;
            }
        }
        public bool HasContradiction
        {
            get
            {
                return SampleGrid.HasContradiction;
            }
        }

        public SampleGrid SampleGrid;
        Engine _engine;


        public WFC()
        {

        }

        public WFC(int xDimension, int yDimension, int zDimension, Dictionary<int,Sample> samples)
        {
            SharedLogger.Log("Instantiating WFC");
            SampleGrid = new SampleGrid(samples, xDimension, yDimension, zDimension);
            SampleLibrary = samples;

            _engine = new Engine(_heuristics, SampleGrid);
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

        /*public void AddSampleConnection(int connectionID, Sample currentSample)
        {
            if (Connections.Count(c => c.ID == connectionID) == 0)
            {
                Connections.Add(new Connection(connectionID));
            }
            Connections.First(c => c.ID == connectionID).SampleIDS.Add(currentSample.Id);
        }*/

        /*
    public void RemoveEmptyConnections()
    {
        Connections = Connections.Where(s => s.SampleIDS != null).ToList();
    }
    */


        public Vector3IntShared GetIndexOfPossibleSample(int i)
        {
            return SampleGrid.GetIndexOfPossibleSample(i);
        }

        public List<Sample> SelectedSamples
        {
            get
            {
                return SampleGrid.SelectedSamples;
            }
        }
    }
}
