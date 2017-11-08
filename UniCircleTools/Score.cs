using System;
using System.Collections.Generic;
using System.Text;

namespace UniCircleTools
{
    public class Score
    {
        private short _count300 = 0;
        private short _count100 = 0;
        private short _count50 = 0;
        private short _countMiss = 0;

        private short _k1Count = 0;
        private short _k2Count = 0;
        private short _m1Count = 0;
        private short _m2Count = 0;
        private short _noteLockCount = 0;

        public short Count300 { get => _count300; internal set => _count300 = value; }
        public short Count100 { get => _count100; internal set => _count100 = value; }
        public short Count50 { get => _count50; internal set => _count50 = value; }
        public short CountMiss { get => _countMiss; internal set => _countMiss = value; }

        public short K1Count { get => _k1Count; internal set => _k1Count = value; }
        public short K2Count { get => _k2Count; internal set => _k2Count = value; }
        public short M1Count { get => _m1Count; internal set => _m2Count = value; }
        public short M2Count { get => _m2Count; internal set => _m2Count = value; }
        public short NoteLockCount { get => _noteLockCount; internal set => _noteLockCount = value; }

        /// <summary>
        ///     Score objects hold the data generated from simulating a Replay on a Beatmap
        /// </summary>
        public Score() { }
    }
}
