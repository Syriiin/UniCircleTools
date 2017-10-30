using System;
using System.Linq;
using NUnit.Framework;

using UniCircleTools;
using UniCircleToolsTests.Resources;

namespace UniCircleToolsTests
{
    [TestFixture]
    public class ReplayParserTests
    {
        [Test]
        public void ParseReplay()
        {
            Replay replay = new Replay(Resource.PathToResource("Syrin - Hashimoto Miyuki - Kisetsu o Dakishimete ~blooming white love~ [Insane] (2016-06-23) Osu.osr"));

            Assert.AreEqual(Mode.Standard, replay.Mode);
            Assert.AreEqual(20171030, replay.Version);
            Assert.AreEqual("1bc1dfe3be6581db3011891f415fe4b9", replay.BeatmapHash);
            Assert.AreEqual("Syrin", replay.PlayerName);
            Assert.AreEqual("6bf92aab770a654b6985f0dfe577dabc", replay.ReplayHash);
            Assert.AreEqual(636022768343626525, replay.Timestamp.Ticks);
            Assert.AreEqual(Mods.Hidden | Mods.DoubleTime, replay.Mods);

            Assert.AreEqual(487, replay.Count300);
            Assert.AreEqual(19, replay.Count100);
            Assert.AreEqual(0, replay.Count50);
            Assert.AreEqual(108, replay.CountGeki);
            Assert.AreEqual(15, replay.CountKatu);
            Assert.AreEqual(0, replay.CountMiss);
            Assert.AreEqual(10515081, replay.Score);
            Assert.AreEqual(713, replay.HighestCombo);
            Assert.AreEqual(true, replay.PerfectCombo);

            // Test Life Points
            TestLifePoint(replay.LifePoints.First(), 626, 1);
            TestLifePoint(replay.LifePoints.Last(), 155566, 1);
            Assert.AreEqual(70, replay.LifePoints.Count);

            // Test Replay Frames
            TestReplayFrame(replay.Frames.First(), 0, 256, -500, Keys.None);
            TestReplayFrame(replay.Frames.Last(), 162993, 283.2201f, 231.5342f, Keys.None);
            Assert.AreEqual(7993, replay.Frames.Count);
        }

        private void TestLifePoint(LifePoint lifePoint, int time, float life)
        {
            Assert.AreEqual(time, lifePoint.time);
            Assert.AreEqual(life, lifePoint.life);
        }

        private void TestReplayFrame(ReplayFrame replayFrame, float time, float x, float y, Keys keys)
        {
            Assert.AreEqual(time, replayFrame.time);
            Assert.AreEqual(x, replayFrame.x);
            Assert.AreEqual(y, replayFrame.y);
            Assert.AreEqual(keys, replayFrame.keys);
        }
    }
}
