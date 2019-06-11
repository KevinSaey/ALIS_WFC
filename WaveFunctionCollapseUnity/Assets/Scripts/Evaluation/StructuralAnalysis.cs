using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BriefFiniteElementNet;
using BriefFiniteElementNet.Elements;

namespace WaveFunctionCollapse.Unity
{
    public class StructuralAnalysis // Vicente's code
    {

        float _displacement = -1f;
        //float _tempDisplacement = 10f;
        bool _toggleTransparency = false;
        public bool Deflection = true;
        bool _soleved = false;
        Model _model = new Model();
        List<FeaCorner> _corners;
        Dictionary<Voxel, List<Tetrahedral>> _voxelNodes;

        // grid
        Grid3D _grid;
        Mesh[] _meshes;

        public StructuralAnalysis(Grid3D grid)
        {
            _grid = grid;
        }

        public void Analysis()
        {
            if (!_soleved)
            {
                // analysis model


                _corners = _grid.GetCorners()
                    .Where(c => c.GetConnectedVoxels().Any(v => v.Type == VoxelType.Block))
                    .Select(c => new FeaCorner(c))
                    .ToList();

                var nodes = _corners.Select(c => c.Node).ToArray();

                var voxels = _grid.GetVoxels().Where(v => v.Type == VoxelType.Block).ToList();
                _voxelNodes = new Dictionary<Voxel, List<Tetrahedral>>();
                foreach (var vox in voxels)
                {
                    _voxelNodes.Add(vox, MakeTetrahedra(vox).ToList());
                }
                /*var elements = voxels
                     .SelectMany(v => MakeTetrahedra(v))
                     .ToArray();*/

                _model.Nodes.Add(nodes);
                _model.Elements.Add(_voxelNodes.Values.SelectMany(s => s).ToArray());

                _model.Solve();
                _soleved = true;
            }

            var forces = new List<float>();
            if (!Deflection)
            {
                foreach (var vox in _voxelNodes)
                {
                    float totalForce = 0;
                    foreach (var element in vox.Value)
                    {
                        var internalForce = Mathf.Abs((float)element.GetInternalForce(LoadCombination.DefaultLoadCombination).S11);
                        internalForce += Mathf.Abs((float)element.GetInternalForce(LoadCombination.DefaultLoadCombination).S12);
                        internalForce += Mathf.Abs((float)element.GetInternalForce(LoadCombination.DefaultLoadCombination).S13);
                        internalForce += Mathf.Abs((float)element.GetInternalForce(LoadCombination.DefaultLoadCombination).S21);
                        internalForce += Mathf.Abs((float)element.GetInternalForce(LoadCombination.DefaultLoadCombination).S22);
                        internalForce += Mathf.Abs((float)element.GetInternalForce(LoadCombination.DefaultLoadCombination).S23);
                        internalForce += Mathf.Abs((float)element.GetInternalForce(LoadCombination.DefaultLoadCombination).S31);
                        internalForce += Mathf.Abs((float)element.GetInternalForce(LoadCombination.DefaultLoadCombination).S32);
                        internalForce += Mathf.Abs((float)element.GetInternalForce(LoadCombination.DefaultLoadCombination).S33);

                        internalForce /= 9;

                        totalForce += (float)internalForce;
                    }
                    totalForce /= vox.Value.Count;
                    forces.Add(Mathf.Abs(totalForce));

                    vox.Key.Value = Mathf.Abs(totalForce);
                }
            }

            // analysis results

            if (Deflection)
            {
                foreach (var vox in _grid.Voxels) vox.Value = 0;
            }
            foreach (var corner in _corners)
            {
                var d = corner.Node
               .GetNodalDisplacement(LoadCase.DefaultLoadCase)
               .Displacements;
                corner.Displacement = new Vector3((float)d.X, (float)d.Z, (float)d.Y);
                var length = corner.Displacement.magnitude;
                if (Deflection)
                {
                    foreach (var voxel in corner.GetConnectedVoxels())
                        voxel.Value += length;
                }
            }

            var activeVoxels = _grid.GetVoxels().Where(v => v.Type == VoxelType.Block);

            foreach (var voxel in activeVoxels)
                voxel.Value /= voxel.GetCorners().Count();

            var min = activeVoxels.Min(v => v.Value);
            var max = activeVoxels.Max(v => v.Value);

            foreach (var voxel in activeVoxels)
                voxel.Value = Mathf.InverseLerp(min, max, voxel.Value);
        }

        public void DrawMesh(float tempDisplacement)
        {
            if (_grid == null) return;

            if (_meshes == null || Time.frameCount % 10 == 0)
            {
                _displacement = tempDisplacement;
                MakeVoxelMesh();
            }

            Drawing.DrawMesh(_toggleTransparency, _meshes);
        }

        public void MakeVoxelMesh()
        {
            _meshes = _grid.GetVoxels()
                .Where(v => v.Type == VoxelType.Block)
                .Select(v =>
                {
                    var corners = v.GetCorners()
                            .Select(c => c.Position + ((c as FeaCorner).Displacement * _displacement))
                            .ToArray();

                    return Drawing.MakeTwistedBox(corners, v.Value, null);
                }).ToArray();
        }

        public IEnumerable<Tetrahedral> MakeTetrahedra(Voxel voxel)
        {
            var c = voxel.GetCorners().ToArray();

            var t = new[,]
            {
           { c[0], c[1], c[2], c[4]},
           { c[3], c[1], c[2], c[7]},
           { c[1], c[2], c[4], c[7]},
           { c[4], c[5], c[7], c[1]},
           { c[4], c[7], c[6], c[2]}
        };

            for (int i = 0; i < 5; i++)
            {
                var tetra = new Tetrahedral() { E = 210e9, Nu = 0.33 };

                for (int j = 0; j < 4; j++)
                    tetra.Nodes[j] = (t[i, j] as FeaCorner).Node;

                yield return tetra;
            }
        }
    }

    class FeaCorner : Corner
    {
        public Node Node;
        public Vector3 Displacement;

        public FeaCorner(Corner corner) : base(corner)
        {
            Node = new Node
            {
                Location = new Point(corner.Position.x, corner.Position.z, corner.Position.y),
                Constraints = corner.Index.y == 0 ? Constraints.Fixed : Constraints.RotationFixed
            };

            Node.Loads.Add(new NodalLoad(new Force(0, 0, -100000, 0, 0, 0)));
        }
    }
}