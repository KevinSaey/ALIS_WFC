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
        List<List<int>> _possibleNeighbours = new List<List<int>>();
        public Color Col;

        public List<List<int>> PossibleNeighbours
        {
            get
            {
                return _possibleNeighbours;
            }
        }

        //For testing purpouses
        public void SetRandomNeighbours(int NumberOfSamples)
        {

            for (int i = 0; i < 6; i++)
            {
                _possibleNeighbours.Add(new List<int>());
                for (int j = 0; j < 5; j++)
                {
                    _possibleNeighbours[i].Add(Random.Range(0, NumberOfSamples));
                }
                _possibleNeighbours[i] = _possibleNeighbours[i].Distinct().ToList();
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
