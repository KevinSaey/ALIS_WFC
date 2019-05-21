using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaveFunctionCollapse.Shared;

namespace WaveFunctionCollapse.Unity
{
    public enum Axis { X, Y, Z }; 
    public class UnityLog : ISharedLogger
    {
        public void Log(object message)
        {

            //Debug.Log(message);
            
        }
    }



    
    public static class Util
    {
        public static Random RandomGenerator = new Random();

        public static Vector3Int ToUnityVector3Int(Vector3IntShared vector3IntShared) // Ask Vicente how to do this properly
        {
            return new Vector3Int(vector3IntShared.x, vector3IntShared.y, vector3IntShared.z);
        }

        public static Vector3Int ToVector3Int(this Vector3 v)
        {
            int x = Mathf.RoundToInt(v.x);
            int y = Mathf.RoundToInt(v.y);
            int z = Mathf.RoundToInt(v.z);
            return new Vector3Int(x, y, z);
        }
    }

   
}
