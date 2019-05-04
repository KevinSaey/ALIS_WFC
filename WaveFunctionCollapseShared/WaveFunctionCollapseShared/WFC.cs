﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse.Shared
{
    public class WFC<T> where T : Sample
    {
        List<T> _samples;
        public List<T> Samples
        {
            get
            {
                return _samples;
            }
            set
            {
                _samples = value;
            }
        }

        List<IHeuristic<T>> _heuristics;

        public List<IHeuristic<T>> Heuristics
        {
            get
            {
                return _heuristics;
            }
            set
            {
                _heuristics = value;
            }
        }

        SampleGrid<T> _sampleGrid;
        Engine<T> _engine;

        public WFC()
        {
            
        }

        public WFC(int xDimension, int yDimension, int zDimension, List<T> samples)
        {
            SharedLogger.Log("Instantiating WFC");
            _samples = samples;
            _sampleGrid = new SampleGrid<T>(this, xDimension, yDimension, zDimension);
            _engine = new Engine<T>(_samples, _heuristics, _sampleGrid);
            SharedLogger.Log( "WFC Instantiated");
            _engine.Execute();
        }
    }
}
