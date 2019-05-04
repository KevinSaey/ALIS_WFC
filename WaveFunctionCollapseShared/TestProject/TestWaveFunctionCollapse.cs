using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WaveFunctionCollapse.Shared;

namespace TestProject
{
    [TestClass]
    class TestWaveFunctionCollapse
    {
        

        [TestMethod]
        public void WFCTest()
        {
            for (int i = 0; i < 20; i++)
            {
                _samples.Add(new TestSample(i));
            }

        }
        List<TestSample> _samples = new List<TestSample>();
        WaveFunctionCollapse<TestSample> _WFC = new WaveFunctionCollapse<TestSample>(10,20,20);
    }
}
