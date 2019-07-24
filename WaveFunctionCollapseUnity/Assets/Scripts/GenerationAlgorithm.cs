using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace WaveFunctionCollapse.Unity
{
    public class GenerationAlgorithm : IGenerationAlgorithm //Coded together with Maxiem Geldhof
    {
        Vector3Int _target;


        public GenerationAlgorithm(Vector3Int target)
        {
            _target = target;
        }


        public Block GetNextBlock(Grid3D grid)
        {
            // The connection points, sorted by distance.
            IEnumerable<Voxel> byDistance = SortByDistance(GetConnectionPoints(grid));
            IEnumerable<Block> blocksFromDistance = byDistance.SelectMany(b => new Block[] { new Block(new PatternA(), b, grid), new Block(new PatternB(), b, grid) });

            return blocksFromDistance.FirstOrDefault(bl => Validate(bl, grid));
        }

        /// <summary>
        /// Get all the connectionvoxels out of a grid
        /// </summary>
        /// <param name="grid">grid</param>
        /// <returns>list of voxels with the VoxelType Connection</returns>
        private List<Voxel> GetConnectionPoints(Grid3D grid)
        {
            List<Voxel> voxels = new List<Voxel>();

            foreach (var vox in grid.Voxels)
            {
                if (vox.Type == VoxelType.Connection)
                    voxels.Add(vox);
            }
            return voxels;
        }

        /// <summary>
        /// Sort a list of voxels by there distance to the target
        /// </summary>
        /// <param name="toSort">The list of voxels to sort</param>
        /// <returns>Sorted list of voxel</returns>
        private IEnumerable<Voxel> SortByDistance(List<Voxel> toSort)
        {
            return toSort.OrderBy(vx => Vector3Int.Distance(vx.Index, _target));
        }


        /// <summary>
        /// These functions are meant to eliminate blocks that cannot / should not exist
        /// </summary>
        /// <param name="block">The block to validate</param>
        /// <param name="grid">The voxelgrid</param>
        /// <returns>Can the block be added or not</returns>
        private bool Validate(Block block, Grid3D grid)
        {
            bool valid = true;
            valid = valid && grid.CanBlockExist(block);
            //valid = valid && ConnectedPath(block, grid);
            //add more rules
            return valid;
        }

        /// <summary>
        /// Check if the added block is connected to the main robot path (no fully functional)
        /// </summary>
        /// <param name="block">The block to validate</param>
        /// <param name="grid">The voxelgrid</param>
        /// <returns>Can the block be added or not</returns>
        private bool ConnectedPath(Block block, Grid3D grid)
        {
            var pathFinding = grid.PFinding;
            var index = block.BlockVoxels.Where(v => v.Type == VoxelType.Block).Select(v => v.Index);
            var prevVoxels = index.Select(v => grid.GetVoxelAt(v));

            foreach (var vox in block.BlockVoxels.Where(v => v.Type == VoxelType.Block))
            {
                //place the block within the voxelgrid
                grid.AddVoxel(vox);

                foreach (var face in grid.GetVoxelAt(vox.Index).Faces.Where(f => f.Climable))
                {
                    int test = pathFinding.GetPathCount(pathFinding.CreateGraph(), face);
                    if (test != pathFinding.RidiculousHighNumber)
                    {
                        return true;
                    }
                }
            }

            //restore the original state of the voxelgrid
            foreach (var v in prevVoxels) grid.AddVoxel(v);

            return false;
        }
    }
}