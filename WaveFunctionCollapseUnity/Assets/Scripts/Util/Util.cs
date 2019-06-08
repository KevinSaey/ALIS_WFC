
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaveFunctionCollapse.Shared;

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
            if (message.ToString().StartsWith("Error")) Debug.LogError(message);
            else if (EnableLog) Debug.Log(message);
        }
    }




    public static class Util
    {
        public static Random RandomGenerator = new Random();

        public static Vector3Int ToUnityVector3Int(Vector3IntShared vector3IntShared) // Ask Vicente how to do this properly
        {
            return new Vector3Int(vector3IntShared.x, vector3IntShared.y, vector3IntShared.z);
        }


        public static void TryOrientIndex(Vector3 localIndex, Vector3 anchor, Quaternion rotation, out Vector3Int worldIndex)
        {
            var rotated = rotation * localIndex.ToVector3Int();
            worldIndex = (anchor + rotated).ToVector3Int();

        }


        public static void OrientIndex(Vector3 localIndex, Vector3 pivot, int angle, out Vector3Int rotatedIndex)
        {
            var direction = localIndex - pivot;
            direction = Quaternion.Euler(0, 90, 0) * direction;

            rotatedIndex = (direction + pivot).ToVector3Int();
        }

        public static bool TryOrientIndex(Vector3Int localIndex, Vector3Int anchor, Quaternion rotation, Grid3D grid, out Vector3Int worldIndex)
        {
            var rotated = rotation * localIndex;
            worldIndex = anchor + rotated.ToVector3Int();
            return CheckBounds(worldIndex, grid);
        }

        public static Vector3Int ToVector3Int(this Vector3 v) => new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));

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
    }



}

