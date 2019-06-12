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

        public static bool Export(List<ALIS_Sample> SelectedSamples, Vector3Int gridDimensions, Vector3Int tileDimensions, string path, int seed)
        {
            for (int i = 0; i < SelectedSamples.Count; i++)
            {
                var sample = SelectedSamples[i];
                var sampleIndex = Util.ToUnityVector3Int(UtilShared.GetIndexInGrid(i, gridDimensions.ToVector3IntShared()));
                Vector3 worldIndex = sampleIndex * sampleIndex;

                foreach (var instance in sample.Instances)
                {
                    var newInstance = new RhinoInstance()
                    {
                        //transform to z-up
                        Pose = new RhinoPose
                        {
                            position = new Vector3d(worldIndex + instance.Pose.position),
                            rotation = new QuaternionRhino(instance.Pose.rotation)
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
        public Vector3d position;
        public QuaternionRhino rotation;
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
            C = quaternion.z;
            D = quaternion.y;
        }
    }
    
        
}
