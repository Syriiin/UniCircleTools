using System;
using System.Collections.Generic;
using System.Text;

namespace UniCircleTools
{
    public class TimingPoint
    {
        private double _offset;     // documentation claims to be int, but double is possible (see charles445 smile of split)
        private double _msPerBeat;
        private int _beatsPerMeasure;
        private bool _inherited;
        private double _sliderVelocity;

        public double Offset { get => _offset; internal set => _offset = value; }
        public double MillisecondsPerBeat { get => _msPerBeat; internal set => _msPerBeat = value; }
        public int BeatsPerMeasure { get => _beatsPerMeasure; internal set => _beatsPerMeasure = value; }
        public bool Inherited { get => _inherited; internal set => _inherited = value; }
        public double SliderVelocity { get => _sliderVelocity; internal set => _sliderVelocity = value; }

        public double BeatsPerMinute { get => 60000 / _msPerBeat; }

        /// <summary>
        ///     Construct empty TimingPoint object
        /// </summary>
        public TimingPoint() { }
    }
}
