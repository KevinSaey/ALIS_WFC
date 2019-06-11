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
        public int OrigId;
        public Color Col;
        public int Density;
        public int Type;
        public List<Instance> Instances;
        public string Name;
        static readonly float transparency = .2f;
        readonly ManagerWFC  _managerWFC;

        public ALIS_Sample()
        {
        }

        public ALIS_Sample(int id)
        {
            Id = id;
            OrigId = id;
            PossibleNeighbours = new List<HashSet<Sample>>();
            Col = new Color(Random.Range(0, 255) / 255f, Random.Range(0, 255) / 255f, Random.Range(0, 255) / 255f, transparency);
        }

        public ALIS_Sample(int id, int origID,int density, int type, List<Instance> instances, string name, ManagerWFC managerWFC)
        {
            Id = id;
            OrigId = origID;
            Density = density;
            Type = type;
            Instances = instances;
            Name = name;
            Col = new Color(Random.Range(0, 255) / 255f, Random.Range(0, 255) / 255f, Random.Range(0, 255) / 255f, transparency);
            _managerWFC = managerWFC;
        }

        /*public void AddConnectionsToWFC(WFC<ALIS_Sample> wfc)
        {
            foreach (var connection in PossibleConnections.SelectMany(s => s).Distinct())
            {
                wfc.AddSampleConnection(connection, this);
            }
        }*/

        //For testing purpouses
        public void SetRandomNeighbours(int NumberOfConnections, WFC wfc)
        {
            for (int i = 0; i < 6; i++)
            {
                PossibleNeighbours.Add(new HashSet<Sample>());
                for (int j = 0; j < 2; j++)
                {
                    int nextConnection = Random.Range(1, NumberOfConnections + 1);
                    PossibleNeighbours[i].Add(wfc.SampleLibrary[nextConnection]);
                    //wfc.AddSampleConnection(nextConnection, this);
                }
            }
        }

        public override void Propagate(SampleGrid grid, int index)
        {
            base.Propagate(grid, index);
        }

        public override void DrawSample(int sampleIndex)
        {
            _managerWFC.DrawSample(sampleIndex);
        }
    }


}
