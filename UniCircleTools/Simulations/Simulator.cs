using System;
using System.Collections.Generic;
using System.Text;

using UniCircleTools.Beatmaps;
using UniCircleTools.Replays;

namespace UniCircleTools.Simulations
{
    public class Simulator
    {
        private string _beatmapHash = "";
        private float _simulationTime = 0;
        private List<HitObject> _objectPool = new List<HitObject>();
        private List<HitObject> _activeObjects = new List<HitObject>();
        private List<HitObject> _completedObjects = new List<HitObject>();

        public int Count300 { get; set; }
        public int Count100 { get; set; }
        public int Count50 { get; set; }
        public int CountMiss { get; set; }

        public Simulator()
        {
            // Initialise simulation to blank state
            Reset();
        }

        public void Reset()
        {
            // Reset simulation to blank state
            _beatmapHash = "";
            _simulationTime = 0;
            _objectPool.Clear();
            _activeObjects.Clear();
            _completedObjects.Clear();
            Count300 = 0;
            Count100 = 0;
            Count50 = 0;
            CountMiss = 0;
        }

        private void LoadBeatmap(Beatmap beatmap)
        {
            _beatmapHash = beatmap.Hash;
            
            // Iterate hitobjects and add converted versions to pool
            foreach (HitObject hitObject in beatmap.HitObjects)
            {
                // Only processing hitcircles for now. Sliders can be converted to just account for sliderhead, and spinner can be added later
                if (hitObject is HitCircle)
                {
                    hitObject.Time += 1515;     // TODO: Figure out what defines the difference in replay and beatmap times
                    _objectPool.Add(hitObject);
                }
                else if (hitObject is Slider)
                {
                    _objectPool.Add(new HitCircle {
                        X = hitObject.X,
                        Y = hitObject.Y,
                        Difficulty = hitObject.Difficulty,
                        NewCombo = hitObject.NewCombo,
                        Time = hitObject.Time + 1515
                    });
                }
            }
        }

        private void UpdateObjectPools()
        {
            List<HitObject> toRemove = new List<HitObject>();
            foreach (HitObject hitObject in _objectPool)
            {
                // If simulation has reached the objects approach window, add to active objects
                if (_simulationTime > hitObject.Time - hitObject.ApproachTime)
                {
                    toRemove.Add(hitObject);
                    _activeObjects.Add(hitObject);
                }
                else
                {
                    break;
                }
            }
            foreach (HitObject hitObject in toRemove)
            {
                _objectPool.Remove(hitObject);
            }
        }

        private HitResult UpdateHitObject(HitObject hitObject, float x, float y, FrameAction action, float time, bool clickUsed)
        {
            if (time > hitObject.Time + hitObject.HitWindowFor(HitResult.Hit50))
            {
                return HitResult.Miss;
            }

            if (action == FrameAction.Click && !clickUsed && hitObject.PointInCircle(x, y))
            {
                return hitObject.GetResultForOffset(time);
            }
            return HitResult.None;
        }

        private void ProcessFrame(ReplayFrame frame)
        {
            // Advance simulation time
            _simulationTime = frame.time;

            // Update object pools
            UpdateObjectPools();

            // Update active objects with action
            bool clickUsed = false;
            HitResult hitResult;
            List<HitObject> toRemove = new List<HitObject>();
            foreach (HitObject hitObject in _activeObjects)
            {
                hitResult = UpdateHitObject(hitObject, frame.x, frame.y, frame.action, _simulationTime, clickUsed);
                if (hitResult != HitResult.None)
                {
                    switch (hitResult)
                    {
                        case HitResult.Hit300:
                            Count300++;
                            break;
                        case HitResult.Hit100:
                            Count100++;
                            break;
                        case HitResult.Hit50:
                            Count50++;
                            break;
                        case HitResult.Miss:
                            CountMiss++;
                            break;
                        default:
                            break;
                    }

                    clickUsed = true;
                    toRemove.Add(hitObject);
                    _completedObjects.Add(hitObject);
                }
            }
            foreach (HitObject hitObject in toRemove)
            {
                _activeObjects.Remove(hitObject);
            }
        }

        public void SimulateReplay(Beatmap beatmap, Replay replay)
        {
            LoadBeatmap(beatmap);

            // Check beatmap hash
            if (replay.BeatmapHash != _beatmapHash)
            {
                throw new Exception("Replay hash does not match beatmap");
            }

            // Iterate through frames, processing each
            foreach (ReplayFrame frame in replay.Frames)
            {
                ProcessFrame(frame);
            }
            return;
        }
    }
}
