using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveFunctionCollapse.Shared;
using UnityEngine;

namespace WaveFunctionCollapse.Unity
{
    public class ALIS_Sample : Sample
    {
        public Color Col;
        public int Density;
        public int Type;
        public List<Instance> Instances;

        public ALIS_Sample(int id)
        {
            Id = id;
            PossibleConnections = new List<HashSet<int>>();
            Col = new Color(Random.Range(0, 255) / 255f, Random.Range(0, 255) / 255f, Random.Range(0, 255) / 255f, 0.8f);
        }

        public ALIS_Sample(int id, int density, int type, List<HashSet<int>> possibleConnecitons, List<Instance> instances)
        {
            Id = id;
            Density = density;
            Type = type;
            PossibleConnections = possibleConnecitons;
            Instances = instances;
            Col = new Color(Random.Range(0, 255) / 255f, Random.Range(0, 255) / 255f, Random.Range(0, 255) / 255f, 0.8f);

        }

        public void AddConnectionsToWFC(WFC<ALIS_Sample> wfc)
        {
            foreach (var connection in PossibleConnections.SelectMany(s => s).Distinct())
            {
                wfc.AddSampleConnection(connection, this);
            }
        }

        //For testing purpouses
        public void SetRandomNeighbours(int NumberOfConnections, WFC<ALIS_Sample> wfc)
        {
            for (int i = 0; i < 6; i++)
            {
                PossibleConnections.Add(new HashSet<int>());
                for (int j = 0; j < 2; j++)
                {
                    int nextConnection = Random.Range(1, NumberOfConnections + 1);
                    PossibleConnections[i].Add(nextConnection);
                    wfc.AddSampleConnection(nextConnection, this);
                }
            }
        }



        void Propagate(SampleGrid<ALIS_Sample> grid, int index)
        {
            base.Propagate(grid, index);
            // add propogation rules
        }
    }


}
