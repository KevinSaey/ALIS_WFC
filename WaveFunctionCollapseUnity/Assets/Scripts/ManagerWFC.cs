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
        Material _blockMaterial, _transparentMaterial;
        [SerializeField]
        GUISkin _guiSkin;
        [SerializeField]
        GameObject _goPlot;
        [SerializeField]
        GameObject _goRecorder;
        [SerializeField]
        Texture2D _alisLogo;
        [SerializeField]
        GameObject _goFloor;

        Dictionary<int, Sample> _sampleLibrary = new Dictionary<int, Sample>();
        WFC _waveFunctionCollapse;
        List<GameObject> _goColorCubes = new List<GameObject>();
        IEnumerator _step, _regenerate;
        RhinoImporter _rhinoImporter;
        GridController _gridController;
        bool _colorCubes = false, _imported = false, _hasMesh, _plot = true, _record = false;
        Dictionary<int, ImportedTile> _tiles;
        bool _generateOne = false;
        Dictionary<int, float> _bestSeedsProgress = new Dictionary<int, float>();
        List<int> _bestSeeds = new List<int>();
        CaptureCamera _recorder;
        Stack<ALIS_Tile> _generatedTiles;
        int _historySteps = 5;
        Dictionary<int, float> _generationTime = new Dictionary<int, float>();
        float _runtime = 0f, _generationStartTime = 0f;
        int _currentPlot = 0;

        public static int Seed = 0;
        public Vector3 CenterWFC;
        public string Warnings;
        public string Path = @"C:\Users\Kevin\Desktop\WFC";
        public bool RotateCam = true;



        void Awake()
        {
            SharedLogger.CurrentLogger = new UnityLog(_log);
            _recorder = _goRecorder.GetComponent<CaptureCamera>();
            
            _generatedTiles = new Stack<ALIS_Tile>();

            ManagerPlot.CreateGoPlot(new Material(_transparentMaterial));
            CenterWFC = ManagerPlot.CentrePlot(_currentPlot);
        }

        void RandomAwake()
        {
            InitialiseRandomSamples();

            _waveFunctionCollapse = new WFC(_WFCSize.x, _WFCSize.y, _WFCSize.z, _sampleLibrary);

            SetRandomSamples();
        }

        bool RhinoAwake()
        {


            bool rhinoAwakeSucces = false;
            _rhinoImporter = new RhinoImporter();
            rhinoAwakeSucces = _rhinoImporter.InstantiateSamples(_rotate, _reflectX, _reflectY, _reflectZ, _merge, this);
            if (rhinoAwakeSucces)
            {
                _sampleLibrary = _rhinoImporter.SampleLibrary;
                _hasMesh = _rhinoImporter.HasMesh;
                _tileSize = _rhinoImporter.TileSize;

                if (_hasMesh)
                {
                    _voxelSize = _rhinoImporter.VoxelSize;
                    _tiles = _rhinoImporter.Tiles;
                }

                Debug.Log($"{_sampleLibrary.Count} samples loaded");
            }

            _WFCSize = ManagerPlot.GetWFCSize(_currentPlot, _voxelSize, _tileSize);
            //SetPlotBoundaries();
            
            return rhinoAwakeSucces;
        }

        void SetUpWFC()
        {
            
            _waveFunctionCollapse = new WFC(_WFCSize.x, _WFCSize.y, _WFCSize.z, _sampleLibrary);

            if (!_hasMesh) _gridController = new GridController(_tileSize, _voxelSize, _WFCSize);
            _waveFunctionCollapse.HistorySteps = _historySteps;

            CenterWFC = Vector3.Scale(_WFCSize, _tileSize) * _voxelSize / 2;
        }

        void Start()
        {

        }

        void OnGUI()
        {
            int buttonHeight = 35;
            int buttonWidth = 210;
            int i = 1;
            int padding = 5;
            int s = buttonHeight + padding;
            GUI.skin = _guiSkin;


            int leftPanelWidth = buttonWidth + 4 * padding + buttonHeight;
            GUI.Box(new Rect(0, 0, leftPanelWidth, Screen.height), "");

            //Alis logo
            GUI.DrawTexture(new Rect(s, padding, buttonHeight * 5, buttonHeight * 5), _alisLogo, ScaleMode.ScaleToFit);
            i += 4;

            //Warnings
            GUI.Label(new Rect(s, Screen.height - s, padding + 500, Screen.height - s), Util.Warning);
            RotateCam = GUI.Toggle(new Rect(s, Screen.height - s, buttonWidth, buttonHeight), RotateCam, "Rotate camera");

            if (_rhino)
            {
                Path = GUI.TextField(new Rect(s, s * i++, buttonWidth - padding, buttonHeight), Path);

                //Before the Samples are imported
                Seed = int.TryParse(GUI.TextField(new Rect(s, s * i, buttonWidth / 2 - padding, buttonHeight), Seed.ToString()), out int l) ? l : 1;

                GUI.Label(new Rect(s + buttonWidth / 2, s * i++, buttonWidth / 2 - padding, buttonHeight), "Seed");

                _record = GUI.Toggle(new Rect(s, s * i++, buttonWidth, buttonHeight), _record, "Record");
                _recorder.EnableCapture = _record;

                if (!_imported)
                {
                    var rect = new Rect(s, s * i++, buttonWidth, buttonHeight);
                    if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Import Rhino"))
                    {
                        _imported = RhinoAwake();
                        _recorder.SetPath(Path);
                    }

                    _reflectX = GUI.Toggle(new Rect(s, s * i++, buttonWidth, buttonHeight), _reflectX, "ReflectX");
                    _reflectY = GUI.Toggle(new Rect(s, s * i++, buttonWidth, buttonHeight), _reflectY, "ReflectY");
                    _reflectZ = GUI.Toggle(new Rect(s, s * i++, buttonWidth, buttonHeight), _reflectZ, "ReflectZ");
                    _merge = GUI.Toggle(new Rect(s, s * i++, buttonWidth, buttonHeight), _merge, "Merge");
                    _rotate = GUI.Toggle(new Rect(s, s * i++, buttonWidth, buttonHeight), _rotate, "Rotate");
                    i++;
                }

                // set-up WFC
                else if (_imported && _waveFunctionCollapse == null)
                {
                    if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Set-up WFC"))
                    {
                        SetUpWFC();
                    }
                    if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Next plot"))
                    {
                        NextPlot();
                    }
                    _plot = GUI.Toggle(new Rect(s, s * i++, buttonWidth, buttonHeight), _plot, "Plot");
                    ManagerPlot.TogglePlot(_plot);

                    _WFCSize.x = int.TryParse(GUI.TextField(new Rect(s, s * ++i, buttonWidth / 2 - padding, buttonHeight), _WFCSize.x.ToString()), out int m) ? m : 1;
                    GUI.Label(new Rect(s + buttonWidth / 2, s * i++, buttonWidth / 2 - padding, buttonHeight), "WFC X");
                    _WFCSize.y = int.TryParse(GUI.TextField(new Rect(s, s * i, buttonWidth / 2 - padding, buttonHeight), _WFCSize.y.ToString()), out int n) ? n : 1;
                    GUI.Label(new Rect(s + buttonWidth / 2, s * i++, buttonWidth / 2 - padding, buttonHeight), "WFC Y");
                    _WFCSize.z = int.TryParse(GUI.TextField(new Rect(s, s * i, buttonWidth / 2 - padding, buttonHeight), _WFCSize.z.ToString()), out int o) ? o : 1;
                    GUI.Label(new Rect(s + buttonWidth / 2, s * i++, buttonWidth / 2 - padding, buttonHeight), "WFC Z");
                    i++;
                    _historySteps = int.TryParse(GUI.TextField(new Rect(s, s * ++i, buttonWidth / 2 - padding, buttonHeight), _historySteps.ToString()), out int p) ? p : 1;
                    GUI.Label(new Rect(s + buttonWidth / 2, s * i++, buttonWidth / 2 - padding, buttonHeight), "History steps");
                }

                //When the samples are imported
                else if (_imported && _waveFunctionCollapse != null)
                {
                    // Right screen GUI
                    // show progress bar
                    int labelCounter = 1;
                    if (GUI.Button(new Rect(Screen.width - buttonWidth - s, s * labelCounter++, buttonWidth, buttonHeight), "Toggle colorcubes"))
                    {
                        _colorCubes = !_colorCubes;
                        HideColorCubes(_colorCubes);
                    }
                    
                    labelCounter++;

                    //Show labels
                    GUI.Label(new Rect(Screen.width - buttonWidth - s, s * labelCounter++, buttonWidth, buttonHeight), $"WFC dimension X: {_WFCSize.x} Y: {_WFCSize.y} Z: {_WFCSize.z}");

                    if (_reflectX) GUI.Label(new Rect(Screen.width - buttonWidth - s, s * labelCounter++, buttonWidth, buttonHeight), "Reflect X");
                    if (_reflectY) GUI.Label(new Rect(Screen.width - buttonWidth - s, s * labelCounter++, buttonWidth, buttonHeight), "Reflect Y");
                    if (_reflectZ) GUI.Label(new Rect(Screen.width - buttonWidth - s, s * labelCounter++, buttonWidth, buttonHeight), "Reflect Z");
                    if (_merge) GUI.Label(new Rect(Screen.width - buttonWidth - s, s * labelCounter++, buttonWidth, buttonHeight), "Merge");
                    if (_rotate) GUI.Label(new Rect(Screen.width - buttonWidth - s, s * labelCounter++, buttonWidth, buttonHeight), "Rotate");
                    labelCounter++;

                    if (_generationTime.Count > 0)
                        GUI.Label(new Rect(Screen.width - buttonWidth - s, s * labelCounter++, buttonWidth, buttonHeight), $"Last generation time: {Mathf.Round(_generationTime.Last().Value)} seconds");

                    labelCounter++;
                    GUI.Label(new Rect(Screen.width - buttonWidth - s, s * labelCounter++, buttonWidth, buttonHeight), "Best seeds");
                    for (int j = 0; j < _bestSeeds.Count; j++)
                    {
                        GUI.Label(new Rect(Screen.width - buttonWidth - s, s * labelCounter++, buttonWidth, buttonHeight),
                            $"Seed {_bestSeeds[j].ToString()} progres: {(int)(_bestSeedsProgress[_bestSeeds[j]] * 100)} %");
                    }

                    //ProgressBar
                    GUI.HorizontalSlider(new Rect(leftPanelWidth + s, Screen.height - s, Screen.width - 2 * s - leftPanelWidth, buttonHeight), _waveFunctionCollapse.Progress, 0, 1);

                    //leftScreen ui
                    if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Reset weights"))
                    {
                        ResetWeight();
                        SharedLogger.Log($"{_sampleLibrary.Count} samples Weights reset");
                    }

                    if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Generate"))
                    {
                        SetPlotBoundaries();
                        _regenerate = Regenerate();
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

                        SetPlotBoundaries();
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
                                    StartCoroutine(Regenerate());
                                }
                            }
                        }
                        if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Export to rhino"))
                        {
                            RhinoExporter.Export(_waveFunctionCollapse.Tiles.Where(t => t.Set && t.Enabled).ToList(), _WFCSize, _tileSize, _voxelSize, Path, _hasMesh);
                        }

                        if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Export to ALIS allocation"))
                        {
                            VoxelExporter.ExportVoxels(_gridController, Path);
                        }
                    }
                }
            }
        }

        IEnumerator Regenerate()
        {
            EndGeneration();
            StopCoroutine(_step);
            yield return new WaitForSeconds(0.1f);

            ClearGameobjects();
            _waveFunctionCollapse.Reset();
            if (!_hasMesh)
            {
                _gridController.Reset();
            }
            _step = Step(0.1f);
            SetPlotBoundaries();
            StartCoroutine(_step);
            yield break;
        }

        void HideColorCubes(bool flag)
        {
            for (int i = 0; i < _goColorCubes.Count; i++)
            {
                _goColorCubes[i].GetComponent<MeshRenderer>().enabled = flag;
            }
        }

        IEnumerator Step(float time)
        {
            _generationStartTime = _runtime;
            while (true)
            {
                _waveFunctionCollapse.Step(1);
                while (_goColorCubes.Count > _waveFunctionCollapse.NrSetSamples)
                {
                    DeleteLastTile();
                }

                if (_waveFunctionCollapse.HasContradiction)
                {
                    StartCoroutine(Regenerate());
                    if (!_generateOne)
                    {
                        CheckSeed(Seed, _waveFunctionCollapse.Progress);
                        Seed++;
                        UtilShared.RandomNR = new System.Random(Seed);

                    }

                    yield break;
                }

                if (_waveFunctionCollapse.IsAllDetermined)
                {
                    EndGeneration();
                    //DrawGrid();
                    yield break;
                    //StopCoroutine(_step);
                }
                yield return new WaitForSeconds(time);
            }
        }

        void EndGeneration()
        {
            _recorder.CaptureOneShot();
            if (_generationTime.ContainsKey(Seed))
            {
                _generationTime[Seed] = _runtime - _generationStartTime;
            }
            else
            {
                _generationTime.Add(Seed, _runtime - _generationStartTime);
            }
        }

        void Update()
        {
            _runtime = Time.realtimeSinceStartup;
            if (_imported && _rhino && !_hasMesh && _gridController != null) _gridController.Update();

            SetFloorToBottom();
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
            if (_plot)
            {
                List<MeshCollider> colliders = new List<MeshCollider>();
                colliders.Add(ManagerPlot.GoPlot.GetComponent<MeshCollider>());
                HashSet<int> tilesToDisable = new HashSet<int>();
                for (int i = 0; i < _waveFunctionCollapse.Tiles.Count; i++)
                {
                    Vector3Int index = Util.ToUnityVector3Int(_waveFunctionCollapse.GetIndexOfPossibleSample(i));
                    Vector3 position = Vector3.Scale(index, Vector3.Scale(Vector3.one * _voxelSize, _tileSize)) + ((Vector3)_tileSize - Vector3.one) * _voxelSize / 2;

                    if (!Util.IsInside(colliders, position))
                    {
                        tilesToDisable.Add(i);
                    }
                }
                _waveFunctionCollapse.DisableTiles(tilesToDisable);
            }
        }

        void NextPlot()
        {
            _currentPlot++;
            if (_currentPlot == ManagerPlot.NrOfPlots) _currentPlot = 0;
            ManagerPlot.NextPlot(_currentPlot);
            _WFCSize = ManagerPlot.GetWFCSize(_currentPlot, _voxelSize, _tileSize);
            CenterWFC = ManagerPlot.CentrePlot(_currentPlot);
        }

        public void ClearGameobjects()
        {
            foreach (var go in _goColorCubes)
            {
                GameObject.Destroy(go);
            }
            _goColorCubes.Clear();
        }


        public void DrawGrid()
        {
            ClearGameobjects();

            foreach (var tile in _waveFunctionCollapse.Tiles)
            {
                DrawSample(tile);
            }
        }

        public void DrawSample(Tile tile)
        {
            var sample = tile.SelectedSample;
            _generatedTiles.Push(new ALIS_Tile(tile));
            if (sample.Id != 0)
            {
                ALIS_Sample selectedSample = sample as ALIS_Sample;
                Vector3Int index = Util.ToUnityVector3Int(tile.Index);
                GameObject goTile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                goTile.transform.localScale = Vector3.Scale(Vector3.one * _voxelSize, _tileSize);
                goTile.transform.localPosition = Vector3.Scale(index, goTile.transform.localScale) + ((Vector3)_tileSize - Vector3.one) * _voxelSize / 2;
                goTile.name = $"tile: {tile.Id}{index} {selectedSample.Name}";
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
                        _gridController.AddSampleToGrid(selectedSample, Util.ToUnityVector3Int(tile.Index), _tileSize, goTile.transform, _generatedTiles.Peek());
                    }
                    else
                    {
                        InstantiateGOMesh(selectedSample, Util.ToUnityVector3Int(tile.Index), goTile.transform);
                    }
                }
                _goColorCubes.Add(goTile);
            }
        }

        public void DeleteLastTile()
        {
            GameObject.Destroy(_goColorCubes.Last());
            _goColorCubes.RemoveAt(_goColorCubes.Count - 1);
            _gridController.RemoveTileFromGrid(_generatedTiles.Peek());
            _generatedTiles.Pop();
        }

        public void InstantiateGOMesh(ALIS_Sample selectedSample, Vector3Int tileIndex, Transform goTile)
        {
            foreach (var instance in selectedSample.Instances)
            {
                var position = (Vector3)(tileIndex * _tileSize) * _voxelSize; // tile position
                position += instance.Pose.position * _voxelSize;
                var rotation = instance.Pose.rotation;
                var go = _tiles[instance.DefinitionIndex].Instantiate(new Pose(position, rotation), _blockMaterial);
                go.transform.SetParent(goTile, true);
            }
        }

        void ResetWeight()
        {
            foreach (var sample in _sampleLibrary)
            {
                sample.Value.Weight = 1;
            }
        }

        void CheckSeed(int seed, float progressValue)
        {
            if (_bestSeedsProgress.Count == 0)
            {
                _bestSeedsProgress.Add(seed, progressValue);
                _bestSeeds.Add(seed);
            }
            bool bigger = false;

            for (int i = 0; i < _bestSeeds.Count; i++)
            {
                if (progressValue > _bestSeedsProgress[_bestSeeds[i]])
                {
                    bigger = true;
                }
            }

            if (bigger)
            {
                _bestSeedsProgress.Add(seed, progressValue);
                _bestSeeds.Add(seed);
            }
            var newDictionary = new Dictionary<int, float>();
            foreach (var goodSeed in _bestSeeds)
            {
                newDictionary.Add(goodSeed, _bestSeedsProgress[goodSeed]);
            }
            _bestSeedsProgress = newDictionary;
            _bestSeeds = _bestSeedsProgress.OrderByDescending(s => s.Value).ToList().GetRange(0, _bestSeedsProgress.Count < 10 ? _bestSeedsProgress.Count : 10).Select(s => s.Key).ToList();
        }

        void SetFloorToBottom()
        {
            if (_imported&& _waveFunctionCollapse != null)
            {
                var bottomPosition = Vector3.up * (_waveFunctionCollapse.Bottom * _tileSize.y * _voxelSize - _voxelSize / 2);
                _goFloor.transform.position = bottomPosition;
            }
        }
    }
}
