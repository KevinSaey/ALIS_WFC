using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;


namespace WaveFunctionCollapse.Shared
{
    public class SampleGrid<T> where T : Sample
    {
        public Vector3Int Dimensions { get; private set; }
        public List<List<int>> PossibleSampleGrid { get; set; } = new List<List<int>>();
        List<List<List<T>>> _samples;

        WFC<T> _wfc;

        internal SampleGrid(WFC<T> wfc, int dimX, int dimY, int dimZ)
        {
            Dimensions = new Vector3Int { x = dimX, y = dimY, z = dimZ };
            _wfc = wfc;

            createPossibleSampleGrid();
        }

        void CreateSampleGrid()
        {
            for (int z = 0; z < Dimensions.z; z++)
            {
                for (int y = 0; y < Dimensions.y; y++)
                {
                    for (int x = 0; x < Dimensions.x; x++)
                    {

                    }
                }
            }
        }

        void createPossibleSampleGrid()
        {
            for (int i = 0; i < Dimensions.z * Dimensions.y * Dimensions.x; i++)
            {
                PossibleSampleGrid.Add(_wfc.Samples.Select(s => s.Id).ToList());
            }

        }

        public Vector3Int GetIndexOfPossibleSample(int i)
        {
            int newZ = i / (Dimensions.x * Dimensions.y);
            i %= (Dimensions.x * Dimensions.y);
            int newY = i / Dimensions.x;
            i %= Dimensions.x;
            int newX = i;

            return new Vector3Int { x = newX, y = newY, z = newZ };
        }

        public int GetPossibleSampleByIndex(Vector3Int index)
        {
            return Dimensions.x * Dimensions.y * index.z + Dimensions.x * index.y + index.x;
        }

        
        public Boolean IsAllDetermined() {
            return PossibleSampleGrid.All(s => s.Count == 1);
        }

        public Boolean HasConflict()
        {
            foreach (var sample in PossibleSampleGrid)
            {
                if (sample.Count == 0) return true;
            }
            return false;
        }

        public int FindLowestNonZeroEntropy()
        {
            List<int> lowestSample = PossibleSampleGrid.OrderByDescending(o => o.Count).Where(s => s.Count > 1).First();
            return PossibleSampleGrid.IndexOf(lowestSample);
        }
    }
}
