using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using WaveFunctionCollapse.Shared;

namespace WaveFunctionCollapse.Unity
{
    public static class RhinoExporter
    {
        static List<RhinoInstance> _instancesExport = new List<RhinoInstance>();

        public static bool Export(List<Tile> tiles, Vector3Int gridDimensions, Vector3Int tileDimensions, float voxelSize, string path, bool mesh)
        {
            if (!mesh) voxelSize = 1;
            var seed = ManagerWFC.Seed;
            foreach (var tile in tiles)
            {
                var sample = tile.SelectedSample as ALIS_Sample;
                Vector3 worldIndex = (Vector3)(Util.ToUnityVector3Int(tile.Index) * tileDimensions)*voxelSize;

                if (sample.Instances != null)
                    foreach (var instance in sample.Instances)
                    {
                        var newInstance = new RhinoInstance()
                        {
                            //transform to z-up
                            Pose = new RhinoPose
                            {
                                Position = new Vector3d(worldIndex + instance.Pose.position*voxelSize),
                                Rotation = new QuaternionRhino(instance.Pose.rotation)
                            }
                        };

                        _instancesExport.Add(newInstance);
                    }
            }

            if (!Directory.Exists(path))
            {
                SharedLogger.Log("Error: Path doesn't exist - function: Export");
                return false;
            }
            string filename = $@"{path}\Export\ExportSeed{seed}.xml";

            ExportAssembly.Export(_instancesExport, filename);
            return true;
        }
    }

    [XmlType(TypeName = "Assembly")]
    public class ExportAssembly
    {
        public List<RhinoInstance> Instances;
        public ExportAssembly()
        {
        }

        public static void Export(List<RhinoInstance> instances, string fileName)
        {
            var exportAssembly = new ExportAssembly()
            {
                Instances = instances
            };

            var serializer = new XmlSerializer(typeof(ExportAssembly));
            using (var writer = XmlWriter.Create(fileName))
            {
                serializer.Serialize(writer, exportAssembly);
            }
        }
    }

    [XmlType(TypeName = "Instance")]
    public struct RhinoInstance
    {
        public int DefnitionIndex;
        public RhinoPose Pose;
    }

    [XmlType(TypeName = "RhinoPose")]
    public struct RhinoPose
    {
        public Vector3d Position;
        public QuaternionRhino Rotation;
    }

    [XmlType(TypeName = "ImportVector3d")]
    public struct Vector3d
    {
        public float X, Y, Z;
        public Vector3d(Vector3 vector)
        {
            X = vector.x;
            Y = vector.z;
            Z = vector.y;
        }
    }

    [XmlType(TypeName = "ImportQuaternion")]
    public struct QuaternionRhino
    {
        public float A, B, C, D;

        public QuaternionRhino(Quaternion quaternion)
        {
            A = -quaternion.w;
            B = quaternion.x;
            D = quaternion.y;
            C = quaternion.z;
        }
    }


}
