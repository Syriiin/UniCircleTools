﻿using System;
using System.Collections.Generic;
using System.Text;

namespace UniCircleTools
{
    public class Spinner : HitObject
    {
        private int _endTime;

        public int EndTime { get => _endTime; internal set => _endTime = value; }

        /// <summary>
        ///     Construct empty Spinner object
        /// </summary>
        public Spinner() { }
    }
}
