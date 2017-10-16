using System;
using System.Collections.Generic;
using System.Text;

namespace UniCircleTools
{
    public abstract class HitObject
    {
        private int _x;
        private int _y;
        private int _time;
        private bool _newCombo;

        public int X { get => _x; internal set => _x = value; }
        public int Y { get => _y; internal set => _y = value; }
        public int Time { get => _time; internal set => _time = value; }
        public bool NewCombo { get => _newCombo; internal set => _newCombo = value; }

        public HitObject() { }
    }
}
