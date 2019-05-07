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
        public List<BitArray> PossibleSamples;
        public List<int> SelectedSamples;
        public List<Connection> Connections;
        public List<T> SampleLibrary;


        internal SampleGrid(List<T> sampleLibrary, int dimX, int dimY, int dimZ)
        {
            Connections = new List<Connection>();
            Dimensions = new Vector3IntShared { x = dimX, y = dimY, z = dimZ };
            SampleLibrary = sampleLibrary;

            createPossibleSampleGrid();
        }

        

        void createPossibleSampleGrid()
        {
            PossibleSamples = new List<BitArray>();
            SelectedSamples = new List<int>(PossibleSamples.Count);
            for (int i = 0; i < Dimensions.z * Dimensions.y * Dimensions.x; i++)
            {
                //SharedLogger.Log($"NumberOfSamples {_wfc.Samples.Count}");
                PossibleSamples.Add(new BitArray(SampleLibrary.Count, true));
                SelectedSamples.Add(0);
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
                foreach (var sample in PossibleSamples)
                {
                    if (Entropy(sample) == 0)
                    {
                        SharedLogger.Log("Solution has a conflict");
                        return true;
                    }
                }
                return false;
            }
        }


        int Entropy(BitArray sample)
        {
            int count = 0;
            foreach (bool bln in sample)
            {
                if (bln == true) count++;
            }
            return count;
        }

        public int FindLowestNonZeroEntropy()
        {
            BitArray lowestSample = PossibleSamples.OrderByDescending(o => Entropy(o)).Where(s => Entropy(s) > 1).First();
            //SharedLogger.Log($"Lowest sample index {PossibleSampleGrid.IndexOf(lowestSample)} ");
            return PossibleSamples.IndexOf(lowestSample);
        }

        public List<int> GetConnectionSamples(HashSet<int> connectionID )
        {
            HashSet<Connection> selectedConnections = new HashSet<Connection>();
            foreach (var index in connectionID)
            {
                selectedConnections.Add(Connections[index]);
            }
            
            return selectedConnections.Select(s => s.SampleIDS).SelectMany(s => s).ToList();
        }
    }
}
