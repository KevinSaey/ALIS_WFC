using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using g3;

namespace WaveFunctionCollapse.Shared
{
    enum Directions { minX = 0, plusX = 1, minY = 2, plusY = 3, minZ = 4, plusZ = 5 }

    public abstract class Sample
    {
        public int Id { get; set; } // sample null is always an empty sample
        public List<HashSet<Sample>> PossibleNeighbours;

        public override string ToString()
        {
            return $"Sample - base id: {Id}";
        }

        public Sample()
        { }

        public virtual void DrawSample(int sampleIndex)
        {
        }

        public void InitialisePossibleNeighbours(List<HashSet<Sample>> possibleNeighbours)
        {
            PossibleNeighbours = possibleNeighbours;
        }

        public virtual void Propagate(SampleGrid grid, int currentIndex)
        {
            if (Id == 0)
            {
                SharedLogger.Log("Error: Propogating sample ID 0");
                return;
            }
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
                //SharedLogger.Log($"index {currentIndex} vector {grid.GetIndexOfPossibleSample(currentIndex).ToString()}, neighbour {neighbourIndex.ToString()} ");

                if (UtilShared.CheckIndex(neighbourIndex, grid.Dimensions)) // check if the neighbour is out of bounds
                {
                    var indexToPropagate = grid.GetPossibleSampleByIndex(neighbourIndex);

                    //crossreference lists and change neighbour
                    HashSet<Sample> CurrentPossibleNeighbours = PossibleNeighbours[j];

                    //Crossreference
                    grid.PossibleSamples[indexToPropagate].IntersectWith(CurrentPossibleNeighbours);
                    
                    //assign sample
                    if (grid.PossibleSamples[indexToPropagate].Count == 1)
                    {
                        var sample = grid.PossibleSamples[indexToPropagate].First();
                        if (sample.Id != 0)
                        {
                            var sampleIndex = grid.GetPossibleSampleByIndex(neighbourIndex);
                            grid.SetSample(sampleIndex, sample);
                        }
                    }
                }
            }
            //grid.LogEntropy();
        }
    }
}

