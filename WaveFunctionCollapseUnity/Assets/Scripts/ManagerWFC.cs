using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaveFunctionCollapse.Shared;

namespace WaveFunctionCollapse.Unity
{
    public class ManagerWFC : MonoBehaviour
    {
        
        List<ALIS_Sample> _samples = new List<ALIS_Sample>();
        WFC<ALIS_Sample> WaveFunctionCollapse;
        // Start is called before the first frame update
        void Awake()
        {

            SharedLogger.CurrentLogger = new UnityLog();
            InitiateSamples();
            Debug.Log("Samples instantiated");
            WaveFunctionCollapse = new WFC<ALIS_Sample>(3, 3, 3, _samples);
            SetRandomSamples();
            Debug.Log("Random samples added");
        }

         void Start()
        {
            Debug.Log("Execute WFC");
            WaveFunctionCollapse.Execute();
            
        }

         void Update()
        {
           // Debug.Log(Time.time);
        }

        public void InitiateSamples()
        {
            for (int i = 0; i < 20; i++)
            {
                _samples.Add(new ALIS_Sample(i));
            }
        }

        public void SetRandomSamples()
        {
            for (int i = 0; i < _samples.Count; i++)
            {
                _samples[i].SetRandomNeighbours(6,WaveFunctionCollapse);
                _samples[i].Col = new Color(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255));
            }
        }
    }
}
