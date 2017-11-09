using System;
using System.Collections.Generic;
using System.Text;

namespace UniCircleTools
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
        ///     Simulate play given a Replay
        /// </summary>
        /// <param name="replay">Replay to simulate</param>
        /// <returns>Score object holding simulation results</returns>
        public Score SimulatePlay(Replay replay)
        {
            Score score = new Score();

            int currentHitObjectIdx = 0;

            HitObject currentHitObject;
            List<Slider> activeSliders = new List<Slider>();    // Possible to have multiple overlapping sliders/spinners in unrankable maps
            List<Spinner> activeSpinners = new List<Spinner>();

            // loop frames
            foreach (ReplayFrame frame in replay.Frames)
            {
                currentHitObject = HitObjects[currentHitObjectIdx];
                if (frame.time > currentHitObject.Time + currentHitObject.HitWindowFor(HitResult.Miss))
                {
                    score.CountMiss++;
                    currentHitObjectIdx++;
                    if (currentHitObjectIdx == HitObjects.Count)
                    {
                        break;
                    }
                    currentHitObject = HitObjects[currentHitObjectIdx];
                }

                // if key down, process hit
                // then process sliders and spinners
                // process interactions with relevent hitobjects
                if (frame.keyDown)
                {
                    Console.WriteLine("KeyDown {0}", frame.keys);
                    // Check for hit
                    if (frame.time > currentHitObject.Time - currentHitObject.ApproachTime && currentHitObject.PointInCircle(frame.x, frame.y))
                    {
                        HitResult hitRes = currentHitObject.GetResultForOffset(frame.time);
                        switch (hitRes)
                        {
                            case HitResult.Hit300:
                                score.Count300++;
                                currentHitObjectIdx++;
                                break;
                            case HitResult.Hit100:
                                score.Count100++;
                                currentHitObjectIdx++;
                                break;
                            case HitResult.Hit50:
                                score.Count50++;
                                currentHitObjectIdx++;
                                break;
                            case HitResult.Miss:
                                score.CountMiss++;
                                currentHitObjectIdx++;
                                break;
                            case HitResult.None:
                                break;
                            default:
                                break;
                        }
                        Console.WriteLine("Ping {0}", hitRes);
                        continue;   // We had an interaction with the current active object, thus none others can be hit
                    }

                    // check for notelock
                    for (int i = currentHitObjectIdx + 1; i < HitObjects.Count; i++)
                    {
                        if (frame.time > HitObjects[i].Time - HitObjects[i].ApproachTime)
                        {
                            // This object hasnt appeared yet
                            break;
                        }
                        if (HitObjects[i].PointInCircle(frame.x, frame.y))
                        {
                            // Notelocked.
                            break;
                        }
                    }
                }

                foreach (Slider slider in activeSliders)
                {
                    // Process slider (body aim, holding for ticks, etc)
                }

                foreach (Spinner spinner in activeSpinners)
                {
                    // Process spinner movement
                }
            }

            return score;
        }
    }
}
