﻿using System.Collections;
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

        public RhinoImporter(/*Grid3D grid*/)
        {
            var files = LoadFiles();
            Debug.Log($"{files.Count()} ALIS_samples loaded");
            //_grid = grid;
            

            for (int i = 0; i < files.Count; i++)
            {
                Samples.Add(Assembly.Import(files[i]).ToALIS_Sample());
            }

            //Assembly.Import(_path).Generate(_grid);
        }

        public List<string> LoadFiles()
        {
            return Directory.GetFiles(_path, "*.xml").ToList();
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
            List<HashSet<int>> possibleNeighbours = new List<HashSet<int>>();
            for (int i = 0; i < Neighbours.Count; i++)
            {
                possibleNeighbours.Add(new HashSet<int>(Neighbours[i].Neighbours));
            }

            ALIS_Sample alis_Sample = new ALIS_Sample(Id,Density,Type,possibleNeighbours,Instances);
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

    public class Instance
    {
        public int DefinitionIndex;
        public Pose Pose;
    }
}