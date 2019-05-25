using System;
using System.Collections.Generic;
using System.Text;

namespace WaveFunctionCollapse.Shared
{
    public class Domain
    {
        public string Name;
        public int ID;
        public HashSet<int> TileIndices;
        public List<int> PossibleSamples;

        public Domain(string name, int id, List<int> possibleSamples)
        {
            TileIndices = new HashSet<int>();
            Name = name;
            ID = id;
            PossibleSamples = possibleSamples;
        }

        public void AddTileIndex(int index)
        {
            TileIndices.Add(index);
        }
    }
}
