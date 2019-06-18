using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaveFunctionCollapse.Shared;
using System.Linq;

namespace WaveFunctionCollapse.Unity
{
    public class ManagerWFC : MonoBehaviour
    {
        [SerializeField]
        float _voxelSize;
        [SerializeField]
        Vector3Int _tileSize;
        [SerializeField]
        Vector3Int _WFCSize;
        [SerializeField]
        bool _rhino, _log, _interval, _rotate, _reflectX, _reflectY, _reflectZ, _merge;
        [SerializeField]
        public static string Path = @"D:\Unity\School\ALIS_WFC\WaveFunctionCollapseUnity\RhinoExporter\";
        [SerializeField]
        Material _blockMaterial;
        [SerializeField]
        GUISkin _guiSkin;
        [SerializeField]
        GameObject _goPlot;


        Dictionary<int, Sample> _sampleLibrary = new Dictionary<int, Sample>();
        WFC _waveFunctionCollapse;
        List<GameObject> goColorCubes = new List<GameObject>();
        IEnumerator _step;
        RhinoImporter _rhinoImporter;
        GridController _gridController;
        bool _colorCubes = true, _imported = false, _hasMesh;
        public static int Seed = 0;
        Dictionary<int, Tile> _tiles;
        bool _generateOne = false;

        public Vector3 CenterWFC;


        void Awake()
        {
            SharedLogger.CurrentLogger = new UnityLog(_log);

            /*if (_rhino) RhinoAwake();
            else RandomAwake();*/

            CenterWFC = Vector3.Scale(_WFCSize, _tileSize) * (_voxelSize / 2);
        }

        void RandomAwake()
        {
            InitialiseRandomSamples();

            _waveFunctionCollapse = new WFC(_WFCSize.x, _WFCSize.y, _WFCSize.z, _sampleLibrary);

            SetRandomSamples();
        }

        void RhinoAwake()
        {
            _rhinoImporter = new RhinoImporter();
            _rhinoImporter.InstantiateSamples(_tileSize, _rotate, _reflectX, _reflectY, _reflectZ, _merge, this);
            _sampleLibrary = _rhinoImporter.SampleLibrary;
            _hasMesh = _rhinoImporter.HasMesh;
            _tileSize = _rhinoImporter.TileSize;

            if (_hasMesh)
            {
                _voxelSize = _rhinoImporter.VoxelSize;
                _tiles = _rhinoImporter.Tiles;
            }

            Debug.Log($"{_sampleLibrary.Count} samples loaded");

            _waveFunctionCollapse = new WFC(_WFCSize.x, _WFCSize.y, _WFCSize.z, _sampleLibrary);

            //Add the samples connections to the wfc grid
            //foreach (var sample in _sampleLibrary) sample.AddConnectionsToWFC(_waveFunctionCollapse);
            //_waveFunctionCollapse.RemoveEmptyConnections();

            if (!_hasMesh) _gridController = new GridController(_tileSize, _voxelSize, _WFCSize);

            //SetPlotBoundaries();
        }

        void Start()
        {


        }

        void OnGUI()
        {
            int buttonHeight = 40;
            int buttonWidth = 180;
            int i = 1;
            int padding = 5;
            int s = buttonHeight + padding;
            GUI.skin = _guiSkin;

            if (_rhino)
            {
                //Before the Samples are imported
                Seed = int.TryParse(GUI.TextField(new Rect(s, s * i, buttonWidth / 2 - padding, buttonHeight), Seed.ToString()), out int l) ? l : 1;
                GUI.Label(new Rect(s + buttonWidth / 2, s * i++, buttonWidth / 2 - padding, buttonHeight), "Seed");
                if (!_imported)
                {
                    if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Import Rhino"))
                    {
                        RhinoAwake();
                        _imported = true;
                    }
                    
                    _reflectX = GUI.Toggle(new Rect(s, s * i++, buttonWidth, buttonHeight), _reflectX, "ReflectX");
                    _reflectY = GUI.Toggle(new Rect(s, s * i++, buttonWidth, buttonHeight), _reflectY, "ReflectY");
                    _reflectZ = GUI.Toggle(new Rect(s, s * i++, buttonWidth, buttonHeight), _reflectZ, "ReflectZ");
                    _merge = GUI.Toggle(new Rect(s, s * i++, buttonWidth, buttonHeight), _merge, "Merge");
                    _rotate = GUI.Toggle(new Rect(s, s * i++, buttonWidth, buttonHeight), _rotate, "Rotate");

                    _WFCSize.x = int.TryParse(GUI.TextField(new Rect(s, s * ++i, buttonWidth / 2 - padding, buttonHeight), _WFCSize.x.ToString()), out int m) ? m : 1;
                    GUI.Label(new Rect(s + buttonWidth / 2, s * i++, buttonWidth / 2 - padding, buttonHeight), "WFC X");
                    _WFCSize.y = int.TryParse(GUI.TextField(new Rect(s, s * i, buttonWidth / 2 - padding, buttonHeight), _WFCSize.y.ToString()), out int n) ? n : 1;
                    GUI.Label(new Rect(s + buttonWidth / 2, s * i++, buttonWidth / 2 - padding, buttonHeight), "WFC Y");
                    _WFCSize.z = int.TryParse(GUI.TextField(new Rect(s, s * i, buttonWidth / 2 - padding, buttonHeight), _WFCSize.z.ToString()), out int o) ? o : 1;
                    GUI.Label(new Rect(s + buttonWidth / 2, s * i++, buttonWidth / 2 - padding, buttonHeight), "WFC Z");

                }

                //When the samples are imported
                else if (_imported)
                {
                    // Right screen GUI
                    // show progress bar
                    int labelCounter = 1;
                    if (GUI.Button(new Rect(Screen.width - buttonWidth - s, s * labelCounter++, buttonWidth, buttonHeight), "Toggle colorcubes"))
                    {
                        _colorCubes = !_colorCubes;
                        HideColorCubes(_colorCubes);
                    }
                    GUI.HorizontalSlider(new Rect(Screen.width - buttonWidth - s, s * labelCounter++, buttonWidth, buttonHeight), _waveFunctionCollapse.Progress, 0, 1);

                    //Show labels

                    if (_reflectX) GUI.Label(new Rect(Screen.width - buttonWidth - s, s * labelCounter++, buttonWidth, buttonHeight), "Reflect X");
                    if (_reflectY) GUI.Label(new Rect(Screen.width - buttonWidth - s, s * labelCounter++, buttonWidth, buttonHeight), "Reflect Y");
                    if (_reflectZ) GUI.Label(new Rect(Screen.width - buttonWidth - s, s * labelCounter++, buttonWidth, buttonHeight), "Reflect Z");
                    if (_merge) GUI.Label(new Rect(Screen.width - buttonWidth - s, s * labelCounter++, buttonWidth, buttonHeight), "Merge");
                    if (_rotate) GUI.Label(new Rect(Screen.width - buttonWidth - s, s * labelCounter++, buttonWidth, buttonHeight), "Rotate");


                    //leftScreen ui
                    if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Generate"))
                    {
                        if (_interval)
                        {
                            UtilShared.RandomNR = new System.Random(Seed);
                            _step = Step(0.5f);
                            StartCoroutine(_step);
                        }
                        else
                        {
                            _waveFunctionCollapse.Execute();
                            DrawGrid();
                        }

                    }
                    if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Generate 1 seed"))
                    {
                        _imported = true;
                        _generateOne = true;
                        if (_interval)
                        {
                            UtilShared.RandomNR = new System.Random(Seed);
                            _step = Step(0.5f);
                            StartCoroutine(_step);
                        }
                        else
                        {
                            _waveFunctionCollapse.Execute();
                            DrawGrid();
                        }
                    }


                    

                    //When the aggregation is finished
                    if (_waveFunctionCollapse == null ? false : _waveFunctionCollapse.IsAllDetermined || _waveFunctionCollapse.HasContradiction)
                    {
                        if (!_hasMesh)
                        {
                            i = _gridController.OnGUI(buttonHeight, buttonWidth, i, s);

                            if (!_gridController.ShowStructuralAnalysis)
                            {
                                if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Regenerate"))
                                {
                                    Regenerate();
                                }
                            }
                        }
                        if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Export to rhino"))
                        {
                            RhinoExporter.Export(_waveFunctionCollapse.SelectedSamples.Select(t => t as ALIS_Sample).ToList(), _WFCSize, _tileSize);
                        }

                        if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Export to ALIS allocation"))
                        {
                            VoxelExporter.ExportVoxels(_gridController);
                        }
                        /*if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Import voxel grid"))
                        {
                            VoxelImporter.ImportVoxels();
                        }*/
                    }


                }
            }
        }



        private void Regenerate()
        {
            StopCoroutine(_step);
            ClearGameobjects();
            _waveFunctionCollapse.Reset();
            if (!_hasMesh)
            {
                _gridController.Reset();
            }
            _step = Step(0.1f);
            StartCoroutine(_step);

        }

        void HideColorCubes(bool flag)
        {
            for (int i = 0; i < goColorCubes.Count; i++)
            {
                goColorCubes[i].GetComponent<MeshRenderer>().enabled = flag;
            }
        }

        IEnumerator Step(float time)
        {
            while (true)
            {
                //Debug.Log("Step");
                //DrawSamples(_waveFunctionCollapse.Step(1));
                _waveFunctionCollapse.Step(1);

                if (_waveFunctionCollapse.HasContradiction)
                {
                    if (!_generateOne)
                    {
                        Seed++;
                        UtilShared.RandomNR = new System.Random(Seed);
                        Regenerate();
                    }
                    yield break;
                }

                if (_waveFunctionCollapse.IsAllDetermined)
                {
                    //DrawGrid();
                    yield break;
                    //StopCoroutine(_step);
                }
                yield return new WaitForSeconds(time);
            }
        }


        void Update()
        {
            if (_imported && _rhino && !_hasMesh) _gridController.Update();
        }

        public void InitialiseRandomSamples()
        {
            for (int i = 0; i < 20; i++)
            {
                _sampleLibrary.Add(i, new ALIS_Sample(i));
            }
        }

        public void SetRandomSamples()
        {
            for (int i = 1; i < _sampleLibrary.Count; i++)
            {
                var sample = _sampleLibrary[i] as ALIS_Sample;
                sample.SetRandomNeighbours(10, _waveFunctionCollapse);
            }
        }

        public void SetPlotBoundaries()
        {
            List<MeshCollider> colliders = new List<MeshCollider>();
            colliders.Add(_goPlot.GetComponent<MeshCollider>());
            for (int i = 0; i < _waveFunctionCollapse.SelectedSamples.Count; i++)
            {
                Vector3Int index = Util.ToUnityVector3Int(_waveFunctionCollapse.GetIndexOfPossibleSample(i));
                Vector3 position = Vector3.Scale(index, Vector3.Scale(Vector3.one * _voxelSize, _tileSize)) + ((Vector3)_tileSize - Vector3.one) * _voxelSize / 2;

                if (!Util.IsInside(colliders, position))
                {
                    _waveFunctionCollapse.SetSample(i, _sampleLibrary[0]);
                }

            }
        }

        public void ClearGameobjects()
        {
            foreach (var go in goColorCubes)
            {
                GameObject.Destroy(go);
            }
            goColorCubes.Clear();
        }


        public void DrawGrid()
        {
            ClearGameobjects();
            for (int i = 0; i < _waveFunctionCollapse.SelectedSamples.Count; i++)
            {
                DrawSample(i);
            }
        }

        /*public void DrawSamples(List<int> SampleIndices)
        {
            foreach (var sampleIndex in SampleIndices)
            {
                DrawSample(sampleIndex);
            }
        }*/

        public void DrawSample(int sampleIndex)
        {
            var sample = _waveFunctionCollapse.SelectedSamples[sampleIndex];
            if (sample.Id != 0)
            {
                ALIS_Sample selectedSample = sample as ALIS_Sample;
                Vector3Int index = Util.ToUnityVector3Int(_waveFunctionCollapse.GetIndexOfPossibleSample(sampleIndex));
                GameObject goTile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                goTile.transform.localScale = Vector3.Scale(Vector3.one * _voxelSize, _tileSize);
                goTile.transform.localPosition = Vector3.Scale(index, goTile.transform.localScale) + ((Vector3)_tileSize - Vector3.one) * _voxelSize / 2;
                goTile.name = $"tile: {sampleIndex}{index} {selectedSample.Name}";
                goTile.GetComponent<MeshRenderer>().enabled = _colorCubes;

                Material mat = goTile.GetComponent<Renderer>().material;
                mat.color = selectedSample.Col;
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.DisableKeyword("_ALPHABLEND_ON");
                mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;

                if (_rhino)
                {
                    if (!_hasMesh)
                    {
                        _gridController.Generate(selectedSample, Util.ToUnityVector3Int(_waveFunctionCollapse.GetIndexOfPossibleSample(sampleIndex)), _tileSize, goTile.transform);
                    }
                    else
                    {
                        InstantiateGOMesh(selectedSample, Util.ToUnityVector3Int(_waveFunctionCollapse.GetIndexOfPossibleSample(sampleIndex)), goTile.transform);
                    }
                }

                goColorCubes.Add(goTile);
            }
        }

        public void InstantiateGOMesh(ALIS_Sample selectedSample, Vector3Int tileIndex, Transform goTile)
        {
            foreach (var instance in selectedSample.Instances)
            {

                /*var position = instance.Pose.position - ((Vector3)_tileSize * _voxelSize / 2);
                position += (Vector3)(tileIndex * _tileSize) * _voxelSize;*/


                var position = (Vector3)(tileIndex * _tileSize) * _voxelSize; // tile position
                position += instance.Pose.position * _voxelSize;
                var rotation = instance.Pose.rotation;
                var go = _tiles[instance.DefinitionIndex].Instantiate(new Pose(position, rotation), _blockMaterial);
                go.transform.SetParent(goTile, true);


                //GameObject tile = GameObject.Instantiate(_tiles[instance.DefinitionIndex].GetGoTile(), instance.Pose.position + tileIndex * _tileSize,instance.Pose.rotation,goTile);
            }
        }
    }
}
