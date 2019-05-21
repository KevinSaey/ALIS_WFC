using System.Collections.Generic;
using UnityEngine;

namespace WaveFunctionCollapse.Unity
{
    public class Corner // stolen from Vicente
    {
        public Vector3 Position;
        public Vector3Int Index;
        protected Grid3D _grid;

        public Corner(Vector3Int index, Grid3D grid)
        {
            _grid = grid;
            Index = index;
            Position = _grid.Corner + new Vector3(index.x, index.y, index.z)*grid.VoxelSize;
        }

        public Corner(Corner corner) : this(corner.Index, corner._grid)
        {
            _grid.Corners[corner.Index.x, corner.Index.y, corner.Index.z] = this;
        }

        public IEnumerable<Voxel> GetConnectedVoxels()
        {
            for (int zi = -1; zi <= 0; zi++)
            {
                int z = zi + Index.z;
                if (z == -1 || z == _grid.Size.z) continue;

                for (int yi = -1; yi <= 0; yi++)
                {
                    int y = yi + Index.y;
                    if (y == -1 || y == _grid.Size.y) continue;

                    for (int xi = -1; xi <= 0; xi++)
                    {
                        int x = xi + Index.x;
                        if (x == -1 || x == _grid.Size.x) continue;

                        var i = new Vector3Int(x, y, z);

                        yield return _grid.Voxels[x, y, z];
                    }
                }
            }
        }
    }
}
