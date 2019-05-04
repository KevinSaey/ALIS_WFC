using System;
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
        List<int>[] _possibleNeighbours = new List<int>[6];
        public List<int>[] PossibleNeighbours
        {
            get
            {
                return _possibleNeighbours;
            }
        }

        public void SetRandomNeighbours(int NumberOfSamples)
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    System.Random rnd = new System.Random();
                    _possibleNeighbours[i].Add(rnd.Next(0, NumberOfSamples));
                }
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
