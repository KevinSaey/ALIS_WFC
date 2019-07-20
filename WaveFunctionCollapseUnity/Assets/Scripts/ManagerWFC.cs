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
        public string Path = @"C:\Users\Kevin\Desktop\WFC\RhinoExporter\";
        [SerializeField]
        Material _blockMaterial, _transparentMaterial;
        [SerializeField]
        GUISkin _guiSkin;
        [SerializeField]
        GameObject _goPlot;
        [SerializeField]
        GameObject _goRecorder;

        Dictionary<int, Sample> _sampleLibrary = new Dictionary<int, Sample>();
        WFC _waveFunctionCollapse;
        List<GameObject> goColorCubes = new List<GameObject>();
        IEnumerator _step;
        RhinoImporter _rhinoImporter;
        GridController _gridController;
        bool _colorCubes = true, _imported = false, _hasMesh, _plot = false, _record = false;
        Dictionary<int, ImportedTile> _tiles;
        bool _generateOne = false;
        Dictionary<int, float> _bestSeedsProgress = new Dictionary<int, float>();
        List<int> _bestSeeds = new List<int>();
        CaptureCamera _recorder;

        public static int Seed = 0;
        public Vector3 CenterWFC;
        public string Warnings;


        void Awake()
        {
            SharedLogger.CurrentLogger = new UnityLog(_log);
            _recorder = _goRecorder.GetComponent<CaptureCamera>();
            /*if (_rhino) RhinoAwake();
            else RandomAwake();*/
            var rend = _goPlot.GetComponent<MeshRenderer>();
            rend.material = new Material(_transparentMaterial);
            CenterWFC = Vector3.Scale(_WFCSize, _tileSize) * (_voxelSize / 2);
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
            rhinoAwakeSucces = _rhinoImporter.InstantiateSamples(_tileSize, _rotate, _reflectX, _reflectY, _reflectZ, _merge, this);
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

                _waveFunctionCollapse = new WFC(_WFCSize.x, _WFCSize.y, _WFCSize.z, _sampleLibrary);

                //Add the samples connections to the wfc grid
                //foreach (var sample in _sampleLibrary) sample.AddConnectionsToWFC(_waveFunctionCollapse);
                //_waveFunctionCollapse.RemoveEmptyConnections();

                if (!_hasMesh) _gridController = new GridController(_tileSize, _voxelSize, _WFCSize);
            }

            //SetPlotBoundaries();
            return rhinoAwakeSucces;
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

            //Warnings
            GUI.Label(new Rect(s, Screen.height-s, padding + 500, Screen.height -  s), Util.Warning);


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
                    if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Import Rhino"))
                    {
                        _imported = RhinoAwake();
                    }

                    _reflectX = GUI.Toggle(new Rect(s, s * i++, buttonWidth, buttonHeight), _reflectX, "ReflectX");
                    _reflectY = GUI.Toggle(new Rect(s, s * i++, buttonWidth, buttonHeight), _reflectY, "ReflectY");
                    _reflectZ = GUI.Toggle(new Rect(s, s * i++, buttonWidth, buttonHeight), _reflectZ, "ReflectZ");
                    _merge = GUI.Toggle(new Rect(s, s * i++, buttonWidth, buttonHeight), _merge, "Merge");
                    _rotate = GUI.Toggle(new Rect(s, s * i++, buttonWidth, buttonHeight), _rotate, "Rotate");
                    i++;
                    _plot = GUI.Toggle(new Rect(s, s * i++, buttonWidth, buttonHeight), _plot, "Plot");
                    var render = _goPlot.GetComponent<MeshRenderer>();
                    render.enabled = _plot;

                    

                    _WFCSize.x = int.TryParse(GUI.TextField(new Rect(s, s * ++i, buttonWidth / 2 - padding, buttonHeight), _WFCSize.x.ToString()), out int m) ? m : 1;
                    GUI.Label(new Rect(s + buttonWidth / 2, s * i++, buttonWidth / 2 - padding, buttonHeight), "WFC X");
                    _WFCSize.y = int.TryParse(GUI.TextField(new Rect(s, s * i, buttonWidth / 2 - padding, buttonHeight), _WFCSize.y.ToString()), out int n) ? n : 1;
                    GUI.Label(new Rect(s + buttonWidth / 2, s * i++, buttonWidth / 2 - padding, buttonHeight), "WFC Y");
                    _WFCSize.z = int.TryParse(GUI.TextField(new Rect(s, s * i, buttonWidth / 2 - padding, buttonHeight), _WFCSize.z.ToString()), out int o) ? o : 1;
                    GUI.Label(new Rect(s + buttonWidth / 2, s * i++, buttonWidth / 2 - padding, buttonHeight), "WFC Z");

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
                    GUI.HorizontalSlider(new Rect(Screen.width - buttonWidth - s, s * labelCounter++, buttonWidth, buttonHeight), _waveFunctionCollapse.Progress, 0, 1);

                    //Show labels

                    GUI.Label(new Rect(Screen.width - buttonWidth - s, s * labelCounter++, buttonWidth, buttonHeight), $"WFC dimension X: {_WFCSize.x} Y: {_WFCSize.y} Z: {_WFCSize.z}");

                    if (_reflectX) GUI.Label(new Rect(Screen.width - buttonWidth - s, s * labelCounter++, buttonWidth, buttonHeight), "Reflect X");
                    if (_reflectY) GUI.Label(new Rect(Screen.width - buttonWidth - s, s * labelCounter++, buttonWidth, buttonHeight), "Reflect Y");
                    if (_reflectZ) GUI.Label(new Rect(Screen.width - buttonWidth - s, s * labelCounter++, buttonWidth, buttonHeight), "Reflect Z");
                    if (_merge) GUI.Label(new Rect(Screen.width - buttonWidth - s, s * labelCounter++, buttonWidth, buttonHeight), "Merge");
                    if (_rotate) GUI.Label(new Rect(Screen.width - buttonWidth - s, s * labelCounter++, buttonWidth, buttonHeight), "Rotate");

                    labelCounter++;
                    GUI.Label(new Rect(Screen.width - buttonWidth - s, s * labelCounter++, buttonWidth, buttonHeight), "Best seeds");
                    for (int j = 0; j < _bestSeeds.Count; j++)
                    {
                        GUI.Label(new Rect(Screen.width - buttonWidth - s, s * labelCounter++, buttonWidth, buttonHeight),
                            $"Seed {_bestSeeds[j].ToString()} progres: {(int)(_bestSeedsProgress[_bestSeeds[j]] * 100)} %");
                    }


                    //leftScreen ui
                    if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Generate"))
                    {
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
                                    Regenerate();
                                }
                            }
                        }
                        if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Export to rhino"))
                        {
                            RhinoExporter.Export(_waveFunctionCollapse.Tiles.Where(t => t.Set && t.Enabled).ToList(), _WFCSize, _tileSize,_voxelSize,Path,_hasMesh);
                        }

                        if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Export to ALIS allocation"))
                        {
                            VoxelExporter.ExportVoxels(_gridController,Path);
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
            SetPlotBoundaries();
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
                        CheckSeed(Seed, _waveFunctionCollapse.Progress);
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
            if (_imported && _rhino && !_hasMesh && _gridController != null) _gridController.Update();
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
                colliders.Add(_goPlot.GetComponent<MeshCollider>());
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

            foreach (var tile in _waveFunctionCollapse.Tiles)
            {
                DrawSample(tile);
            }
        }

        /*public void DrawSamples(List<int> SampleIndices)
        {
            foreach (var sampleIndex in SampleIndices)
            {
                DrawSample(sampleIndex);
            }
        }*/

        public void DrawSample(Tile tile)
        {
            var sample = tile.SelectedSample;
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
                        _gridController.Generate(selectedSample, Util.ToUnityVector3Int(tile.Index), _tileSize, goTile.transform);
                    }
                    else
                    {
                        InstantiateGOMesh(selectedSample, Util.ToUnityVector3Int(tile.Index), goTile.transform);
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
    }
}
