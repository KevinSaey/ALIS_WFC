using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaveFunctionCollapse.Unity
{

    /// <summary>
    /// The possible types of a voxel (Empty, Connection, Block)
    /// </summary>
    public enum VoxelType { Empty, Connection, Block };

    /// <summary>
    /// Representation of a voxel, can be independent of the voxelgrid
    /// </summary>
    public class Voxel
    {
        public Vector3Int Index;
        public VoxelType Type;
        public Vector3Int Orientation;
        public Pattern ParentPattern { get; private set; }
        public Block ParentBlock { get; set; }
        public string Name;
        public GameObject Go;
        public List<Vector3Int> WalkableFaces = new List<Vector3Int>();
        public List<Face> Faces = new List<Face>(6);
        public float Value;
        public Vector3 Center;

        /// <summary>
        /// Instantiate an empty voxel
        /// </summary>
        public Voxel()
        {

        }

        /// <summary>
        /// Instantiate a voxel.
        /// </summary>
        /// <param name="x">X index of the voxel</param>
        /// <param name="y">Y index of the voxel</param>
        /// <param name="z">Z index of the voxel</param>
        /// /// <param name="type">The type of the voxel (Empty, Connection, Block) </param>
        public Voxel(int x, int y, int z, VoxelType type)
        {
            Index = new Vector3Int(x, y, z);
            Type = type;
        }

        /// <summary>
        /// Instantiate a block or connection voxel
        /// </summary>
        /// <param name="x">X index of the voxel</param>
        /// <param name="y">Y index of the voxel</param>
        /// <param name="z">Z index of the voxel</param>
        /// <param name="type">The type of the voxel (Empty, Connection, Block) </param>
        /// <param name="parentPattern">Parrent Pattern of the connection</param>
        /// <param name="orientation">Orientation of the block or connection</param>
        public Voxel(int x, int y, int z, VoxelType type, Vector3Int orientation, Pattern parentPattern)
        {
            Index = new Vector3Int(x, y, z);
            Type = type;
            Orientation = orientation;
            ParentPattern = parentPattern;
            Name = $"x {x}, y {y}, z {z}";
            
        }

        /// <summary>
        /// Instantiate a block voxel with walkable faces assigned
        /// </summary>
        /// <param name="x">X index of the voxel</param>
        /// <param name="y">Y index of the voxel</param>
        /// <param name="z">Z index of the voxel</param>
        /// <param name="type">The type of the voxel (Empty, Connection, Block) </param>
        /// <param name="parentPattern">Parrent Pattern of the connection</param>
        /// <param name="orientation">Orientation of the block</param>
        /// <param name="walkableFaces">The direction vectors between the center of the block and the climable faces eg. (1,0,0)</param>
        public Voxel(int x, int y, int z, VoxelType type, Vector3Int orientation, Pattern parentPattern, List<Vector3Int> walkableFaces)
            : this(x, y, z, type, orientation, parentPattern)
        {
            WalkableFaces = walkableFaces;
        }

        /// <summary>
        /// Make a copy of the voxel
        /// </summary>
        /// <param name="orig">Original voxel</param>
        public void Copy(Voxel orig)
        {
            Type = orig.Type;
            Orientation = orig.Orientation;
            if (Type != VoxelType.Block)
            {
                ParentBlock = orig.ParentBlock;
            }
            ParentPattern = orig.ParentPattern;
            Name = orig.Name;
            WalkableFaces = orig.WalkableFaces;
        }

        /// <summary>
        /// Get the corners of this voxel
        /// </summary>
        /// <returns>List of corners</returns>
        public IEnumerable<Corner> GetCorners()
        {
            for (int y = 0; y <= 1; y++)
                for (int z = 0; z <= 1; z++)
                    for (int x = 0; x <= 1; x++)
                    {
                        yield return GridController._grid.Corners[Index.x + x, Index.y + y, Index.z + z];
                    }
        }

        /// <summary>
        /// Clone all the fields of the voxel to a new voxel
        /// </summary>
        /// <returns>The cloned voxel</returns>
        public Voxel ShallowClone()
        {
            return MemberwiseClone() as Voxel;
        }

        /// <summary>
        /// Remove the voxels gameobject
        /// </summary>
        public void DestroyGoVoxel()
        {
            Debug.Log("The voxel went to the dark side!");
            GameObject.Destroy(Go);
        }
    }
}