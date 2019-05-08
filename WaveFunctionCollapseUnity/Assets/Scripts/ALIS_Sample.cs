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
        public Color Col;

        //public List<HashSet<int>> PossibleConnections;

        //For testing purpouses
        public void SetRandomNeighbours(int NumberOfConnections, WFC<ALIS_Sample> wfc)
        {
            Random.InitState(Id * System.Guid.NewGuid().GetHashCode());
            for (int i = 0; i < 6; i++)
            {
                PossibleConnections.Add(new HashSet<int>());
                for (int j = 0; j < 4; j++)
                {
                    int nextConnection = Random.Range(1, NumberOfConnections + 1);
                    PossibleConnections[i].Add(nextConnection);
                    wfc.AddSampleConnection(nextConnection, this);
                }
                Col = new Color(Random.Range(0, 255) / 255f, Random.Range(0, 255) / 255f, Random.Range(0, 255) / 255f,0.5f);
            }
        }

        public ALIS_Sample(int id)
        {
            Id = id;
            PossibleConnections = new List<HashSet<int>>();
        }


        void Propagate(SampleGrid<ALIS_Sample> grid, int index)
        {
            base.Propagate(grid, index);
            // add propogation rules
        }
    }


}
