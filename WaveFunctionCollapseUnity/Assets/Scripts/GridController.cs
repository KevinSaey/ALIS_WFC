using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace WaveFunctionCollapse.Unity
{
    public class GridController
    {
        public static Vector3Int Size;
        public static float VoxelSize;
        public static int  MinCon;

        Block startBlock;
        public static Grid3D _grid;

        bool _showPath;
        bool _showStructuralAnalysis;
        bool _showBlocks;
        bool _iniPath = true;
        bool _iniSA = true;

        float _tempDisplacement = 10f;


        public GridController(Vector3Int size, float voxelSize)
        {
            Size = size;
            VoxelSize = voxelSize;

            _grid = new Grid3D(Size,VoxelSize);

            var pattern = new PatternA();
            //startBlock = new Block(pattern, new Vector3Int(Size.x / 2, 1, Size.y / 2), new Vector3Int(0, 180, 0), _grid);

            //_grid.AddBlockToGrid(startBlock);
            _grid.IniPathFindingStrucutralAnalysis();

            //StartCoroutine(NextBlockOverTime()); //To generate blocks over time
        }

        public void OnGUI() //Vicente
        {
            int buttonHeight = 30;
            int buttonWidth = 150;
            int i = 1;
            int s = buttonHeight + 5;

            if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Show Blocks"))
            {
                _showPath = false;
                _showStructuralAnalysis = false;
                _showBlocks = true;
                _grid.SwitchBlockVisibility(_showBlocks);
            }
            if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Show Structural Analysis")) //DO NOT USE! Not working yet
            {
                if (_iniSA == true)
                {
                    _grid.SAnalysis.Analysis();
                    _iniSA = false;
                }
                _showPath = false;
                _showStructuralAnalysis = true;
                _showBlocks = false;
                _grid.SwitchBlockVisibility(_showBlocks);
            }
            _tempDisplacement = GUI.HorizontalSlider(new Rect(s, s * i++, buttonWidth, buttonHeight), _tempDisplacement, 0, 500);
            if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Show Graph"))
            {
                if (_iniPath == true)
                {
                    _grid.PFinding.Regenerate();
                    _iniPath = false;
                }

                _showPath = true;
                _showStructuralAnalysis = false;
                _showBlocks = false;
                _grid.SwitchBlockVisibility(_showBlocks);
            }
        }

        public void Update()
        {
            if (_showPath)
            {
                _grid.PFinding.DrawMesh();
            }
            if (_showStructuralAnalysis)
            {
                _grid.SAnalysis.DrawMesh(_tempDisplacement);
            }
        }

        public void Generate(ALIS_Sample sample, Vector3Int tileIndex, Vector3Int tileSize)
        {
            var pattern = new PatternC();
            foreach (var instance in sample.Instances)
            {
                var rotation = instance.Pose.rotation.eulerAngles.ToVector3Int();
                /*Debug.Log("imported rotation " + rotation);
                if (rotation.x == -90) rotation.x = 270;
                if (rotation.y == -90) rotation.x = 270;
                if (rotation.z == -90) rotation.x = 270;
                Debug.Log("adjusted rotation " + rotation);*/

                var block = new Block(pattern, instance.Pose.position.ToVector3Int() + tileIndex * tileSize, rotation, _grid);
                _grid.AddBlockToGrid(block);
            }
        }

    }

    
}