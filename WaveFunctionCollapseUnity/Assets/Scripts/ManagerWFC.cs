using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaveFunctionCollapse.Shared;

namespace WaveFunctionCollapse.Unity
{
    public class ManagerWFC : MonoBehaviour
    {
        
        List<ALIS_Sample> _samples = new List<ALIS_Sample>();
        WFC<ALIS_Sample> _WFC;
        // Start is called before the first frame update
        void Start()
        {

            SharedLogger.CurrentLogger = new UnityLog();

            InitiateSamples();
            Debug.Log("Samples instantiated");
            SetRandomSamples();
            Debug.Log("Random samples added");
            _WFC = new WFC<ALIS_Sample>(10, 20, 20, _samples);
        }

        private void Update()
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
                _samples[i].SetRandomNeighbours(_samples.Count);
                _samples[i].Col = new Color(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255));
            }
        }
    }
}
