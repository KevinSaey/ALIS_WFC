using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveFunctionCollapse;

namespace TestProject
{
    class TestSample : Sample
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
                    Random rnd = new Random();
                    _possibleNeighbours[i].Add(rnd.Next(0, NumberOfSamples));
                }
            }
        }

        public TestSample(int id)
        {
            _id = id;
        }

        void Propagate(SampleGrid<TestSample> grid)
        {
            base.Propagate(grid, 1);
            // add propogation rules
        }
    }


}
