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
        public int Id { get; set; } // sample null is always an empty sample
        public List<HashSet<int>> PossibleConnections;

        public void Propagate<U>(SampleGrid<U> grid, int currentIndex) where U : Sample
        {
            SharedLogger.Log("Propogating");
            // Neighbours propagation here
            List<Vector3IntShared> neighbourIndices = new List<Vector3IntShared> {
                new Vector3IntShared { x=-1, y=0, z=0 },
                new Vector3IntShared { x=1, y=0, z=0 },
                new Vector3IntShared { x=0, y=-1, z=0 },
                new Vector3IntShared { x=0, y=1, z=0 },
                new Vector3IntShared { x=0, y=0, z=-1 },
                new Vector3IntShared { x=0, y=0, z=1 } };


            for (int j = 0; j < 6; j++)
            {
                Vector3IntShared neighbourIndex = grid.GetIndexOfPossibleSample(currentIndex) + neighbourIndices[j];

                if (UtilShared.CheckIndex(neighbourIndex, grid.Dimensions))
                {
                    SharedLogger.Log(neighbourIndex.ToString());
                    SharedLogger.Log($"possibleConnection {PossibleConnections[j].Count}");
                    BitArray possibleNeighbours = grid.PossibleSamples[grid.GetPossibleSampleByIndex(neighbourIndex)];
                    //var t = PossibleConnections[j];
                    //grid.GetConnectionSamples(PossibleConnections[j]);
                    //crossreference lists and change neighbour

                    //possibleNeighbours.And(UtilShared.ToBitArray(grid.GetConnectionSamples(PossibleConnections[j]), grid.SampleLibrary.Count));
                    SharedLogger.Log($"{UtilShared.CountBitarrayTrue(UtilShared.ToBitArray(grid.GetConnectionSamples(PossibleConnections[j]), grid.SampleLibrary.Count))} possible neighbours");
                }
            }
        }




    }
}

