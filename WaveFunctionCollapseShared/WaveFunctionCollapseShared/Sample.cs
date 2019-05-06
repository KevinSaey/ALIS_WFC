using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WaveFunctionCollapse.Shared
{
    enum directions { minX = 0, plusX = 1, minY = 2, plusY = 3, minZ = 4, plusZ = 5 }

    public abstract class Sample
    {
        public int Id { get; } // sample null is always an empty sample
        List<List<int>> PossibleConnections { get; }

        public void Propagate<U>(SampleGrid<U> grid, int currentIndex) where U : Sample
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
                Vector3Int neighbourIndex = grid.GetIndexOfPossibleSample(currentIndex) * neighbourIndices[j];
                SharedLogger.Log(neighbourIndex.ToString());
                if (Util.CheckIndex(neighbourIndex, grid.Dimensions))
                {
                    BitArray possibleNeighbours = grid.PossibleSamples[grid.GetPossibleSampleByIndex(neighbourIndex)];

                    //crossreference lists and change neighbour
                    possibleNeighbours = possibleNeighbours.And(Util.ToBitArray(grid.GetConnectionSamples(PossibleConnections[j]),grid.Connections.Count));
                }
            }
        }

        
        
        
    }
}

