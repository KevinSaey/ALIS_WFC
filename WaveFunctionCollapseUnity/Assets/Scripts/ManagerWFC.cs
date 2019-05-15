using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaveFunctionCollapse.Shared;

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
        bool _rhino;

        List<ALIS_Sample> _sampleLibrary = new List<ALIS_Sample>();
        WFC<ALIS_Sample> _waveFunctionCollapse;
        List<GameObject> goColorCubes = new List<GameObject>();
        IEnumerator _step;
        RhinoImporter _rhinoImporter;
        GridController _gridController;


        void Awake()
        {
            SharedLogger.CurrentLogger = new UnityLog();

            if (_rhino) RhinoAwake();
            else RandomAwake();
        }

        void RandomAwake()
        {
            InitialiseRandomSamples();
            Debug.Log($"{_sampleLibrary.Count} samples added");

            _waveFunctionCollapse = new WFC<ALIS_Sample>(_WFCSize.x, _WFCSize.y, _WFCSize.z, _sampleLibrary);

            SetRandomSamples();
        }

        void RhinoAwake()
        {
            _rhinoImporter = new RhinoImporter();
            _sampleLibrary = _rhinoImporter.Samples;
            Debug.Log($"{_sampleLibrary.Count} samples loaded");

            _waveFunctionCollapse = new WFC<ALIS_Sample>(_WFCSize.x, _WFCSize.y, _WFCSize.z, _sampleLibrary);

            //Add the samples connections to the wfc grid
            foreach (var sample in _sampleLibrary) sample.AddConnectionsToWFC(_waveFunctionCollapse);

            _gridController = new GridController(_tileSize, _voxelSize,_WFCSize);
        }

        void Start()
        {
            Debug.Log("Execute WFC");
           _waveFunctionCollapse.Execute();
           DrawGrid();

            _step = Step(.3f);
            //StartCoroutine(_step);

        }

        void OnGUI()
        {
            if (_rhino) _gridController.OnGUI();
        }

        IEnumerator Step(float time)
        {
            while (true)
            {
                Debug.Log("Step");
                _waveFunctionCollapse.Step(1);
                ClearGameobjects();
                DrawGrid();
                if (_waveFunctionCollapse.IsAllDetermined)
                {
                    StopCoroutine(_step);
                }
                yield return new WaitForSeconds(time);
            }

        }


        void Update()
        {
            if (_rhino) _gridController.Update();
        }

        public void InitialiseSamles()
        {

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
            for (int i = 0; i < _waveFunctionCollapse.SampleIndexGrid.Count; i++)
            {
                var sample = _waveFunctionCollapse.SampleLibrary[_waveFunctionCollapse.SampleIndexGrid[i]];
                if (sample.Id != 0)
                {
                    if (_rhino)
                    {
                        _gridController.Generate(_sampleLibrary[sample.Id], Util.ToUnityVector3Int(_waveFunctionCollapse.GetIndexOfPossibleSample(i)), _tileSize);
                    }

                    Vector3Int index = Util.ToUnityVector3Int(_waveFunctionCollapse.GetIndexOfPossibleSample(i));
                    GameObject goTile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    goTile.transform.localScale = Vector3.Scale(Vector3.one * _voxelSize, _tileSize);
                    goTile.transform.position = Vector3.Scale(index, goTile.transform.localScale);
                    Material mat = goTile.GetComponent<Renderer>().material;
                    mat.color = sample.Col;
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.DisableKeyword("_ALPHABLEND_ON");
                    mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = 3000;

                    goColorCubes.Add(goTile);
                }
            }
        }
    }
}
