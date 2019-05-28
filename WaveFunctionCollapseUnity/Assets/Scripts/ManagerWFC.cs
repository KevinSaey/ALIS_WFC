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
        bool _rhino, _log,_interval, _rotate, _reflect;

        List<ALIS_Sample> _sampleLibrary = new List<ALIS_Sample>();
        WFC<ALIS_Sample> _waveFunctionCollapse;
        List<GameObject> goColorCubes = new List<GameObject>();
        IEnumerator _step;
        RhinoImporter _rhinoImporter;
        GridController _gridController;
        bool _colorCubes = true;

        public Vector3 CenterWFC;


        void Awake()
        {
            SharedLogger.CurrentLogger = new UnityLog(_log);
                        
            if (_rhino) RhinoAwake();
            else RandomAwake();

            CenterWFC = Vector3.Scale(_WFCSize , _tileSize)* (_voxelSize / 2);
        }

        void RandomAwake()
        {
            InitialiseRandomSamples();

            _waveFunctionCollapse = new WFC<ALIS_Sample>(_WFCSize.x, _WFCSize.y, _WFCSize.z, _sampleLibrary);

            SetRandomSamples();
        }

        void RhinoAwake()
        {
            _rhinoImporter = new RhinoImporter(_tileSize,_rotate,_reflect,this);
            _sampleLibrary = _rhinoImporter.Samples;
            Debug.Log($"{_sampleLibrary.Count} samples loaded");

            _waveFunctionCollapse = new WFC<ALIS_Sample>(_WFCSize.x, _WFCSize.y, _WFCSize.z, _sampleLibrary);

            //Add the samples connections to the wfc grid
            //foreach (var sample in _sampleLibrary) sample.AddConnectionsToWFC(_waveFunctionCollapse);
            //_waveFunctionCollapse.RemoveEmptyConnections();
            
            _gridController = new GridController(_tileSize, _voxelSize, _WFCSize);
        }

        void Start()
        {
            if (_interval)
            {
                _step = Step(2f);
                StartCoroutine(_step);
            }
            else
            {
                _waveFunctionCollapse.Execute();
                DrawGrid();
            }
        }

        void OnGUI()
        {
            int buttonHeight = 30;
            int buttonWidth = 150;
            int i = 1;
            int s = buttonHeight + 5;

            if (_rhino)
            {
                i =_gridController.OnGUI(buttonHeight, buttonWidth, i, s);

                if (GUI.Button(new Rect(s, s * i++, buttonWidth, buttonHeight), "Hide colorcubes"))
                {
                    _colorCubes = !_colorCubes;
                    HideColorCubes(_colorCubes);
                }
            }

        }

        void HideColorCubes(bool flag)
        {
            for (int i = 0; i < goColorCubes.Count; i++)
            {
                goColorCubes[i].GetComponent <MeshRenderer>().enabled = flag;
        }

        }

        IEnumerator Step(float time)
        {
           while (true)
            {
                Debug.Log("Step");
                //DrawSamples(_waveFunctionCollapse.Step(1));
                _waveFunctionCollapse.Step(1);

                if (_waveFunctionCollapse.IsAllDetermined||_waveFunctionCollapse.HasConflict)
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
            if (_rhino) _gridController.Update();
        }

        public void InitialiseRandomSamples()
        {
            for (int i = 0; i < 20; i++)
            {
                _sampleLibrary.Add(new ALIS_Sample(i));
            }
        }

        public void SetRandomSamples()
        {
            for (int i = 1; i < _sampleLibrary.Count; i++)
            {
                _sampleLibrary[i].SetRandomNeighbours(10, _waveFunctionCollapse);
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
                ALIS_Sample selectedSample = sample;
                Vector3Int index = Util.ToUnityVector3Int(_waveFunctionCollapse.GetIndexOfPossibleSample(sampleIndex));
                GameObject goTile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                goTile.transform.localScale = Vector3.Scale(Vector3.one * _voxelSize, _tileSize);
                goTile.transform.localPosition = Vector3.Scale(index, goTile.transform.localScale)+((Vector3)_tileSize-Vector3.one)*_voxelSize/2;
                goTile.name = $"tile: {sampleIndex}{index} {selectedSample.Name}";

                Material mat = goTile.GetComponent<Renderer>().material;
                mat.color = sample.Col;
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.DisableKeyword("_ALPHABLEND_ON");
                mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;

                if (_rhino)
                {
                    _gridController.Generate(selectedSample, Util.ToUnityVector3Int(_waveFunctionCollapse.GetIndexOfPossibleSample(sampleIndex)), _tileSize, goTile.transform);
                }

                goColorCubes.Add(goTile);
            }
        }
    }
}
