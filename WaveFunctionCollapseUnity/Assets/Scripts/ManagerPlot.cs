using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using WaveFunctionCollapse.Shared;

namespace WaveFunctionCollapse.Unity
{
    public static class ManagerPlot
    {
        static List<Mesh> _plotMeshes = new List<Mesh>();
        public static GameObject GoPlot = new GameObject("goPlot");
        public static bool _convex = false;
        public static int NrOfPlots
        {
            get
            {
                return _plotMeshes.Count;
            }
        }

        public static bool PlotsLoaded
        {
            get
            {
                return _plotMeshes.Count > 0;
            }
        }

        public static Vector3 CentrePlot(int currentPlot)
        {
            if (PlotsLoaded) return _plotMeshes[currentPlot].bounds.center;
            else return Vector3.zero;
        }


        public static void CreateGoPlot(Material mat)
        {
            LoadMeshes();
            mat.mainTexture = null;
            var meshFilter = GoPlot.AddComponent<MeshFilter>();
            meshFilter.mesh = _plotMeshes[0];
            var meshRender = GoPlot.AddComponent<MeshRenderer>();
            meshRender.material = mat;
            meshRender.enabled = false;
            var meshCollider = GoPlot.AddComponent<MeshCollider>();
            meshCollider.convex = _convex;
            meshCollider.sharedMesh = meshFilter.mesh;

        }

        public static Vector3Int GetWFCSize(int currentPlot, float voxelSize, Vector3 tileSize)
        {
            var mesh = _plotMeshes[currentPlot];
            var plotBounds = mesh.bounds;
            Vector3Int wfcSize = new Vector3Int();
            wfcSize.x = Mathf.RoundToInt(plotBounds.size.x / (tileSize.x * voxelSize));
            wfcSize.y = Mathf.RoundToInt(plotBounds.size.y / (tileSize.y * voxelSize));
            wfcSize.z = Mathf.RoundToInt(plotBounds.size.z / (tileSize.z * voxelSize));
            return wfcSize;
        }

        static void LoadMeshes()
        {
            List<Mesh> meshes = Resources.LoadAll("Plot", typeof(Mesh)).Cast<Mesh>().ToList();
            _plotMeshes.AddRange(meshes);
        }

        public static void TogglePlot(bool toggle)
        {
            var render = GoPlot.GetComponent<MeshRenderer>();
            render.enabled = toggle;
        }

        public static void NextPlot(int nextPlot)
        {
            var meshFilter = GoPlot.GetComponent<MeshFilter>();
            meshFilter.mesh = _plotMeshes[nextPlot];
            var meshCollider = GoPlot.GetComponent<MeshCollider>();
            meshCollider.convex = _convex;
            meshCollider.sharedMesh = meshFilter.mesh;
        }
    }
}
