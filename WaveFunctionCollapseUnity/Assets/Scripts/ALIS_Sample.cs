using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveFunctionCollapse.Shared;
using UnityEngine;

namespace WaveFunctionCollapse.Unity
{
    class ALIS_Sample : Sample
    {
        public int Id { get { return _id; } }
        int _id;
        List<List<int>> _possibleConnections = new List<List<int>>();
        public Color Col;

        public List<List<int>> PossibleConnections
        {
            get
            {
                return _possibleConnections;
            }
        }

        //For testing purpouses
        public void SetRandomNeighbours(int NumberOfConnections, WFC<ALIS_Sample> wfc)
        {
            Random.InitState(_id);
            for (int i = 0; i < 6; i++)
            {
                _possibleConnections.Add(new List<int>());
                for (int j = 0; j < 5; j++)
                {
                    wfc.AddSampleConnection(Random.Range(1, NumberOfConnections+1),this);
                }
                _possibleConnections[i] = _possibleConnections[i].Distinct().ToList();
                Col = new Color(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255));
                // Debug.Log(_possibleNeighbours[i].Count);
                // Debug.Log(_possibleNeighbours[i][1].ToString());
            }
        }

        public ALIS_Sample(int id)
        {
            _id = id;
        }

        void Propagate(SampleGrid<ALIS_Sample> grid)
        {
            base.Propagate(grid, 1);
            // add propogation rules
        }
    }


}
