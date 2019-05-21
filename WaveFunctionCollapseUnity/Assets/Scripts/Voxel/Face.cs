using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace WaveFunctionCollapse.Unity
{
    public class Face //Adapted Vicente's code
    {
        public enum BoundaryType { Inside = 0, Left = -1, Right = 1, Outside = 2 }
        bool _climableFloor;

        /// <summary>
        /// Check if this face is climable. Horizontal planes (floors), and vertical planes (walls) according to pattern are climable. Ceilings aren't
        /// </summary>
        public bool Climable
        {
            get
            {
                return (Normal != Vector3Int.down && HasOneBlockParent() && (WalkablePattern() || Normal == Vector3Int.up)) || Normal == Vector3Int.up && Index.y == 0;
            }
        }

        /// <summary>
        /// Get the Boundary type of this face.
        /// </summary>
        public BoundaryType Boundary//VS
        {
            get
            {
                bool left = ParentVox[0]?.Type == VoxelType.Block;
                bool right = ParentVox[1]?.Type == VoxelType.Block;

                if (!left && right) return BoundaryType.Left;
                if (left && !right) return BoundaryType.Right;
                if (left && right) return BoundaryType.Inside;
                return BoundaryType.Outside;
            }
        }

        /// <summary>
        /// Get the normal of this face. Only faces with boundry type 'Outside' have normals
        /// </summary>
        public Vector3Int Normal//VS
        {
            get
            {
                int f = (int)Boundary;
                if (Boundary == BoundaryType.Outside) f = 0;

                if (Index.y == 0 && Direction == Axis.Y)
                {
                    f = Boundary == BoundaryType.Outside ? 1 : 0;
                }

                switch (Direction)
                {
                    case Axis.X:
                        return Vector3Int.right * f;
                    case Axis.Y:
                        return Vector3Int.up * f;
                    case Axis.Z:
                        return new Vector3Int(0, 0, 1) * f;
                    default:
                        throw new Exception("Wrong direction.");
                }
            }
        }

        public Axis Direction;
        public Voxel[] ParentVox;
        public Vector3Int Index;
        public Vector3 Center;
        Grid3D _grid;
        public int DistanceFromZero = 0;

        /// <summary>
        /// Initialise a face within a voxel grid
        /// </summary>
        /// <param name="x">X index</param>
        /// <param name="y">Y index</param>
        /// <param name="z">Z index</param>
        /// <param name="direction">Allignment of normal the face (.X, .Y, .Z)</param>
        /// <param name="grid">The voxelgrid where the face exists in</param>
        public Face(int x, int y, int z, Axis direction, Grid3D grid)
        {
            _grid = grid;
            Index = new Vector3Int(x, y, z);
            Direction = direction;
            ParentVox = GetVoxels();

            foreach (var v in ParentVox.Where(v => v != null))
                v.Faces.Add(this);

            Center = GetCenter();
        }

        /// <summary>
        /// Get the voxels at both side of the grid
        /// </summary>
        /// <returns>The the two voxels (if the face is at the edge of the voxelgrid, the voxel outside the grid will be null)</returns>
        Voxel[] GetVoxels()//VS
        {
            int x = Index.x;
            int y = Index.y;
            int z = Index.z;

            switch (Direction)
            {
                case Axis.X:
                    return new[]
                    {
                   x == 0 ? null : _grid.Voxels[x - 1, y, z],
                   x == _grid.Size.x ? null : _grid.Voxels[x, y, z]
                };
                case Axis.Y:
                    return new[]
                    {
                   y == 0 ? null : _grid.Voxels[x, y - 1, z],
                   y == _grid.Size.y ? null : _grid.Voxels[x, y, z]
                };
                case Axis.Z:
                    return new[]
                    {
                   z == 0 ? null : _grid.Voxels[x, y, z - 1],
                   z == _grid.Size.z ? null : _grid.Voxels[x, y, z]
                };
                default:
                    throw new Exception("Wrong direction, Bitch!");
            }
        }

        /// <summary>
        /// Get the center of this face.
        /// </summary>
        /// <returns>The center of this face</returns>
        Vector3 GetCenter()//VS
        {
            int x = Index.x;
            int y = Index.y;
            int z = Index.z;

            switch (Direction)
            {
                case Axis.X:
                    return new Vector3(x, y + 0.5f, z + 0.5f) * _grid.VoxelSize;
                case Axis.Y:
                    return new Vector3(x + 0.5f, y, z + 0.5f) * _grid.VoxelSize;
                case Axis.Z:
                    return new Vector3(x + 0.5f, y + 0.5f, z) * _grid.VoxelSize;
                default:
                    throw new Exception("Wrong direction, Bitch!");
            }
        }

        /// <summary>
        /// Get the corners of this face within the voxelgrid
        /// </summary>
        /// <returns></returns>
        Corner[] GetCorners()//VS
        {
            int x = Index.x;
            int y = Index.y;
            int z = Index.z;

            switch (Direction)
            {
                case Axis.X:
                    return new[]
                    {
                 _grid.Corners[x, y, z],
                 _grid.Corners[x, y + 1, z],
                 _grid.Corners[x, y, z + 1],
                 _grid.Corners[x, y + 1, z + 1]
                };
                case Axis.Y:
                    return new[]
                    {
                 _grid.Corners[x, y, z],
                 _grid.Corners[x + 1, y, z],
                 _grid.Corners[x, y, z + 1],
                 _grid.Corners[x + 1, y, z + 1]
                };
                case Axis.Z:
                    return new[]
                    {
                 _grid.Corners[x, y, z],
                 _grid.Corners[x + 1, y, z],
                 _grid.Corners[x, y + 1, z],
                 _grid.Corners[x + 1, y + 1, z]
                };
                default:
                    throw new Exception("Wrong direction.");
            }
        }

        /// <summary>
        /// Check if the face is part of the climable faces of the pattern
        /// </summary>
        /// <returns>The face is walkable</returns>
        public bool WalkablePattern()
        {
            return ParentVox.First(f => f?.Type == VoxelType.Block).WalkableFaces.Count(s => s == Normal) == 1;
        }

        /// <summary>
        /// Check if this face has only one parrent voxel with Voxel type block
        /// </summary>
        /// <returns></returns>
        public bool HasOneBlockParent()
        {
            return ParentVox.Count(s => s?.Type == VoxelType.Block) == 1;
        }
    }
}