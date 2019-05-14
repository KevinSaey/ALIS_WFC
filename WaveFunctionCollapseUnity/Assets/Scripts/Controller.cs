using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace WaveFunctionCollapse.Unity
{
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        GameObject _goVoxel;
        public static GameObject GoVoxel;

        [SerializeField]
        GameObject _goTarget;
        public static GameObject GoTarget;

        [SerializeField]
        Material _matBlock, _matConnection;
        public static Material MatBlock, MatConnection;

        [SerializeField]
        Vector3Int _size;
        public static Vector3Int Size;

        [SerializeField]
        int _voxelSize, _minCon;
        public static int VoxelSize, MinCon;

        Block startBlock;
        public static Grid3D Grid;

        bool _showPath;
        bool _showStructuralAnalysis;
        bool _showBlocks;
        bool _iniPath = true;
        bool _iniSA = true;

        float _tempDisplacement = 10f;

        public RhinoImporter RhinoImport;

        void Start()
        {
            GoVoxel = _goVoxel;
            MatBlock = _matBlock;
            MatConnection = _matConnection;
            GoTarget = _goTarget;
            Size = _size;
            VoxelSize = _voxelSize;
            MinCon = _minCon;

            Grid = new Grid3D(Size);

            var pattern = new PatternA();
            //startBlock = new Block(pattern, new Vector3Int(Size.x / 2, 1, Size.y / 2), new Vector3Int(0, 180, 0), _grid);

            //_grid.AddBlockToGrid(startBlock);
            Grid.IniPathFindingStrucutralAnalysis();

            //StartCoroutine(NextBlockOverTime()); //To generate blocks over time
        }

        void OnGUI() //Vicente
        {
            int buttonHeight = 30;
            int buttonWidth = 150;
            int i = 1;
            int s = buttonHeight + 5;

            if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Import from rhino"))
            {
                //RhinoImport = new RhinoImporter(Grid);
            }
            if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Show Blocks"))
            {
                _showPath = false;
                _showStructuralAnalysis = false;
                _showBlocks = true;
                Grid.SwitchBlockVisibility(_showBlocks);
            }
            if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Show Structural Analysis")) //DO NOT USE! Not working yet
            {
                if (_iniSA == true)
                {
                    Grid.SAnalysis.Analysis();
                    _iniSA = false;
                }
                _showPath = false;
                _showStructuralAnalysis = true;
                _showBlocks = false;
                Grid.SwitchBlockVisibility(_showBlocks);
            }
            _tempDisplacement = GUI.HorizontalSlider(new Rect(s, s * i++, buttonWidth, buttonHeight), _tempDisplacement, 0, 500);
            if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Show Graph"))
            {
                if (_iniPath == true)
                {
                    Grid.PFinding.Regenerate();
                    _iniPath = false;
                }

                _showPath = true;
                _showStructuralAnalysis = false;
                _showBlocks = false;
                Grid.SwitchBlockVisibility(_showBlocks);
            }
            if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Generate Next Block"))
            {
                NextBlock();
                Grid.SAnalysis.Analysis();
                Grid.SwitchBlockVisibility(_showBlocks);
            }
        }

        void Update()
        {
            if (_showPath)
            {
                Grid.PFinding.DrawMesh();
            }
            if (_showStructuralAnalysis)
            {
                Grid.SAnalysis.DrawMesh(_tempDisplacement);
            }
        }

        /// <summary>
        /// Generate the next block
        /// </summary>
        public void NextBlock()
        {
            var block = Grid.GenerateNextBlock();
            Grid.SwitchBlockVisibility(!_showPath);
            Grid.PFinding.Regenerate();
            Grid.SAnalysis.Analysis();
            Debug.Log("NextBlock");
        }

        /// <summary>
        /// Generate a next block over time
        /// </summary>
        /// <returns></returns>
        IEnumerator NextBlockOverTime(float time)
        {
            while (true)
            {
                yield return new WaitForSeconds(time);

                var block = Grid.GenerateNextBlock();
                Grid.SwitchBlockVisibility(!_showPath);
                Grid.PFinding.Regenerate();
                Grid.SAnalysis.Analysis();
                Debug.Log("NextBlock");
            }
        }
    }
}