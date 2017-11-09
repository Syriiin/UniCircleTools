using System;
using System.Collections.Generic;
using System.Text;

namespace UniCircleTools.Replays
{
    [Flags]
    public enum Keys
    {
        None = 0,
        M1 = 1,
        M2 = 2,
        K1 = 4 | M1,    // Always set with M1
        K2 = 8 | M2     // Always set with M2
    }

    public enum FrameAction
    {
        None = 0,
        Click,
        Hold,
        Release
    }

    public struct LifePoint
    {
        public int time;
        public float life;
    }

    public struct ReplayFrame
    {
        public float time;
        public float x;
        public float y;
        public Keys keys;
        public FrameAction action;  // TODO: perhaps split frame action into individual key actions
    }

    public class Replay
    {
        // General
        private Mode _mode;
        private uint _version;
        private string _beatmapHash;
        private string _playerName;
        private string _replayHash;
        private DateTime _timestamp;
        private Mods _mods;

        public Mode Mode { get => _mode; internal set => _mode = value; }
        public uint Version { get => _version; internal set => _version = value; }
        public string BeatmapHash { get => _beatmapHash; internal set => _beatmapHash = value; }
        public string PlayerName { get => _playerName; internal set => _playerName = value; }
        public string ReplayHash { get => _replayHash; internal set => _replayHash = value; }
        public DateTime Timestamp { get => _timestamp; internal set => _timestamp = value; }
        public Mods Mods { get => _mods; internal set => _mods = value; }

        // Stats
        private ushort _count300;
        private ushort _count100;
        private ushort _count50;
        private ushort _countGeki;
        private ushort _countKatu;
        private ushort _countMiss;
        private uint _score;
        private ushort _highestCombo;
        private bool _perfectCombo;

        public ushort Count300 { get => _count300; internal set => _count300 = value; }
        public ushort Count100 { get => _count100; internal set => _count100 = value; }
        public ushort Count50 { get => _count50; internal set => _count50 = value; }
        public ushort CountGeki { get => _countGeki; internal set => _countGeki = value; }
        public ushort CountKatu { get => _countKatu; internal set => _countKatu = value; }
        public ushort CountMiss { get => _countMiss; internal set => _countMiss = value; }
        public uint Score { get => _score; internal set => _score = value; }
        public ushort HighestCombo { get => _highestCombo; internal set => _highestCombo = value; }
        public bool PerfectCombo { get => _perfectCombo; internal set => _perfectCombo = value; }

        // Life graph
        private List<LifePoint> _lifePoints = new List<LifePoint>();

        public List<LifePoint> LifePoints { get => _lifePoints; }

        // Frames
        private List<ReplayFrame> _replayFrames = new List<ReplayFrame>();

        public List<ReplayFrame> Frames { get => _replayFrames; }

        // Actions
        private List<ReplayFrame> _replayActions = new List<ReplayFrame>();

        public List<ReplayFrame> Actions { get => _replayActions; }

        /// <summary>
        ///     Construct empty Replay object or parse from given replay path.
        /// </summary>
        /// <param name="replayPath">Path to replay to parse</param>
        public Replay(string replayPath)
        {
            ReplayParser.Parse(replayPath, this);
        }
    }
}
