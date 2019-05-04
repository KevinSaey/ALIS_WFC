using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveFunctionCollapse.Shared.Utilities;

namespace WaveFunctionCollapse.Shared
{
    enum directions { minX = 0, plusX = 1, minY = 2, plusY = 3, minZ = 4, plusZ = 5 }

    public abstract class Sample
    {
        public int Id { get; }
        List<List<int>> PossibleNeighbours { get; }

        protected void Propagate<U>(SampleGrid<U> grid, int currentIndex) where U : Sample
        {
            // Neighbours propagation here
            List<Vector3Int> neighbourIndices = new List<Vector3Int> {
                new Vector3Int { x=-1, y=0, z=0 },
                new Vector3Int { x=1, y=0, z=0 },
                new Vector3Int { x=0, y=-1, z=0 },
                new Vector3Int { x=0, y=1, z=0 },
                new Vector3Int { x=0, y=0, z=-1 },
                new Vector3Int { x=0, y=0, z=1 } };

            for (int j = 0; j < 6; j++)
            {
                var neighbour = grid.PossibleSampleGrid[grid.GetPossibleSampleByIndex(grid.GetIndexOfPossibleSample(currentIndex) * neighbourIndices[j])];
                //crossreference lists and change neighbour
                neighbour = Util.CrossreferenceIncludeList(neighbour, PossibleNeighbours[j]);
            }
        }
    }
}

