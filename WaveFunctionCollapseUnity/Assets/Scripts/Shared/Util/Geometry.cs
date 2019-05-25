using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace WaveFunctionCollapse.Shared //Code by VS
{
    public struct Vector3Shared
    {
        public float x, y, z;
    }

    public struct Vector3IntShared
    {
        public int x, y, z;

        public static Vector3IntShared operator *(Vector3IntShared a, Vector3IntShared b)
        {
            return new Vector3IntShared { x = a.x * b.x, y = a.y * b.y, z = a.z * b.z };
        }

        public static Vector3IntShared operator +(Vector3IntShared a, Vector3IntShared b)
        {
            return new Vector3IntShared { x = a.x + b.x, y = a.y + b.y, z = a.z + b.z };
        }

        public string ToString()
        {
            return $"Vector3Int x:{x}, y:{y}, z:{z}";
        }
    }


    public struct Vector2
    {
        public float x, y;
    }

    /*public struct Quaternion
    {
        public float x, y, z, w;
    }*/

    /*public struct Pose
    {
        public Vector3 position;
        public Quaternion rotation;
    }*/


    public class Mesh
    {
        public List<Vector3Shared> Vertices { get; set; }
        public List<Vector2> TextureCoordinates { get; set; }
        public List<int> Faces { get; set; }
    }
}
