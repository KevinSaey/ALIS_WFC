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
        public string Name;
        static float transparency = .2f;

        public ALIS_Sample(int id)
        {
            Id = id;
            PossibleNeighbours = new List<HashSet<int>>();
            Col = new Color(Random.Range(0, 255) / 255f, Random.Range(0, 255) / 255f, Random.Range(0, 255) / 255f, transparency);
        }

        public ALIS_Sample(int id, int density, int type, List<HashSet<int>> possibleConnecitons, List<Instance> instances, string name)
        {
            Id = id;
            Density = density;
            Type = type;
            PossibleNeighbours = possibleConnecitons;
            Instances = instances;
            Name = name;
            Col = new Color(Random.Range(0, 255) / 255f, Random.Range(0, 255) / 255f, Random.Range(0, 255) / 255f, transparency);

        }

        /*public void AddConnectionsToWFC(WFC<ALIS_Sample> wfc)
        {
            foreach (var connection in PossibleConnections.SelectMany(s => s).Distinct())
            {
                wfc.AddSampleConnection(connection, this);
            }
        }*/

        //For testing purpouses
        public void SetRandomNeighbours(int NumberOfConnections, WFC<ALIS_Sample> wfc)
        {
            for (int i = 0; i < 6; i++)
            {
                PossibleNeighbours.Add(new HashSet<int>());
                for (int j = 0; j < 2; j++)
                {
                    int nextConnection = Random.Range(1, NumberOfConnections + 1);
                    PossibleNeighbours[i].Add(nextConnection);
                    //wfc.AddSampleConnection(nextConnection, this);
                }
            }
        }

        List<int> Propagate(SampleGrid<ALIS_Sample> grid, int index)
        {
            return base.Propagate(grid, index);
        }
    }


}
