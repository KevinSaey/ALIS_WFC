using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using g3;

namespace WaveFunctionCollapse.Shared
{
    enum directions { minX = 0, plusX = 1, minY = 2, plusY = 3, minZ = 4, plusZ = 5 }

    public abstract class Sample
    {
        public int Id { get; set; } // sample null is always an empty sample
        public List<HashSet<int>> PossibleNeighbours;

        public Sample()
        { }

        public void DrawSample<U>(U sample, int sampleIndex) where U : Sample
        {
        }

        public List<int> Propagate<U>(SampleGrid<U> grid, int currentIndex) where U : Sample
        {
            List<int> setSamples = new List<int>();
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
                    bool[] NeighbourSamples = grid.PossibleSamples[indexToPropagate];

                    //crossreference lists and change neighbour
                    if (UtilShared.CountBoolarrayTrue(NeighbourSamples) != 1) // if statement becomes obsolete if we actually have working patterns
                    {
                        HashSet<int> samplesToMatch = PossibleNeighbours[j];

                        /*foreach (var connection in PossibleConnections[j])
                            samplesToMatch.AddRange(
                                grid.Connections.First(s => s.ID == connection)
                                .SampleIDS.Where(s => s != grid.GetIndexFromSampleId(this.Id)));*/

                        grid.PossibleSamples[indexToPropagate] = NeighbourSamples.And(UtilShared.ToBoolArray(samplesToMatch, grid.SampleLibrary.Count));
                    }
                    else
                    {
                        //assign sample
                        var index = UtilShared.GetOneTrue(NeighbourSamples);
                        if (index != 0)
                        {
                            var sampleIndex = grid.GetPossibleSampleByIndex(neighbourIndex);
                            grid.SetSample(sampleIndex, grid.SampleLibrary[index]);
                            //setSamples.Add(sampleIndex); THIS IS DOING NOTHING
                        }
                    }
                }
            }
            //grid.ShowEntropy();
            return setSamples;
        }
    }
}

