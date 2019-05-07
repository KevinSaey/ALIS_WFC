using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaveFunctionCollapse.Shared;

namespace WaveFunctionCollapse.Unity
{
    public class ManagerWFC : MonoBehaviour
    {
        [SerializeField]

        List<ALIS_Sample> _samples = new List<ALIS_Sample>();
        WFC<ALIS_Sample> _waveFunctionCollapse;
        List<GameObject> goColorCubes = new List<GameObject>();

        void Awake()
        {

            SharedLogger.CurrentLogger = new UnityLog();
            InitiateSamples();
            Debug.Log("Samples instantiated");
            _waveFunctionCollapse = new WFC<ALIS_Sample>(5, 5, 5, _samples);
            SetRandomSamples();
            Debug.Log("Random samples added");
        }

        void Start()
        {
            Debug.Log("Execute WFC");
            //_waveFunctionCollapse.Execute();

        }

        void Update()
        {
            _waveFunctionCollapse.Step(1);
            ClearGameobjects();
            DrawGrid();
            // Debug.Log(Time.time);
        }

        public void InitiateSamples()
        {
            for (int i = 0; i < 21; i++)
            {
                _samples.Add(new ALIS_Sample(i));
            }
        }

        public void SetRandomSamples()
        {
            for (int i = 1; i < _samples.Count; i++)
            {
                _samples[i].SetRandomNeighbours(6, _waveFunctionCollapse);
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
                var sample = _waveFunctionCollapse.Samples[_waveFunctionCollapse.SampleIndexGrid[i]];
                if (sample.Id != 0)
                {
                    Vector3Int index = Util.ToUnityVector3Int(_waveFunctionCollapse.GetIndexOfPossibleSample(i));
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    go.transform.position = index;
                    Debug.Log(sample.Col);
                    go.GetComponent<Renderer>().material.color = sample.Col;
                    goColorCubes.Add(go);
                    Debug.Log(index);
                }
            }
        }
    }
}
