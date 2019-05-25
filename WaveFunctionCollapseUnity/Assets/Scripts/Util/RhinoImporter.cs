using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.IO;


namespace WaveFunctionCollapse.Unity
{
    /// <summary>
    /// Import a assembly pattern generated in grasshopper
    /// </summary>
    public class RhinoImporter//based on Vicente's code
    {
        public List<ALIS_Sample> Samples = new List<ALIS_Sample>();
        string _path = @"D:\Unity\School\ALIS_WFC\WaveFunctionCollapseUnity\RhinoExporter\";
        //Grid3D _grid;

        public RhinoImporter(Vector3Int tileSize, bool rotate)
        {
            var files = LoadFiles();
            Debug.Log($"{files.Count()} ALIS_samples loaded");


            for (int i = 0; i < files.Count; i++)
            {
                Samples.Add(Assembly.Import(files[i]).ToALIS_Sample());
            }

            if (rotate)
            {
                //rotate samples
                var nrOfSamples = Samples.Count;
                for (int i = 1; i < nrOfSamples; i++)
                {
                    //if (Samples[i].Instances.Count != 0)
                    //{
                    Samples.Add(RotateALISSample(Samples[i], Samples.Count, ((Vector3)tileSize - Vector3.one) / 2, 1, i));
                    for (int j = 1; j < 3; j++)
                    {
                        Samples.Add(RotateALISSample(Samples.Last(), Samples.Count, ((Vector3)tileSize - Vector3.one) / 2, 1 + j, i));
                    }
                    //}
                }
            }
        }

        public List<string> LoadFiles()
        {
            return Directory.GetFiles(_path, "*.xml").ToList();
        }

        public ALIS_Sample RotateALISSample(ALIS_Sample sample, int id, Vector3 anchor, int timesRoated, int origSampleId)
        {
            var conn = sample.PossibleConnections;
            var newConn = new List<HashSet<int>> { conn[2], conn[3], conn[1], conn[0], conn[4], conn[5] }; // check this for lefthand rotation
            List<Instance> newInstances = new List<Instance>();
            for (int i = 0; i < sample.Instances.Count; i++)
            {
                var oldInstance = sample.Instances[i];

                var oldPos = oldInstance.Pose.position;
                Util.OrientIndex(oldPos, anchor, 90, out var newPos);
                var oldRot = oldInstance.Pose.rotation.eulerAngles;
                var newRot = Quaternion.Euler(Vector3.up * 90 + oldInstance.Pose.rotation.eulerAngles);
                var newPose = new Pose(newPos, newRot);

                newInstances.Add(new Instance { DefinitionIndex = oldInstance.DefinitionIndex, Pose = newPose });
            }
            var name = $"sample {origSampleId} type {sample.Type} rot: {timesRoated * 90}";

            return new ALIS_Sample(id, sample.Density, sample.Type, newConn, newInstances, name);
        }
    }



    /// <summary>
    /// Assembly pattern existing of blocks 
    /// </summary>
    public class Assembly //VS
    {
        public List<Instance> Instances { get; set; }
        public List<Neighbour> Neighbours { get; set; }
        public int Id;
        public int Density;
        public int Type;

        public static Assembly Import(string fileName)
        {
            var serializer = new XmlSerializer(typeof(Assembly));
            using (var reader = XmlReader.Create(fileName))
            {
                return serializer.Deserialize(reader) as Assembly;
            }

        }

        public ALIS_Sample ToALIS_Sample()
        {
            List<HashSet<int>> possibleConnections = new List<HashSet<int>>();
            for (int i = 0; i < Neighbours.Count; i++)
            {
                
                    possibleConnections.Add(new HashSet<int>(Neighbours[i].Neighbours/*.Where(s=>s!= int.MaxValue)*/));
                
            }
            var name = $"sample {Id} type {Type} rot: 0 minX: {possibleConnections[0].First()} plusX: {possibleConnections[1].First()} minY: {possibleConnections[2].First()}  plusY: {possibleConnections[3].First()}  minZ: {possibleConnections[4].First()} plusZ  {possibleConnections[5].First()}";
            ALIS_Sample alis_Sample = new ALIS_Sample(Id, Density, Type, possibleConnections, Instances, name);
            return alis_Sample;
        }


        /*
        public void Generate(Grid3D grid)
        {
            var pattern = new PatternC();
            foreach (var instance in Instances)
            {
                var rotation = instance.Pose.rotation.eulerAngles.ToVector3Int();
                Debug.Log("imported rotation " + rotation);
                if (rotation.x == -90) rotation.x = 270;
                if (rotation.y == -90) rotation.x = 270;
                if (rotation.z == -90) rotation.x = 270;
                Debug.Log("adjusted rotation " + rotation);

                var block = new Block(pattern, instance.Pose.position.ToVector3Int(), rotation, grid);
                grid.AddBlockToGrid(block);
            }
        }*/
        public class Neighbour
        {
            public List<int> Neighbours;

            private Neighbour() { }
            public Neighbour(List<int> neighbours)
            {
                Neighbours = neighbours;
            }
        }
    }

    public struct Instance
    {
        public int DefinitionIndex;
        public Pose Pose;
    }
}