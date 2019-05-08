using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaveFunctionCollapse.Shared;

namespace WaveFunctionCollapse.Unity
{
    public class ManagerWFC : MonoBehaviour
    {
        [SerializeField]
        Vector3 _voxelSize;
        List<ALIS_Sample> _sampleLibrary = new List<ALIS_Sample>();
        WFC<ALIS_Sample> _waveFunctionCollapse;
        List<GameObject> goColorCubes = new List<GameObject>();
        IEnumerator _step;


        void Awake()
        {

            SharedLogger.CurrentLogger = new UnityLog();
            InitiateSamples();
            Debug.Log("Samples instantiated");
            _waveFunctionCollapse = new WFC<ALIS_Sample>(10, 11,13, _sampleLibrary);
            SetRandomSamples();
            Debug.Log("Random samples added");
        }

        void Start()
        {
            Debug.Log("Execute WFC");
            //_waveFunctionCollapse.Execute();

            _step = Step(.3f);
            StartCoroutine(_step);

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
            
            
        }

        public void InitiateSamples()
        {
            for (int i = 0; i < 21; i++)
            {
                _sampleLibrary.Add(new ALIS_Sample(i));
            }
        }

        public void SetRandomSamples()
        {
            for (int i = 1; i < _sampleLibrary.Count; i++)
            {
                _sampleLibrary[i].SetRandomNeighbours(3, _waveFunctionCollapse);
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
                    Vector3Int index = Util.ToUnityVector3Int(_waveFunctionCollapse.GetIndexOfPossibleSample(i));
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    go.transform.position = index;
                    go.GetComponent<Renderer>().material.color = sample.Col;
                    goColorCubes.Add(go);
                }
            }
        }
    }
}
