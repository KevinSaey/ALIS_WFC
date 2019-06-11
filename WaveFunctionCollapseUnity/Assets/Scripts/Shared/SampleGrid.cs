using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace WaveFunctionCollapse.Shared
{
    public class SampleGrid
    {
        public Vector3IntShared Dimensions { get; private set; }
        public List<HashSet<Sample>> PossibleSamples;
        public List<Sample> SelectedSamples;
        //public List<Connection> Connections;
        public Dictionary<int, Sample> SampleLibrary;
        public List<Domain> Domains;


        internal SampleGrid(Dictionary<int, Sample> sampleLibrary, int dimX, int dimY, int dimZ)
        {
            //Connections = new List<Connection>();
            Domains = new List<Domain>();
            Dimensions = new Vector3IntShared { x = dimX, y = dimY, z = dimZ };
            SampleLibrary = sampleLibrary;

            CreatePossibleSampleGrid();
            //LogIndex();
            //LogEntropy();
        }



        void CreatePossibleSampleGrid()
        {
            PossibleSamples = new List<HashSet<Sample>>();
            SelectedSamples = new List<Sample>(PossibleSamples.Count);
            for (int i = 0; i < Dimensions.z * Dimensions.y * Dimensions.x; i++)
            {
                PossibleSamples.Add(new HashSet<Sample>(SampleLibrary.Values));

                SelectedSamples.Add(SampleLibrary[0]);
            }


        }

        public void Reset()
        {
            CreatePossibleSampleGrid();
        }

        public Vector3IntShared GetIndexOfPossibleSample(int i)
        {
            int newZ = i / (Dimensions.x * Dimensions.y);
            i %= (Dimensions.x * Dimensions.y);
            int newY = i / Dimensions.x;
            i %= Dimensions.x;
            int newX = i;

            return new Vector3IntShared { x = newX, y = newY, z = newZ };
        }

        public int GetPossibleSampleByIndex(Vector3IntShared index)
        {
            return Dimensions.x * Dimensions.y * index.z + Dimensions.x * index.y + index.x;
        }


        public Boolean IsAllDetermined
        {
            get
            {
                return PossibleSamples.All(s => Entropy(s) == 1);
            }
        }

        public Boolean HasContradiction
        {
            get
            {
                for (int i = 0; i < PossibleSamples.Count; i++)
                {
                    if (Entropy(PossibleSamples[i]) == 0)
                    {
                        SharedLogger.Log($"Solution has a Contradiction in tile {i}");
                        return true;
                    }
                }

                return false;
            }
        }


        public int Entropy(BitArray sample)
        {
            return UtilShared.CountBitarrayTrue(sample);
        }

        public int Entropy(bool[] sample)
        {
            return UtilShared.CountBoolarrayTrue(sample);
        }

        public int Entropy(HashSet<Sample> sample)
        {
            return sample.Count;
        }

        public List<int> FindLowestNonZeroEntropy()
        {
            int lowestEntropy = int.MaxValue;
            List<int> lowestIndices = new List<int>();
            for (int i = 0; i < PossibleSamples.Count; i++)
            {
                int entropy = Entropy(PossibleSamples[i]);
                if (entropy < lowestEntropy && entropy > 1)
                {
                    lowestEntropy = entropy;
                    lowestIndices = new List<int>() { i };
                }
                else if (entropy == lowestEntropy)
                {
                    lowestIndices.Add(i);
                }
            }

            return lowestIndices;
        }

        public void SetSample(int index, Sample selectedSample)
        {
            if (selectedSample.Id == 0)
            {
                SharedLogger.Log("Error: Trying to set sample 0 - function SetSample");
                return;
            }
            SelectedSamples[index] = selectedSample;
            PossibleSamples[index] = new HashSet<Sample>() { SampleLibrary[0] }; //0 is always an empty sample
            selectedSample.DrawSample(index);
            selectedSample.Propagate(this, index);

        }

        public void LogEntropy()
        {
            for (int i = 0; i < PossibleSamples.Count; i++)
            {
                int entropy = Entropy(PossibleSamples[i]);
                SharedLogger.Log($"Tile: {i} entropy: {entropy}");
            }
        }

        public void LogIndex()
        {

            for (int i = 0; i < PossibleSamples.Count; i++)
            {
                SharedLogger.Log($"index {i} vector {GetIndexOfPossibleSample(i).ToString()}");
            }
        }

        public int GetIndexFromSampleId(int sampleId)
        {
            int index = int.MaxValue;
            for (int i = 0; i < SampleLibrary.Count; i++)
            {
                if (SampleLibrary[i].Id == sampleId) index = i;

            }
            if (index == int.MaxValue) SharedLogger.Log($"Requested sample id:{sampleId} is not valid - function GetIndexFromSampleID");
            return index;
        }
    }
}
