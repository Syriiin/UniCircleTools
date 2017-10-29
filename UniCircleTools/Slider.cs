using System;
using System.Collections.Generic;
using System.Text;

namespace UniCircleTools
{
    public enum SliderType
    {
        Linear = 0,
        Perfect,
        Bezier,
        Catmull
    }

    public enum CurvePointType
    {
        Grey = 0,
        Red
    }

    public struct CurvePoint
    {
        public int x, y;
        public CurvePointType type;
    }

    public class Slider : HitObject
    {
        private SliderType _sliderType;
        private List<CurvePoint> _curvePoints = new List<CurvePoint>();
        private int _repeat;
        private double _pixelLength;

        public SliderType SliderType { get => _sliderType; internal set => _sliderType = value; }
        public List<CurvePoint> CurvePoints { get => _curvePoints; }
        public int Repeat { get => _repeat; internal set => _repeat = value; }
        public double PixelLength { get => _pixelLength; internal set => _pixelLength = value; }

        /// <summary>
        ///     Construct empty Slider object
        /// </summary>
        public Slider() { }
    }
}
