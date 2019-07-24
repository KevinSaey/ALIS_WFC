using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaveFunctionCollapse.Shared;
namespace WaveFunctionCollapse.Unity
{
    public class ALIS_Tile
    {
        public List<Block> Blocks;
        public Tile GeneratedTile;
        // inherited class. Link blocks to tiles in order to be able to delete them for backtracking!

        public ALIS_Tile()
        {
        }

        public ALIS_Tile (Tile tile)
        {

            GeneratedTile = tile;
            Blocks = new List<Block>();
        }
    }
}