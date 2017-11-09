using System;
using System.Collections.Generic;
using System.Text;

namespace UniCircleTools.Beatmaps
{
    public enum HitResult
    {
        Hit300 = 0,
        Hit100,
        Hit50,
        Miss,
        None
    }

    public abstract class HitObject
    {
        private int _x;
        private int _y;
        private int _time;
        private bool _newCombo;
        private BeatmapDifficulty _difficulty;

        public int X { get => _x; internal set => _x = value; }
        public int Y { get => _y; internal set => _y = value; }
        public int Time { get => _time; internal set => _time = value; }
        public bool NewCombo { get => _newCombo; internal set => _newCombo = value; }
        public BeatmapDifficulty Difficulty { get => _difficulty; internal set => _difficulty = value; }

        public double ApproachTime
        {
            get
            {
                if (Difficulty.AR <= 5)
                {
                    return 1800 - (Difficulty.AR * 120);
                }
                else
                {
                    return 1950 - (Difficulty.AR * 150);
                }
            }
        }

        public double Radius => 64 * (1 - 0.7 * (Difficulty.CS - 5) / 5) / 2;

        public HitObject() { }

        /// <summary>
        ///     Apply difficulty settings to a hitobject
        /// </summary>
        /// <param name="difficulty">Difficulty settings to apply</param>
        public void ApplyDifficultySettings(BeatmapDifficulty difficulty)
        {
            _difficulty = difficulty;
        }

        public bool PointInCircle(float x, float y)
        {
            double distance = Math.Sqrt(Math.Pow(X - x, 2) + Math.Pow(Y - y, 2));
            return distance < Radius;
        }

        /// <summary>
        ///     Returns hit window for given HitResult
        /// </summary>
        /// <param name="hitResult"></param>
        /// <returns></returns>
        public double HitWindowFor(HitResult hitResult)
        {
            switch (hitResult)
            {
                case HitResult.Hit300:
                    return 79.5 - (6 * Difficulty.OD);
                case HitResult.Hit100:
                    return 139.5 - (8 * Difficulty.OD);
                case HitResult.Hit50:
                    return 199.5 - (10 * Difficulty.OD);
                default:
                    return 400; // osu has a constant 400ms miss window
            }
        }

        public HitResult GetResultForOffset(float offset)
        {
            float hitError = Math.Abs(offset - Time);
            if (hitError < HitWindowFor(HitResult.Hit300))
            {
                return HitResult.Hit300;
            }
            else if (hitError < HitWindowFor(HitResult.Hit100))
            {
                return HitResult.Hit100;
            }
            else if (hitError < HitWindowFor(HitResult.Hit50))
            {
                return HitResult.Hit50;
            }
            else if (hitError < HitWindowFor(HitResult.Miss))
            {
                return HitResult.Miss;
            }

            return HitResult.None;
        }
    }
}
