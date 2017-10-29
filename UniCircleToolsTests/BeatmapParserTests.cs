using System;
using System.Linq;
using NUnit.Framework;

using UniCircleTools;
using UniCircleToolsTests.Resources;

namespace UniCircleToolsTests
{
    [TestFixture]
    public class BeatmapParserTests
    {
        [Test]
        public void ParseBeatmap()
        {
            Beatmap beatmap = new Beatmap(Resource.PathToResource("Hashimoto Miyuki - Kisetsu o Dakishimete -blooming white love- (Kencho) [Insane].osu"));

            Assert.AreEqual(14, beatmap.FormatVersion);
            Assert.AreEqual(Mode.Standard, beatmap.Mode);
            Assert.AreEqual(0.7f, beatmap.StackLeniency);

            Assert.AreEqual("Kisetsu o Dakishimete ~blooming white love~", beatmap.Title);
            Assert.AreEqual("Hashimoto Miyuki", beatmap.Artist);
            Assert.AreEqual("Kencho", beatmap.Creator);
            Assert.AreEqual("Insane", beatmap.Version);
            Assert.AreEqual("654791", beatmap.BeatmapID);
            Assert.AreEqual("290683", beatmap.SetID);

            Assert.AreEqual(6, beatmap.HP);
            Assert.AreEqual(4, beatmap.CS);
            Assert.AreEqual(7, beatmap.OD);
            Assert.AreEqual(9, beatmap.AR);
            Assert.AreEqual(1.6, beatmap.SliderMultiplier);
            Assert.AreEqual(1.0, beatmap.SliderTickRate);

            // Test Timing points
            TestTimingPoint(beatmap.TimingPoints.First(), 444, 365.853658536585, 4, false, 1.0);
            TestTimingPoint(beatmap.TimingPoints.Last(), 155565, 365.853658536585, 4, true, 1.0);
            Assert.AreEqual(34, beatmap.TimingPoints.Count);

            // Test Hitobjects
            TestHitCircle(beatmap.HitObjects[1] as HitCircle, 112, 168, 809, false);
            TestSlider(beatmap.HitObjects.First() as Slider, 140, 316, 444, true, SliderType.Linear, 1, 60.0000022888184, 1, 128, 257, CurvePointType.Grey);
            TestSpinner(beatmap.HitObjects.Last() as Spinner, 256, 192, 149712, true, 155565);
            Assert.AreEqual(506, beatmap.HitObjects.Count);
        }

        private void TestTimingPoint(TimingPoint timingPoint, int offset, double millisecondsPerBeat, int beatsPerMeasure, bool inherited, double sliderVelocity)
        {
            Assert.AreEqual(offset, timingPoint.Offset);
            Assert.AreEqual(millisecondsPerBeat, timingPoint.MillisecondsPerBeat);
            Assert.AreEqual(beatsPerMeasure, timingPoint.BeatsPerMeasure);
            Assert.AreEqual(inherited, timingPoint.Inherited);
            Assert.AreEqual(sliderVelocity, timingPoint.SliderVelocity);
        }

        private void TestHitCircle(HitCircle hitCircle, int x, int y, int time, bool newCombo)
        {
            Assert.AreEqual(x, hitCircle.X);
            Assert.AreEqual(y, hitCircle.Y);
            Assert.AreEqual(time, hitCircle.Time);
            Assert.AreEqual(newCombo, hitCircle.NewCombo);
        }

        private void TestSlider(Slider slider, int x, int y, int time, bool newCombo, SliderType sliderType, int repeat, double pixelLength, int curvePointCount, int curvePoint0X, int curvePoint0Y, CurvePointType curvePoint0Type)
        {
            Assert.AreEqual(x, slider.X);
            Assert.AreEqual(y, slider.Y);
            Assert.AreEqual(time, slider.Time);
            Assert.AreEqual(newCombo, slider.NewCombo);
            Assert.AreEqual(sliderType, slider.SliderType);
            Assert.AreEqual(repeat, slider.Repeat);
            Assert.AreEqual(pixelLength, slider.PixelLength);
            
            // Should really test all curve points but will only test first point for now (since test slider only has one)
            Assert.AreEqual(curvePointCount, slider.CurvePoints.Count);
            Assert.AreEqual(curvePoint0X, slider.CurvePoints[0].x);
            Assert.AreEqual(curvePoint0Y, slider.CurvePoints[0].y);
            Assert.AreEqual(curvePoint0Type, slider.CurvePoints[0].type);
        }

        private void TestSpinner(Spinner spinner, int x, int y, int time, bool newCombo, int endTime)
        {
            Assert.AreEqual(x, spinner.X);
            Assert.AreEqual(y, spinner.Y);
            Assert.AreEqual(time, spinner.Time);
            Assert.AreEqual(newCombo, spinner.NewCombo);
            Assert.AreEqual(endTime, spinner.EndTime);
        }
    }
}
