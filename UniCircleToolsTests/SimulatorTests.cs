using System;
using NUnit.Framework;

using UniCircleTools.Beatmaps;
using UniCircleTools.Replays;
using UniCircleTools.Simulations;
using UniCircleToolsTests.Resources;

namespace UniCircleToolsTests
{
    [TestFixture]
    public class SimulatorTests
    {
        [Test]
        public void SimulateReplay()
        {
            Beatmap beatmap = new Beatmap(Resource.PathToResource("auto.osu"));
            Replay replay = new Replay(Resource.PathToResource("auto.osr"));
            Simulator simulator = new Simulator();

            Assert.AreEqual(0, simulator.Count300);
            Assert.AreEqual(0, simulator.Count100);
            Assert.AreEqual(0, simulator.Count50);
            Assert.AreEqual(0, simulator.CountMiss);

            simulator.SimulateReplay(beatmap, replay);

            Assert.AreEqual(186, simulator.Count300);
            Assert.AreEqual(0, simulator.Count100);
            Assert.AreEqual(0, simulator.Count50);
            Assert.AreEqual(0, simulator.CountMiss);
        }
    }
}
