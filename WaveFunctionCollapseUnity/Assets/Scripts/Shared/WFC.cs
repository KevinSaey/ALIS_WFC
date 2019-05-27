using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse.Shared
{
    public class WFC<T> where T : Sample
    {

        public List<T> SampleLibrary
        {
            get { return _sampleGrid.SampleLibrary; }
            set { _sampleGrid.SampleLibrary = value; }
        }


        /*public List<Connection> Connections
        {
            get { return _sampleGrid.Connections; }
            set { _sampleGrid.Connections = value; }
        }*/

        List<IHeuristic<T>> _heuristics;

        public List<IHeuristic<T>> Heuristics
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
        public bool HasConflict
        {
            get
            {
                return _sampleGrid.HasConflict;
            }
        }

        SampleGrid<T> _sampleGrid;
        Engine<T> _engine;


        public WFC()
        {

        }

        public WFC(int xDimension, int yDimension, int zDimension, List<T> samples)
        {
            SharedLogger.Log("Instantiating WFC");
            _sampleGrid = new SampleGrid<T>(samples, xDimension, yDimension, zDimension);
            SampleLibrary = samples;

            _engine = new Engine<T>(SampleLibrary, _heuristics, _sampleGrid);
            SharedLogger.Log("WFC Instantiated");
        }

        public List<int> Step(int amount)
        {
            return _engine.Step(amount);
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
            return _sampleGrid.GetIndexOfPossibleSample(i);
        }

        public List<int> SelectedSamples
        {
            get
            {
                return _sampleGrid.SelectedSamples;
            }
        }
    }
}
