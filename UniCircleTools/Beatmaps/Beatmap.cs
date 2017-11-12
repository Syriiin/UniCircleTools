using System;
using System.Collections.Generic;
using System.Text;

using UniCircleTools.Replays;
using UniCircleTools.Simulations;

namespace UniCircleTools.Beatmaps
{
    public struct BeatmapDifficulty
    {
        public float HP;
        public float CS;
        public float OD;
        public float AR;
        public double SliderMultiplier;
        public double SliderTickRate;
    }

    public class Beatmap
    {
        // General
        private int _formatVersion;
        private string _hash;
        private Mode _mode;
        private float _stackLeniency;
        // stack leniency allows notes hitobjects of close x,y
        //  to have slightly different playable position.

        public int FormatVersion { get => _formatVersion; internal set => _formatVersion = value; }
        public string Hash { get => _hash; internal set => _hash = value; }
        public Mode Mode { get => _mode; internal set => _mode = value; }
        public float StackLeniency { get => _stackLeniency; internal set => _stackLeniency = value; }

        // Metadata
        private string _title;
        private string _artist;
        private string _creator;
        private string _version;
        private string _beatmapId;
        private string _setId;

        public string Title { get => _title; internal set => _title = value; }
        public string Artist { get => _artist; internal set => _artist = value; }
        public string Creator { get => _creator; internal set => _creator = value; }
        public string Version { get => _version; internal set => _version = value; }
        public string BeatmapID { get => _beatmapId; internal set => _beatmapId = value; }
        public string SetID { get => _setId; internal set => _setId = value; }

        // Difficulty
        private BeatmapDifficulty _difficulty;

        public BeatmapDifficulty Difficulty { get => _difficulty; internal set => _difficulty = value; }

        // TimingPoints
        private List<TimingPoint> _timingPoints = new List<TimingPoint>();

        public List<TimingPoint> TimingPoints { get => _timingPoints; }

        // HitObjects
        private List<HitObject> _hitObjects = new List<HitObject>();

        public List<HitObject> HitObjects { get => _hitObjects; }

        /// <summary>
        ///     Construct empty Beatmap object or parse from given beatmap path.
        /// </summary>
        /// <param name="beatmapPath">Path to beatmap to parse</param>
        public Beatmap(string beatmapPath)
        {
            BeatmapParser.Parse(beatmapPath, this);
        }

        /// <summary>
        ///     Applies difficulty settings to all hitobjects
        /// </summary>
        /// <param name="difficulty">Difficulty settings to apply</param>
        public void ApplyDifficultySettings(BeatmapDifficulty difficulty)
        {
            _difficulty = difficulty;
            foreach (HitObject hitObject in HitObjects)
            {
                hitObject.ApplyDifficultySettings(difficulty);
            }
        }

        /// <summary>
        ///     Runs simulation on passed Replay
        /// </summary>
        /// <param name="replay">Replay to simulate</param>
        /// <returns>Simulation object containing result of simulation</returns>
        public Simulator RunSimulation(Replay replay)
        {
            Simulator simulator = new Simulator();
            simulator.SimulateReplay(this, replay);
            return simulator;
        }

        /// <summary>
        ///     Get timing point active at a given offset
        /// </summary>
        /// <param name="offset">Offset timing point controls</param>
        /// <returns>Timing point active at given offset</returns>
        public TimingPoint GetTimingPointForOffset(double offset)
        {
            TimingPoint timingPoint;
            for (int i = _timingPoints.Count - 1; i >= 0; i--)
            {
                timingPoint = _timingPoints[i];
                if (timingPoint.Offset < offset)
                {
                    return timingPoint;
                }
            }
            return null;
        }
    }
}
