using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using QuickGraph;
using QuickGraph.Algorithms;

namespace WaveFunctionCollapse.Unity
{
    public class PathFinding //this class is largely a refactored version of Vicente's code
    {

        Grid3D _grid;
        Mesh _mesh;
        public int RidiculousHighNumber = 9999;

        public PathFinding(Grid3D grid)
        {
            _grid = grid;
            Regenerate();
        }

        public void Regenerate()
        {
            GenerateClimableMeshes(CreateGraph());
        }

        public void DrawMesh()
        {
            if (_grid == null || _mesh == null) return;
            Drawing.DrawMesh(false, _mesh);
        }

        public TryFunc<Face, IEnumerable<TaggedEdge<Face, Edge>>> CreateGraph()
        {
            // select edges of boundary faces
            var edges = _grid.GetEdges().Where(e => e.ClimbableFaces.Length == 2);

            // create graph from edges -- library quickgraph
            var graphEdges = edges.Select(e => new TaggedEdge<Face, Edge>(e.ClimbableFaces[0], e.ClimbableFaces[1], e));
            var graph = graphEdges.ToUndirectedGraph<Face, TaggedEdge<Face, Edge>>();

            // start face for shortest path
            var start = _grid.Faces[1][0, 0, 0];
            //var start2 = _grid.GetVoxelAt(_grid.Blocks[0].ZeroIndex).Faces.Last(f => f != null && f.Climable);

            // calculate shortest path from start face to all boundary faces
            return graph.ShortestPathsDijkstra(e => 1.0, start);
        }

        public void GenerateClimableMeshes(TryFunc<Face, IEnumerable<TaggedEdge<Face, Edge>>> shortest)
        {
            var faceMeshes = new List<CombineInstance>();

            var climableFaces = _grid.GetClimableFaces().Where(f => GetPathCount(shortest, f) > 0);
            //int biggestDistance = climableFaces.First(f => f.DistanceFromZero < _ridiculusHighNumber).DistanceFromZero;

            foreach (var face in climableFaces)
            {
                float t = 0;

                t = face.DistanceFromZero * 0.01f;//biggestDistance;
                t = Mathf.Clamp01(t);

                Mesh faceMesh;
                faceMesh = Drawing.MakeFace(face.Center, face.Direction, 1, t);
                if (face.DistanceFromZero < RidiculousHighNumber)
                {
                    faceMeshes.Add(new CombineInstance() { mesh = faceMesh });
                }
            }
            var mesh = new Mesh()
            {
                indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
            };

            mesh.CombineMeshes(faceMeshes.ToArray(), true, false, false);

            _mesh = mesh;
        }

        public int GetPathCount(TryFunc<Face, IEnumerable<TaggedEdge<Face, Edge>>> shortest, Face face)
        {
            shortest(face, out var path);
            face.DistanceFromZero = path == null ? RidiculousHighNumber : path.Count();
            return face.DistanceFromZero;
        }
    }
}