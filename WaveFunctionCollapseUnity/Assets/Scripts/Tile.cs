using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace WaveFunctionCollapse.Shared
{
    public class Tile: ICloneable<Tile>
    {
        public bool Enabled = true;
        public bool Set = false;
        public Vector3IntShared Index;
        public HashSet<Sample> PossibleSamples;
        public Sample SelectedSample;
        public int Id;

        public Tile()
        { }

        public Tile(int id, Vector3IntShared index, HashSet<Sample> possibleSamples)
        {
            Id = id;
            Index = index;
            PossibleSamples = possibleSamples;
        }

        public Tile(int id, Vector3IntShared index, HashSet<Sample> possibleSamples, Sample selectedSample, bool set, bool enabled)
        {
            Id = id;
            Index = index;
            PossibleSamples = possibleSamples;
            SelectedSample = selectedSample;
            Set = set;
            Enabled = enabled;
        }

        public Tile Clone()
        {
            return new Tile(Id, Index, new HashSet<Sample>(PossibleSamples),SelectedSample,Set,Enabled);
        }
    }
}
