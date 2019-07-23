using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaveFunctionCollapse.Shared
{
    public class WFCState
    {
        public WFCState PreviousState;
        public List<Tile> Tiles;
        public Tile LastUpdatedTile;
        public Sample SetSample;
        public int Step =0;

        public WFCState(WFCState previousState, List<Tile> tiles, Tile lastUpdatedTile, Sample setSample, int step)
        {
            PreviousState = previousState;
            Tiles = CloneTiles(tiles);
            LastUpdatedTile = lastUpdatedTile.Clone();
            SetSample = setSample;
            Step = step;
        }

        public WFCState( List<Tile> tiles, Tile lastUpdatedTile, Sample setSample, int step)
        {
            Tiles = CloneTiles(tiles);
            LastUpdatedTile = lastUpdatedTile.Clone();
            SetSample = setSample;
            Step = step;
        }

        List<Tile> CloneTiles(List<Tile> tilesToClone)
        {
            List<Tile> clonedTiles = new List<Tile>();
            for (int i = 0; i < tilesToClone.Count; i++)
            {
                clonedTiles.Add(tilesToClone[i].Clone());
            }
            return clonedTiles;
        }
    }
}
