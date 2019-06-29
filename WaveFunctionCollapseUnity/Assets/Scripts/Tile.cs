using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace WaveFunctionCollapse.Shared
{
    public class Tile
    {
        public bool Enabled = true;
        public bool Set = false;
        public Vector3IntShared Index;
        public HashSet<Sample> PossibleSamples;
        public Sample SelectedSample;
        public int Id;

        public Tile(int id, Vector3IntShared index, HashSet<Sample> possibleSamples)
        {
            Index = index;
            PossibleSamples = possibleSamples;
        }
    }
}
