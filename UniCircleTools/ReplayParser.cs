using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using SharpCompress.Compressors.LZMA;

namespace UniCircleTools
{
    internal static class ReplayParser
    {
        /// <summary>
        ///     Parse replay file into passed replay object.
        /// </summary>
        /// <param name="replayPath">File path of replay to parse</param>
        /// <param name="replay">Beatmap object to parse into</param>
        internal static void Parse(string replayPath, Replay replay)
        {
            FileInfo fileInfo = new FileInfo(replayPath);
            if (!fileInfo.Exists || fileInfo.Length == 0)
            {
                throw new Exception("Replay file missing or empty");
            }

            BinaryReader reader = new BinaryReader(new FileStream(replayPath, FileMode.Open), Encoding.UTF8);

            replay.Mode = (Mode)reader.ReadByte();
            replay.Version = reader.ReadUInt32();
            replay.BeatmapHash = reader.ReadNullString();
            replay.PlayerName = reader.ReadNullString();
            replay.ReplayHash = reader.ReadNullString();

            replay.Count300 = reader.ReadUInt16();
            replay.Count100 = reader.ReadUInt16();
            replay.Count50 = reader.ReadUInt16();
            replay.CountGeki = reader.ReadUInt16();
            replay.CountKatu = reader.ReadUInt16();
            replay.CountMiss = reader.ReadUInt16();
            replay.Score = reader.ReadUInt32();
            replay.HighestCombo = reader.ReadUInt16();

            replay.PerfectCombo = reader.ReadBoolean();
            replay.Mods = (Mods)reader.ReadInt32();

            ParseLifeGraph(reader, replay);

            replay.Timestamp = DateTime.FromBinary(reader.ReadInt64());

            ParseFrames(reader, replay);

            reader.Close();
        }

        private static void ParseLifeGraph(BinaryReader reader, Replay replay)
        {
            // Format: time/life,time/life,...
            string rawPointsStr = reader.ReadNullString();
            string[] rawPoints = rawPointsStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string rawPoint in rawPoints)
            {
                string[] parts = rawPoint.Split('|');
                LifePoint lifePoint;
                lifePoint.time = Int32.Parse(parts[0]);
                lifePoint.life = Single.Parse(parts[1]);
                replay.LifePoints.Add(lifePoint);
            }
        }

        private static void ParseFrames(BinaryReader reader, Replay replay)
        {
            byte[] rawReplayData = reader.ReadByteArray();
            MemoryStream frameStream = new MemoryStream(rawReplayData);

            byte[] properties = new byte[5];
            frameStream.Read(properties, 0, 5);
            long outSize = new BinaryReader(frameStream).ReadInt64();
            long compressedSize = frameStream.Length - frameStream.Position;

            LzmaStream lzmaStream = new LzmaStream(properties, frameStream, compressedSize, outSize);
            StreamReader sr = new StreamReader(lzmaStream);

            string[] framesData = sr.ReadToEnd().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            float frameTime = 0;

            Keys lastKeys = Keys.None;

            foreach (string rawFrame in framesData)
            {
                string[] frameData = rawFrame.Split('|');
                float timeDiff = Single.Parse(frameData[0]);

                if (timeDiff < 0)
                {
                    continue;
                }
                frameTime += timeDiff;

                ReplayFrame frame;
                frame.time = frameTime;
                frame.x = Single.Parse(frameData[1]);
                frame.y = Single.Parse(frameData[2]);
                frame.keys = (Keys)Int16.Parse(frameData[3]);

                if (lastKeys != frame.keys)
                {
                    replay.Actions.Add(frame);
                }

                lastKeys = frame.keys;

                replay.Frames.Add(frame);
            }

        }
    }
}
