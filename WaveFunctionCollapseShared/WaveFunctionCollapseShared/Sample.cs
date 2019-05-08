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

                if (UtilShared.CheckIndex(neighbourIndex, grid.Dimensions)) // check if the neighbour is out of bounds
                {
                    BitArray possibleNeighbours = grid.PossibleSamples[grid.GetPossibleSampleByIndex(neighbourIndex)];

                    //crossreference lists and change neighbour
                    if (UtilShared.CountBitarrayTrue(possibleNeighbours) != 1) // if statement becomes obsolete if we actually have working patterns
                    {
                        possibleNeighbours.And(UtilShared.ToBitArray(PossibleConnections[j], grid.SampleLibrary.Count));
                    }
                    else
                    {
                        //assign sample
                        var index = UtilShared.GetOneTrue(possibleNeighbours);
                        if (index != 0)
                            grid.SetSample(grid.GetPossibleSampleByIndex(neighbourIndex), index);
                    }

                }

            }
            //grid.ShowEntropy();
        }




    }
}

