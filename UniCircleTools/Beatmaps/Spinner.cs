using System;
using System.Collections.Generic;
using System.Text;

namespace UniCircleTools.Beatmaps
{
    public class Spinner : HitObject
    {
        private int _endTime;

        public int EndTime { get => _endTime; set => _endTime = value; }

        /// <summary>
        ///     Construct empty Spinner object
        /// </summary>
        public Spinner() { }
    }
}
