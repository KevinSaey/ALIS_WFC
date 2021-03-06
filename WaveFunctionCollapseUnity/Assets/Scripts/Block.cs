﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEngine.Mathf;

namespace WaveFunctionCollapse.Unity
{
    /// <summary>
    /// Rotates and translates a pattern
    /// </summary>
    public class Block

    {
        Grid3D _grid;
        public Pattern Pattern;
        public Vector3Int ZeroIndex;
        public List<Voxel> BlockVoxels;
        public GameObject goBlockParent;

        Vector3Int _rotation;

        /// <summary>
        /// This will initiale a block within the voxel grid on a given location with a given rotation according to a preset pattern
        /// </summary>
        /// <param name="pattern">The pattern that fits the block</param>
        /// <param name="zeroIndex">The index of the blocks zero index (according to the pattern) within the voxel grid</param>
        /// <param name="newRotation">The rotation of the block around the x,y,z axis in degrees</param>
        /// <param name="grid">The global voxel grid</param>
        public Block(Pattern pattern, Vector3Int zeroIndex, Vector3Int newRotation, Grid3D grid)
        {
            Pattern = pattern;
            ZeroIndex = zeroIndex;
            _grid = grid;

            _rotation = newRotation;

            BlockVoxels = GetVoxels().ToList();
            BlockVoxels.ForEach(f => f.Index += zeroIndex);
            BlockVoxels.ForEach(f => f.ParentBlock = this);//translation
            RotateBlockVoxels();
        }

        /// <summary>
        /// This will initiale a block within the voxel grid with the location and orientation of a conneciton voxel according to a preset pattern
        /// </summary>
        /// <param name="pattern">The pattern that fits the block</param>
        /// <param name="connPoint">The connectionvoxel of this pattern. Will be used as zero index</param>
        /// <param name="grid">The global voxel grid</param>
        public Block(Pattern pattern, Voxel connPoint, Grid3D grid) : this(pattern, connPoint.Index, connPoint.Orientation, grid)
        {

        }

        /// <summary>
        /// Create a copy of the pattern voxels with the given location and orientation
        /// </summary>
        /// <returns>The voxels of the block</returns>
        IEnumerable<Voxel> GetVoxels()
        {
            foreach (var voxel in Pattern.Voxels)
            {
                var copyVox = voxel.ShallowClone();
                Util.TryOrientIndex(copyVox.Index, Vector3Int.zero, Quaternion.Euler(_rotation),_grid, out var rotated);

                copyVox.Index = rotated;


                //copyVox.Index = RotateVector(copyVox.Index);
                copyVox.WalkableFaces?.ForEach(s => RotateVector(s));
                yield return copyVox;
            }
        }

        

       

        /// <summary>
        /// Rotate a vector according the the block rotation (only works for axis aligned orientations)
        /// </summary>
        /// <param name="vec">The Vector3Int to rotate</param>
        /// <returns>the rotated vector</returns>
        Vector3Int RotateVector(Vector3Int vec)
        {
            //reduce the rotations to 360 degrees (this limits the maximum size of the voxelgrid)
            vec.x = vec.x % 360;
            vec.y = vec.y % 360;
            vec.z = vec.z % 360;

            // x rotation
            Vector3Int[] rotation_x = new Vector3Int[]
            {
            vec,
            new Vector3Int(vec.x, -vec.z, vec.y),
            new Vector3Int(vec.x, -vec.y, -vec.z),
            new Vector3Int(vec.x, vec.z, -vec.y)//
            };

            vec = rotation_x[_rotation.x / 90 % 4];

            // y rotation
            Vector3Int[] rotation_y = new Vector3Int[]
            {
            vec,
            new Vector3Int(vec.z, vec.y, -vec.x),
            vec,//new Vector3Int(-vec.x, vec.y, -vec.z),
            new Vector3Int(-vec.z, vec.y, vec.x)
            };

            vec = rotation_y[_rotation.y / 90 % 4];

            // z rotation
            Vector3Int[] rotation_z = new Vector3Int[]
            {
            vec,
            new Vector3Int(-vec.y, vec.x, vec.z),
            new Vector3Int(-vec.x, -vec.y, vec.z),
            new Vector3Int(vec.y, -vec.x, vec.z)
            };

            vec = rotation_z[_rotation.z / 90 % 4];

            return vec;
        }

        /// <summary>
        /// set the orientation of the blockvoxels to the orientation of the block
        /// </summary>
        void RotateBlockVoxels()
        {
            BlockVoxels.ForEach(v => v.Orientation += _rotation);
        }

        /// <summary>
        /// Create the parrent gameobject of the block
        /// </summary>
        public void InstantiateGoParrentBlock(float voxelSize, Transform parrent)
        {
            goBlockParent = new GameObject();//GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            goBlockParent.name = $"Block {ZeroIndex}";
            goBlockParent.transform.localScale = new Vector3(voxelSize * 3, voxelSize * 6, voxelSize * 1);
            goBlockParent.transform.position = (Vector3)ZeroIndex * voxelSize;
            goBlockParent.transform.rotation = Quaternion.Euler(_rotation);
            goBlockParent.transform.SetParent(parrent, true);
        }

        /// <summary>
        /// Instantiate or switch the blockvoxels gameobjects to their new state
        /// </summary>
        /// <param name="grid">The global grid</param>
        public void DrawBlock(Grid3D grid, float voxelSize, Transform parrent)
        {
            InstantiateGoParrentBlock(voxelSize,parrent);
            foreach (var vox in BlockVoxels)
            {
                if (!(vox.Index.x < 0 ||
                    vox.Index.y < 0 ||
                    vox.Index.z < 0 ||
                    vox.Index.x >= GridController.Size.x ||
                    vox.Index.y >= GridController.Size.y ||
                    vox.Index.z >= GridController.Size.z))
                {
                    var gridVox = grid.GetVoxelAt(vox.Index);
                    if ((vox.Type == VoxelType.Block || vox.Type == VoxelType.Connection) && gridVox.Type != VoxelType.Block)
                    {
                        if (gridVox.Go == null)
                        {
                            gridVox.Go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            gridVox.Go.transform.position = new Vector3 (voxelSize * vox.Index.x, voxelSize * vox.Index.y, voxelSize * vox.Index.z);
                            gridVox.Go.transform.localScale = voxelSize * Vector3.one*0.98f;
                            gridVox.Go.transform.localRotation = Quaternion.identity;
                            gridVox.Go.transform.SetParent(vox.ParentBlock.goBlockParent.transform,true);
                            gridVox.Go.name = vox.Name;
                        }

                        if (vox.Type == VoxelType.Connection)
                        {
                            GameObject go = gridVox.Go;
                            var rend = go.GetComponentInChildren<Renderer>();
                            go.transform.SetParent(vox.ParentBlock.goBlockParent.transform);
                            //rend.material = Controller.MatConnection;
                        }
                        else if (vox.Type == VoxelType.Block)
                        {
                            GameObject go = gridVox.Go;
                            var rend = go.GetComponentInChildren<Renderer>();
                            go.transform.SetParent(vox.ParentBlock.goBlockParent.transform);
                            //rend.material = Controller.MatBlock;
                        }
                    }
                }
            }
        }
    }
}