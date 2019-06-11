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
        static List<Instance> _instancesExport = new List<Instance>();

        public static bool Export(List<ALIS_Sample> SelectedSamples, Vector3Int gridDimensions, Vector3Int tileDimensions, string path, int seed)
        {
            for (int i = 0; i < SelectedSamples.Count; i++)
            {
                var sample = SelectedSamples[i];
                var sampleIndex = Util.ToUnityVector3Int(UtilShared.GetIndexInGrid(i, gridDimensions.ToVector3IntShared()));
                Vector3 worldIndex = sampleIndex * sampleIndex;

                foreach (var instance in sample.Instances)
                {
                    var newInstance = new Instance()
                    {
                        //transform to z-up
                        Pose = new Pose
                        {
                            position = new Vector3(instance.Pose.position.x, instance.Pose.position.z, instance.Pose.position.y),
                            rotation = instance.Pose.rotation
                        }
                    };
                    newInstance.Pose.rotation.w = -newInstance.Pose.rotation.w;

                    newInstance.Pose.position += new Vector3(worldIndex.x, worldIndex.z,worldIndex.y);
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

    public class ExportAssembly
    {
        public List<Instance> Instances;
        public ExportAssembly()
        {
        }

        public static void Export(List<Instance> instances, string fileName)
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
}
