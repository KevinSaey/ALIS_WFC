using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using WaveFunctionCollapse.Unity;

namespace WaveFunctionCollapse.Unity
{
    public class GridController
    {
        public static Vector3Int Size;
        public static float VoxelSize;

        Block startBlock;
        public static Grid3D _grid;

        bool _showPath;
        public bool ShowStructuralAnalysis { get; private set; }
        bool _showBlocks;
        float _tempDisplacement = 10f;


        public GridController(Vector3Int tileSize, float voxelSize, Vector3Int wfcSize)
        {
            Size = tileSize*wfcSize;
            VoxelSize = voxelSize;

            _grid = new Grid3D(Size,VoxelSize);

            _grid.IniPathFindingStrucutralAnalysis();
        }

        public int OnGUI(int buttonHeight, int buttonWidth, int i, int s) //Vicente
        {
            

            if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Show Blocks"))
            {
                _showPath = false;
                ShowStructuralAnalysis = false;
                _showBlocks = true;
                _grid.SwitchBlockVisibility(_showBlocks);
            }
            if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Show Structural Analysis")) 
            {
                                _grid.SAnalysis.Analysis();

                _showPath = false;
                ShowStructuralAnalysis = true;
                _showBlocks = false;
                _grid.SwitchBlockVisibility(_showBlocks);
            }
            _grid.SAnalysis.Deflection = GUI.Toggle(new Rect(s, s * i++, buttonWidth, buttonHeight), _grid.SAnalysis.Deflection, "Displacement - Force");
            _tempDisplacement = GUI.HorizontalSlider(new Rect(s, s * i++, buttonWidth, buttonHeight), _tempDisplacement, 0, 500);
            /*if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Show Graph"))
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
            }*/
            return i;
        }

        public void Update()
        {
            if (_showPath)
            {
                _grid.PFinding.DrawMesh();
            }
            if (ShowStructuralAnalysis)
            {
                _grid.SAnalysis.DrawMesh(_tempDisplacement);
            }
        }

        public void Generate(ALIS_Sample sample, Vector3Int tileIndex, Vector3Int tileSize, Transform parrent)
        {
            var pattern = new PatternC();
            foreach (var instance in sample.Instances)
            {
                var rotation = instance.Pose.rotation.eulerAngles.ToVector3IntRound();
                
                var block = new Block(pattern, instance.Pose.position.ToVector3IntRound() + tileIndex * tileSize, rotation, _grid);
                _grid.AddBlockToGrid(block,parrent);
            }
        }

        public void Reset()
        {
            _grid.Reset();
        }
    }

    
}