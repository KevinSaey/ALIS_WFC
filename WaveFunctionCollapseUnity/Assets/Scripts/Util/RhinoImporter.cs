using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using WaveFunctionCollapse.Shared;


namespace WaveFunctionCollapse.Unity
{
    /// <summary>
    /// Import a assembly pattern generated in grasshopper
    /// </summary>
    public class RhinoImporter//based on Vicente's code
    {
        //public List<ALIS_Sample> Samples { get; private set; } = new List<ALIS_Sample>();
        public Dictionary<int, Sample> SampleLibrary { get; private set; }
        string _path = @"D:\Unity\School\ALIS_WFC\WaveFunctionCollapseUnity\RhinoExporter\";
        //Grid3D _grid;
        ManagerWFC _managerWFC;

        public RhinoImporter()
        {

        }

        public void InstantiateSamples(Vector3Int tileSize, bool rotate, bool reflect, ManagerWFC managerWFC)
        {
            var files = LoadFiles();
            _managerWFC = managerWFC;
            List<Assembly> importedAssemblies = new List<Assembly>();



            SampleLibrary = new Dictionary<int, Sample>();

            for (int i = 0; i < files.Count; i++)
            {
                importedAssemblies.Add(Assembly.Import(files[i]));
                var sample = Assembly.Import(files[i]).ToALIS_Sample(_managerWFC);

                SampleLibrary.Add(sample.Id, sample);

            }

            foreach (var sampleID in SampleLibrary.Keys)
            {
                List<HashSet<Sample>> neighbours = new List<HashSet<Sample>>();
                for (int i = 0; i < 6; i++)
                {
                    neighbours.Add(new HashSet<Sample>());
                    var importedNeighbours = importedAssemblies.First(s => s.Id == sampleID).Neighbours[i].Neighbours;
                    for (int j = 0; j < importedNeighbours.Count(); j++)
                    {
                        neighbours[i].Add(SampleLibrary[importedNeighbours[j]]);
                    }
                }
                SampleLibrary[sampleID].InitialisePossibleNeighbours(neighbours);
            }


            /*if (rotate)
            {
                var nrOfSamples = Samples.Count;
                var anchor = ((Vector3)tileSize - Vector3.one) / 2;
                //rotate samples

                for (int i = 1; i < nrOfSamples; i++)
                {
                    //if (Samples[i].Instances.Count != 0)
                    //{
                    Samples.Add(RotateALISSample(Samples[i], Samples.Count, anchor, 1, i));
                    for (int j = 1; j < 3; j++)
                    {
                        Samples.Add(RotateALISSample(Samples.Last(), Samples.Count, anchor, 1 + j, i));
                    }
                    //}
                }
            }*/
            /*if (reflect)
            {
                var nrOfSamples = Samples.Count;
                var anchor = ((Vector3)tileSize - Vector3.one) / 2;
                for (int i = 1; i < nrOfSamples; i++)
                {
                    Samples.Add(Reflect(Samples[i], Axis.X, anchor));
                    Samples.Add(Reflect(Samples[i], Axis.Z, anchor));
                }
            }*/

            CheckDuplicates();
            SharedLogger.Log($"{files.Count()} ALIS_samples loaded");
        }

        private List<string> LoadFiles()
        {
            return Directory.GetFiles(_path, "*.xml").ToList();
        }

        /*private ALIS_Sample Reflect(ALIS_Sample sampleToReflect, Axis axis, Vector3 Anchor)
        {
            ALIS_Sample reflectedSample;
            List<HashSet<int>> neighbours = new List<HashSet<int>>();
            List<Instance> instances = new List<Instance>();
            int id = Samples.Count;

            for (int i = 0; i < 6; i++)
            {
                neighbours.Add(new HashSet<int>(sampleToReflect.PossibleNeighbours[i]));
            }

            if (axis == Axis.X)
            {
                neighbours[0] = new HashSet<int>(sampleToReflect.PossibleNeighbours[1]);
                neighbours[1] = new HashSet<int>(sampleToReflect.PossibleNeighbours[0]);
                foreach (var instance in sampleToReflect.Instances)
                {
                    int newX = (int)(Anchor.x + (Anchor.x - instance.Pose.position.x));
                    var newInstance = instance;
                    newInstance.Pose.position.x = newX;
                    instances.Add(newInstance);
                }
                sampleToReflect.PossibleNeighbours[0].Add(id);
                sampleToReflect.PossibleNeighbours[1].Add(id);
            }
            if (axis == Axis.Y)
            {
                neighbours[2] = new HashSet<int>(sampleToReflect.PossibleNeighbours[3]);
                neighbours[3] = new HashSet<int>(sampleToReflect.PossibleNeighbours[2]);
                foreach (var instance in sampleToReflect.Instances)
                {
                    int newY = (int)(Anchor.y + (Anchor.y - instance.Pose.position.y));
                    var newInstance = instance;
                    newInstance.Pose.position.y = newY;
                    instances.Add(newInstance);
                }
                sampleToReflect.PossibleNeighbours[2].Add(id);
                sampleToReflect.PossibleNeighbours[3].Add(id);
            }
            if (axis == Axis.Z)
            {
                neighbours[4] = new HashSet<int>(sampleToReflect.PossibleNeighbours[5]);
                neighbours[5] = new HashSet<int>(sampleToReflect.PossibleNeighbours[4]);
                foreach (var instance in sampleToReflect.Instances)
                {
                    int newZ = (int)(Anchor.z + (Anchor.z - instance.Pose.position.z));
                    var newInstance = instance;
                    newInstance.Pose.position.z = newZ;
                    instances.Add(newInstance);
                }
                sampleToReflect.PossibleNeighbours[4].Add(id);
                sampleToReflect.PossibleNeighbours[5].Add(id);
            }


            var name = $"sample {sampleToReflect.Id} type {sampleToReflect.Type} ref: {axis}";
            reflectedSample = new ALIS_Sample(id, sampleToReflect.Density, sampleToReflect.Type, neighbours, instances, name,_managerWFC);
            return reflectedSample;
        }*/

        private void CheckDuplicates()
        {
            /*var checkedSamples = new List<ALIS_Sample>();
            checkedSamples.Add(Samples[0]);
            checkedSamples.AddRange((from i in Samples
                                     from j in Samples.SkipWhile(j => j != i)
                                     where i != j
                                     where i.Id != 0 && j.Id != 0
                                     select MergeDuplicates(i, j)).Distinct());*/
            //Samples = checkedSamples.OrderBy(s => s.Id).ToList();

            Dictionary<ALIS_Sample, List<ALIS_Sample>> equalsBySample = new Dictionary<ALIS_Sample, List<ALIS_Sample>>();
            foreach (var sample in SampleLibrary.Values)
            {
                ALIS_Sample alisSample = sample as ALIS_Sample;
                List<ALIS_Sample> list = null;
                var found = false;
                foreach (var key in equalsBySample.Keys)
                {
                    if (!found && CompareDuplicateSamples(key as ALIS_Sample, alisSample as ALIS_Sample))
                    {
                        list = equalsBySample[key];
                        found = true;
                        list.Add(alisSample);
                    }
                }

                if (!found)
                {
                    list = new List<ALIS_Sample>();
                    equalsBySample.Add(alisSample, list);
                }
            }

            foreach (var key in equalsBySample.Keys)
            {
                var nodesConsideredEqual = equalsBySample[key];
                if (nodesConsideredEqual.Count > 0)
                {
                    foreach (var itemConsideredEqual in equalsBySample[key])
                    {
                        //merge sample possible neighbours
                        for (int i = 0; i < 6; i++)
                        {
                            key.PossibleNeighbours[i].UnionWith(itemConsideredEqual.PossibleNeighbours[i]);
                            foreach (var sample in SampleLibrary.Values.Where(s => key.PossibleNeighbours[i].Contains(s)))
                            {
                                if (i % 2 != 0)
                                {
                                    sample.PossibleNeighbours[i - 1].Add(key);
                                    sample.PossibleNeighbours[i - 1].Remove(itemConsideredEqual);
                                }
                                else
                                {
                                    sample.PossibleNeighbours[i + 1].Add(key);
                                    sample.PossibleNeighbours[i + 1].Remove(itemConsideredEqual);
                                }
                            }
                        }
                        SampleLibrary.Remove(itemConsideredEqual.Id);
                    }
                }
            }
            SampleLibrary = SampleLibrary.OrderBy(s => s.Key).ToDictionary(p=>p.Key,p=>p.Value);
        }


        private ALIS_Sample MergeDuplicates(ALIS_Sample sample1, ALIS_Sample sample2)
        {
            if (CompareDuplicateSamples(sample1, sample2))
            {
                for (int i = 0; i < sample1.PossibleNeighbours.Count; i++)
                {
                    sample1.PossibleNeighbours[i].UnionWith(sample2.PossibleNeighbours[i]);
                    sample2.PossibleNeighbours[i].UnionWith(sample1.PossibleNeighbours[i]);
                }
            }
            //samples.Remove(sample2);
            return sample1;
        }

        public static bool CompareDuplicateSamples(ALIS_Sample sample1, ALIS_Sample sample2)
        {
            if (sample1.Instances.Count != sample2.Instances.Count) return false;
            if (sample1.Instances.Count == 0 && sample1.Instances.Count == sample2.Instances.Count) return true;
            var instances1 = sample1.Instances;
            var instances2 = sample2.Instances;

            for (int i = 0; i < instances1.Count; i++)
            {
                if (instances2.Count(s => s.Pose == instances1[i].Pose) == 0) return false;
            }

            return true;
        }

        /*private ALIS_Sample RotateALISSample(ALIS_Sample sample, int id, Vector3 anchor, int timesRoated, int origSampleId)
        {
            var conn = sample.PossibleNeighbours;
            var newConn = new List<HashSet<ALIS_Sample>> { conn[2], conn[3], conn[1], conn[0], conn[4], conn[5] }; // check this for lefthand rotation
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

            return new ALIS_Sample(id, sample.Density, sample.Type, newConn, newInstances, name,_managerWFC);
        }*/
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

        List<HashSet<int>> GetNeighbours()
        {
            List<HashSet<int>> possibleNeighbours = new List<HashSet<int>>();
            for (int i = 0; i < Neighbours.Count; i++)
            {
                possibleNeighbours.Add(new HashSet<int>(Neighbours[i].Neighbours/*.Where(s=>s!= int.MaxValue)*/));
            }

            return possibleNeighbours;
        }

        public ALIS_Sample ToALIS_Sample(ManagerWFC managerWFC)
        {
            var name = $"sample {Id} type {Type} rot: 0 ";
            ALIS_Sample alis_Sample = new ALIS_Sample(Id, Density, Type, Instances, name, managerWFC);
            return alis_Sample;
        }
    }

    public class Neighbour
    {
        public List<int> Neighbours;

        private Neighbour() { }
        public Neighbour(List<int> neighbours)
        {
            Neighbours = neighbours;
        }
    }

    public struct Instance
    {
        public int DefinitionIndex;
        public Pose Pose;
    }
}