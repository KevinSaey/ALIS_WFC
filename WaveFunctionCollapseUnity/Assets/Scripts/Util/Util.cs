
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaveFunctionCollapse.Shared;
using System.Linq;

namespace WaveFunctionCollapse.Unity
{
    public enum Axis { X, Y, Z };

    public class UnityLog : ISharedLogger
    {
        public bool EnableLog = true;

        public UnityLog(bool log)
        {
            EnableLog = log;
        }



        public void Log(object message)
        {
            Util.Warning = message.ToString();
            if (message.ToString().StartsWith("Error")) Debug.LogError(message);
            else if (EnableLog) Debug.Log(message);
        }
    }




    public static class Util
    {
        public static string Warning = "No warnings";
        public static Random RandomGenerator = new Random();

        public static Vector3Int ToUnityVector3Int(Vector3IntShared vector3IntShared) // Ask Vicente how to do this properly
        {
            return new Vector3Int(vector3IntShared.X, vector3IntShared.Y, vector3IntShared.Z);
        }


        public static void TryOrientIndex(Vector3 localIndex, Vector3 anchor, Quaternion rotation, out Vector3Int worldIndex)
        {
            var rotated = rotation * localIndex.ToVector3IntRound();
            worldIndex = (anchor + rotated).ToVector3IntRound();

        }


        public static void OrientIndex(Vector3 localIndex, Vector3 pivot, int angle, out Vector3Int rotatedIndex)
        {
            var direction = localIndex - pivot;
            direction = Quaternion.Euler(0, 90, 0) * direction;

            rotatedIndex = (direction + pivot).ToVector3IntRound();
        }

        public static bool TryOrientIndex(Vector3Int localIndex, Vector3Int anchor, Quaternion rotation, Grid3D grid, out Vector3Int worldIndex)
        {
            var rotated = rotation * localIndex;
            worldIndex = anchor + rotated.ToVector3IntRound();
            return CheckBounds(worldIndex, grid);
        }

        public static Vector3Int ToVector3IntRound(this Vector3 v) => new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));

        public static Vector3IntShared ToVector3IntShared(this Vector3Int v) => new Vector3IntShared(v.x, v.y, v.z);

        public static Vector3 AbsoluteValues(this Vector3 v) => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));

        public static bool CheckBounds(Vector3Int index, Grid3D grid)
        {
            if (index.x < 0) return false;
            if (index.y < 0) return false;
            if (index.z < 0) return false;
            if (index.x >= grid.Size.x) return false;
            if (index.y >= grid.Size.y) return false;
            if (index.z >= grid.Size.z) return false;
            return true;
        }

        //Vicente Soler Script  - a bit modified;
        public static bool IsInside(IEnumerable<MeshCollider> colliders, Vector3 point)
        {
            Vector3 position = point;

            Physics.queriesHitBackfaces = true;

            var sortedHits = new Dictionary<Collider, int>();
            foreach (var collider in colliders)
                sortedHits.Add(collider, 0);

            while (Physics.Raycast(new Ray(position, Vector3.forward), out RaycastHit hit))
            {
                var collider = hit.collider;


                if (sortedHits.ContainsKey(collider))
                {
                    sortedHits[collider]++;
                    // Debug.Log("stored hit N" + sortedHits.Count + ", position of 1" + hit.transform.position);
                }

                position = hit.point + Vector3.forward * /*(1 / Mathf.Infinity)*/ 0.00001f;
            }
            bool isInside = sortedHits.Any(kv => kv.Value % 2 != 0);

            return isInside;
        }
    }
}

