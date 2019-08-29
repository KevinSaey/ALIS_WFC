using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;

namespace WaveFunctionCollapse.Unity
{
    public static class ManagerPlot
    {
        static List<Mesh> _plotMeshes = new List<Mesh>();
        public static GameObject GoPlot = new GameObject("goPlot");
        public static bool _convex = false;
        static bool _outlineDirty = false;
        static bool _showPlot = true;
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
            mat.color = new Color(0, 0, 0, 0);
            LoadMeshes();
            mat.mainTexture = null;
            var meshFilter = GoPlot.AddComponent<MeshFilter>();
            meshFilter.mesh = _plotMeshes[0];
            var meshCollider = GoPlot.AddComponent<MeshCollider>();
            meshCollider.convex = _convex;
            meshCollider.sharedMesh = meshFilter.mesh;

            if (_showPlot)
            {
                var meshRender = GoPlot.AddComponent<MeshRenderer>();
                meshRender.material = mat;
                meshRender.enabled = false;

                var outline = GoPlot.AddComponent<Outline>();
                outline.OutlineMode = Outline.Mode.OutlineAll;
                outline.OutlineColor = new Color(0, 0, 0, 1);
                outline.OutlineWidth = 3;
            }


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
            if (_showPlot)
            {
                var render = GoPlot.GetComponent<MeshRenderer>();
                render.enabled = toggle;
            }
        }

        public static void Update()
        {
            if (_outlineDirty)
            {
                var outline = GoPlot.AddComponent<Outline>();
                outline.OutlineMode = Outline.Mode.OutlineAll;
                outline.OutlineColor = new Color(0, 0, 0, 1);
                outline.OutlineWidth = 3;

                _outlineDirty = false;
            }
        }

        public static void NextPlot(int nextPlot)
        {
            var meshFilter = GoPlot.GetComponent<MeshFilter>();
            meshFilter.mesh = _plotMeshes[nextPlot];
            var meshCollider = GoPlot.GetComponent<MeshCollider>();
            meshCollider.convex = _convex;
            meshCollider.sharedMesh = meshFilter.mesh;

            var outline = GoPlot.GetComponent<Outline>();
            GameObject.Destroy(outline);
            _outlineDirty = true;
        }
    }
}
