using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace WaveFunctionCollapse.Unity
{
    public abstract class Pattern
    {
        public List<Voxel> Voxels { get; private set; }
        public Vector3Int PatternOrientation { get; private set; }
        public Vector3Int IndexStartVoxel;

        public Pattern()
        {
            Voxels = new List<Voxel>();
            PatternOrientation = Vector3Int.zero;

            InitialisePattern();
        }

        /// <summary>
        /// Initialise a pattern, setting the block voxels, the connection voxels and their normals in relation to index 0,0,0
        /// </summary>
        public void InitialisePattern()
        {
            // temporary hard coded pattern. This should be imported from rhino
            List<Vector3Int> walkableFaces = new List<Vector3Int> { new Vector3Int(0, 0, 1), new Vector3Int(0, 0, -1) };
            for (int y = 0; y < 6; y++)
                for (int x = -1; x <= 1; x++)
                {
                    Voxels.Add(new Voxel(x, y, 0, VoxelType.Block, PatternOrientation, this, walkableFaces));
                }

            var conVox = new List<Voxel>();
            foreach (var voxel in Voxels.Where(s => s.Type == VoxelType.Block && s.Index.y != 2 && s.Index.y != 3))
            {

                conVox.Add(new Voxel(voxel.Index.x, voxel.Index.y, voxel.Index.z + 1, VoxelType.Connection, new Vector3Int(270, 0, 0), this));
                conVox.Add(new Voxel(voxel.Index.x, voxel.Index.y, voxel.Index.z - 1, VoxelType.Connection, new Vector3Int(90, 0, 0), this));
            }
            Voxels.AddRange(conVox);

            Voxels.Add(new Voxel(-1, 6, 0, VoxelType.Connection, Vector3Int.zero, this));
            Voxels.Add(new Voxel(0, 6, 0, VoxelType.Connection, Vector3Int.zero, this));
            Voxels.Add(new Voxel(1, 6, 0, VoxelType.Connection, Vector3Int.zero, this));

            Voxels.Add(new Voxel(-1, -1, 0, VoxelType.Connection, new Vector3Int(0, 0, 180), this));
            Voxels.Add(new Voxel(0, -1, 0, VoxelType.Connection, new Vector3Int(0, 0, 180), this));
            Voxels.Add(new Voxel(1, -1, 0, VoxelType.Connection, new Vector3Int(0, 0, 180), this));

            for (int i = 0; i < 6; i++)
            {
                if (i != 2 && i != 3)
                {
                    Voxels.Add(new Voxel(-2, i, 0, VoxelType.Connection, Vector3Int.zero, this));
                    Voxels.Add(new Voxel(2, i, 0, VoxelType.Connection, Vector3Int.zero, this));
                }
            }
        }

        public void ShiftPattern(Vector3Int vecShift)
        {
            Voxels.ForEach(v => v.Index += vecShift);
        }
    }


    public class PatternA : Pattern
    {
        /// <summary>
        /// shift the indexes of the pattern with x -1
        /// </summary>
        public PatternA()
        {
            base.InitialisePattern();
            base.ShiftPattern(Vector3Int.left);
        }
    }

    public class PatternB : Pattern
    {
        /// <summary>
        /// shift the indexes of the pattern with x +1
        /// </summary>
        public PatternB()
        {
            base.InitialisePattern();
            base.ShiftPattern(Vector3Int.right);
        }
    }

    public class PatternC : Pattern
    {
        /// <summary>
        /// standard pattern
        /// </summary>
        public PatternC()
        {
            base.InitialisePattern();
        }
    }
}