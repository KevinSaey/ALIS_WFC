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
            GenerateRandomSamples();

            _WFC = new WFC<ALIS_Sample>(10, 20, 20, _samples);
        }

        public void GenerateRandomSamples()
        {
            for (int i = 0; i < 20; i++)
            {
                _samples.Add(new ALIS_Sample(i));
            }

        }
    }
}
