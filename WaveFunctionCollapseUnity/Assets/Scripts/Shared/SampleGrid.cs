﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace WaveFunctionCollapse.Shared
{
    public class SampleGrid<T> where T : Sample
    {
        public Vector3IntShared Dimensions { get; private set; }
        public List<bool[]> PossibleSamples;
        public List<T> SelectedSamples;
        //public List<Connection> Connections;
        public List<T> SampleLibrary;
        public List<Domain> Domains;


        internal SampleGrid(List<T> sampleLibrary, int dimX, int dimY, int dimZ)
        {
            //Connections = new List<Connection>();
            Domains = new List<Domain>();
            Dimensions = new Vector3IntShared { x = dimX, y = dimY, z = dimZ };
            SampleLibrary = sampleLibrary;

            createPossibleSampleGrid();
            //LogIndex();
            //LogEntropy();
        }



        void createPossibleSampleGrid()
        {
            PossibleSamples = new List<bool[]>();
            SelectedSamples = new List<T>(PossibleSamples.Count);
            for (int i = 0; i < Dimensions.z * Dimensions.y * Dimensions.x; i++)
            {
                var boolArray = new bool[SampleLibrary.Count];
                PossibleSamples.Add(boolArray.SetAll(true));
                SelectedSamples.Add(SampleLibrary[0]);
            }


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



        public Boolean HasConflict
        {
            get
            {
                for (int i = 0; i < PossibleSamples.Count; i++)
                {
                    if (Entropy(PossibleSamples[i]) == 0)
                    {
                        SharedLogger.Log($"Solution has a conflict in tile {i}");
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

        public int FindLowestNonZeroEntropy()
        {
            int lowestEntropy = SampleLibrary.Count;
            int lowestIndex = -1;
            for (int i = 0; i < PossibleSamples.Count; i++)
            {
                int entropy = Entropy(PossibleSamples[i]);
                if (entropy < lowestEntropy && entropy > 1)
                {
                    lowestEntropy = entropy;
                    lowestIndex = i;
                }
            }
            if (lowestIndex == -1) lowestIndex = 0;

            return lowestIndex;
            /*
            BitArray lowestSample = PossibleSamples.OrderByDescending(o => Entropy(o)).Where(s => Entropy(s) > 1).First();
            return PossibleSamples.IndexOf(lowestSample);*/
        }

        public void SetSample(int index, T selectedSample)
        {
            SelectedSamples[index] =  selectedSample;
            PossibleSamples[index] = UtilShared.SetFalseBut(PossibleSamples[index], 0); //0 is always an empty sample
            selectedSample.DrawSample(selectedSample, index);
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

        /*public List<int> GetConnectionSamples(HashSet<int> connectionID)
        {
            HashSet<Connection> selectedConnections = new HashSet<Connection>();
            foreach (var index in connectionID)
            {
                selectedConnections.Add(Connections[index]);
            }

            return selectedConnections.Select(s => s.SampleIDS).SelectMany(s => s).ToList();
        }*/

        public int GetIndexFromSampleId(int sampleId)
        {
            int index = int.MaxValue;
            for (int i = 0; i < SampleLibrary.Count; i++)
            {
                if (SampleLibrary[i].Id == sampleId) index = i;
                
            }
            if (index == int.MaxValue)SharedLogger.Log($"Requested sample id:{sampleId} is not valid - function GetIndexFromSampleID");
            return index;
        }
    }
}
