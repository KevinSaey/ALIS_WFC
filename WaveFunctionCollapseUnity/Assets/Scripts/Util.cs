using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaveFunctionCollapse.Shared;

namespace WaveFunctionCollapse.Unity
{
    public class UnityLog : ISharedLogger
    {
        public void Log(object message)
        {
            Debug.Log(message);
            
        }
    }

    
    public static class Util
    {
        public static Random RandomGenerator = new Random();

        public static Vector3Int ToUnityVector3Int(Vector3IntShared vector3IntShared) // Ask Vicente how to do this properly
        {
            return new Vector3Int(vector3IntShared.x, vector3IntShared.y, vector3IntShared.z);
        }

        
    }

   
}
