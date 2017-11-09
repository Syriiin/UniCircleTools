using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UniCircleTools.Beatmaps;
using UniCircleTools.Replays;
using UniCircleTools.Simulations;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //string beatmapPath = "G:\\Swin\\COS20007_OOP\\CUSTOM_PROJECT\\tests\\TRF - Survival dAnce ~no no cry more~ (Echo49) [Insane].osu";
            //string replayPath = "G:\\Swin\\COS20007_OOP\\CUSTOM_PROJECT\\tests\\Syrin - TRF - Survival dAnce ~no no cry more~ [Insane] (2017-01-31) Osu.osr";
            string beatmapPath = "G:\\Swin\\COS20007_OOP\\CUSTOM_PROJECT\\tests\\auto.osu";
            string replayPath = "G:\\Swin\\COS20007_OOP\\CUSTOM_PROJECT\\tests\\auto.osr";

            Beatmap beatmap = new Beatmap(beatmapPath);
            Replay replay = new Replay(replayPath);

            Simulator simulator = new Simulator();
            simulator.SimulateReplay(beatmap, replay);

            Console.ReadKey();
        }
    }
}
