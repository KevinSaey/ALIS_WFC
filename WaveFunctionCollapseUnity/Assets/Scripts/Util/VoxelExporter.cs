﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using WaveFunctionCollapse.Shared;

namespace WaveFunctionCollapse.Unity
{
    public static class VoxelImporter
    {
        public static ExportVoxelGrid ImportVoxels()
        {
            int seed = 160;
            string _path = $@"D:\Unity\School\ALIS_WFC\WaveFunctionCollapseUnity\RhinoExporter\Export\Export_Voxels_Seed{seed}.xml";
            var importedGrid = ExportVoxelGrid.Import(_path);

            return importedGrid;
        }

        public static ExportVoxel[,,] toVoxelArray(ExportVoxelGrid exportVoxelGrid)
        {
            List<ExportVoxel> exportVoxelList = exportVoxelGrid.ExportVoxels;
            Vector3Int gridDimensions = exportVoxelGrid.GridDimension;

            ExportVoxel[,,] voxelArray = new ExportVoxel[gridDimensions.x, gridDimensions.y, gridDimensions.z];
            foreach (var vox in exportVoxelList)
            {
                voxelArray[vox.Index.x, vox.Index.y, vox.Index.z] = vox;
            }

            return voxelArray;
        }
    }

    public static class VoxelExporter
    {
        public static bool ExportVoxels(GridController gridControl,string path)
        {
            int seed = ManagerWFC.Seed;
            if (!Directory.Exists(path))
            {
                SharedLogger.Log("Error: Path doesn't exist - function: Export");
                return false;
            }
            string filename = $@"{path}\Export\Export_Voxels_Seed{seed}.xml";

            List<ExportVoxel> exportVoxels = new List<ExportVoxel>();
            for (int x = 0; x < GridController.Size.x; x++)
            {
                for (int y = 0; y < GridController.Size.y; y++)
                {
                    for (int z = 0; z < GridController.Size.z; z++)
                    {
                        exportVoxels.Add(new ExportVoxel(GridController.Grid.Voxels[x, y, z].Index, (int)GridController.Grid.Voxels[x, y, z].Type));
                    }
                }
            }


            ExportVoxelGrid.Export(exportVoxels, GridController.Size, filename);
            return true;
        }
    }

    public class ExportVoxelGrid
    {
        public List<ExportVoxel> ExportVoxels;
        public Vector3Int GridDimension;

        public static void Export(List<ExportVoxel> exportVoxels, Vector3Int gridDimension, string fileName)
        {
            var exportVoxelGrid = new ExportVoxelGrid()
            {
                ExportVoxels = exportVoxels,
                GridDimension = gridDimension
            };

            var serializer = new XmlSerializer(typeof(ExportVoxelGrid));
            using (var writer = XmlWriter.Create(fileName))
            {
                serializer.Serialize(writer, exportVoxelGrid);
            }
        }

        public static ExportVoxelGrid Import(string fileName)
        {
            var serializer = new XmlSerializer(typeof(ExportVoxelGrid));
            using (var reader = XmlReader.Create(fileName))
            {
                return serializer.Deserialize(reader) as ExportVoxelGrid;
            }
        }
    }

    public class ExportVoxel
    {
        public Vector3Int Index;
        public int Type;

        public ExportVoxel()
        {

        }
        public ExportVoxel(Vector3Int index, int type)
        {
            Index = index;
            Type = type;
        }
    }


}