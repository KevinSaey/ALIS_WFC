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
        public List<Tile> Tiles;
        //public List<Connection> Connections;
        public Dictionary<int, Sample> SampleLibrary;
        public List<Domain> Domains;
        public List<WFCState> States;
        public int HistorySteps = 10;
        public int Bottom
        {
            get
            {
                if (Tiles.Any(s => s.Set))
                {
                    var lowestTile = Tiles.First(s => s.Set == true);
                    int bottom = lowestTile.Index.Y;
                    return bottom;
                }
                else
                {
                    return 0;
                }
            }
        }

        internal SampleGrid(Dictionary<int, Sample> sampleLibrary, int dimX, int dimY, int dimZ)
        {
            //Connections = new List<Connection>();
            Domains = new List<Domain>();
            Dimensions = new Vector3IntShared { X = dimX, Y = dimY, Z = dimZ };
            SampleLibrary = sampleLibrary;

            States = new List<WFCState>();
            CreatePossibleSampleGrid();
            //LogIndex();
            //LogEntropy();
        }

        void CreatePossibleSampleGrid()
        {
            Tiles = new List<Tile>();
            for (int i = 0; i < Dimensions.Z * Dimensions.Y * Dimensions.X; i++)
            {
                Tiles.Add(new Tile(i, UtilShared.GetIndexInGrid(i, Dimensions), new HashSet<Sample>(SampleLibrary.Values)));
            }
        }

        public void Reset()
        {
            States.Clear();
            CreatePossibleSampleGrid();
        }

        public Vector3IntShared GetIndexOfPossibleSample(int i)
        {
            return UtilShared.GetIndexInGrid(i, Dimensions); ;
        }

        public int GetPossibleSampleByIndex(Vector3IntShared index)
        {
            return Dimensions.X * Dimensions.Y * index.Z + Dimensions.X * index.Y + index.X;
        }

        public Tile GetPossibleTileByIndex(Vector3IntShared index)
        {
            int tileId = Dimensions.X * Dimensions.Y * index.Z + Dimensions.X * index.Y + index.X;
            return Tiles[tileId];
        }


        public bool IsAllDetermined
        {
            get
            {
                return Tiles.Where(s => s.Enabled).All(s => Entropy(s.PossibleSamples) == 1);
            }
        }

        public bool HasContradiction
        {
            get
            {
                for (int i = 0; i < Tiles.Count; i++)
                {
                    if (Entropy(Tiles[i].PossibleSamples) == 0)
                    {
                        SharedLogger.Log($"Solution has a Contradiction in tile {i}");
                        return true;
                    }
                }

                return false;
            }
        }

        public float Progress
        {
            get
            {
                return (float)NrSetSamples / (float)Tiles.Count(s => s.Enabled);
            }
        }

        public int NrSetSamples
        {
            get
            {
                return Tiles.Count(s => s.Set);
            }
        }

        public int Entropy(HashSet<Sample> sample)
        {
            return sample.Count;
        }

        public List<Tile> FindLowestNonZeroEntropy()
        {
            int lowestEntropy = int.MaxValue;
            List<Tile> lowestTiles = new List<Tile>();
            for (int i = 0; i < Tiles.Count; i++)
            {
                if (Tiles[i].Enabled)
                {
                    int entropy = Entropy(Tiles[i].PossibleSamples);
                    if (entropy < lowestEntropy && entropy > 1)
                    {
                        lowestEntropy = entropy;
                        lowestTiles = new List<Tile>() { Tiles[i] };
                    }
                    else if (entropy == lowestEntropy)
                    {
                        lowestTiles.Add(Tiles[i]);
                    }
                }
            }

            return lowestTiles;
        }

        public void SetSample(Tile tile, Sample selectedSample)
        {
            tile.SelectedSample = selectedSample;
            tile.PossibleSamples = new HashSet<Sample>() { SampleLibrary[0] }; //0 is always an empty sample
            tile.Set = true;

            if (selectedSample.Id != 0)
            {
                selectedSample.DrawSample(tile);
                selectedSample.Propagate(this, tile);
                if (HasContradiction)
                {
                    while (HasContradiction && States.Count > 0)
                    {
                        RevertState();
                    }
                }
                else
                {
                    SaveState(tile, selectedSample);
                }
            }
        }

        public void SaveState(Tile lastTile, Sample selectedSample)
        {
            if (States.Count == 0)
            {
                States.Add(new WFCState(Tiles, lastTile, selectedSample, 0));
            }
            else
            {
                States.Add(new WFCState(States.Last(), Tiles, lastTile, selectedSample, 0));
            }
            if (States.Count > HistorySteps)
            {
                States.RemoveAt(0);
            }
        }

        public void RevertState()
        {
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////// to do
            var lastState = States.Last();
            var lastIndex = lastState.LastUpdatedTile.Id;//
            lastState.Tiles.First(s => s.Id == lastIndex).PossibleSamples.Remove(lastState.SetSample);//
            Tiles = lastState.Tiles;
            States.Remove(lastState);
        }

        public void LogEntropy()
        {
            for (int i = 0; i < Tiles.Count; i++)
            {
                int entropy = Entropy(Tiles[i].PossibleSamples);
                SharedLogger.Log($"Tile: {i} entropy: {entropy}");
            }
        }

        public void LogIndex()
        {

            for (int i = 0; i < Tiles.Count; i++)
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
