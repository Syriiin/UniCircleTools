using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UniCircleTools
{
    internal static class BeatmapParser
    {
        // Parser for standard .osu beatmap files
        // 
        // osu file format history
        //
        // v3 (first version?):
        // [General]
        // AudioFilename:
        // AudioHash:
        // PreviewTime:
        // SampleSet:
        // 
        // [Metadata]
        // Title:
        // Artist:
        // Creator:
        // Version:
        // 
        // [Difficulty]
        // HPDrainRate:
        // CircleSize:
        // OverallDifficulty:
        // SliderMultiplier: 
        // SliderTickRate: 
        // 
        // [Events]
        // ...
        // 
        // [TimingPoints]
        // Offset, Milliseconds per Beat, Meter, Sample Type, Sample Set, Volume, Inherited, Kiai Mode
        // ...
        // 
        // [HitObjects]
        // ...
        //
        //
        // ----------------------- Changes ------------------------------------
        //
        //
        // - v5 added [Colours] section
        // - v6 added [Editor] section
        // 
        // - General
        //  - v4 added AudioLeadIn, EditorBookmarks
        //  - v5 removed AudioHash
        //  - v5 added Countdown, StackLeniency, Mode, LetterboxInBreaks
        //  - v6 removed EditorBookmarks
        //  - v7 added SkinPreference
        //  - v8 removed SkinPreference
        //  - v12 added WidescreenStoryboard
        // 
        // - Metadata
        //  - v5 added Source and Tags
        //  - v10 added TitleUnicode, ArtistUnicode, BeatmapID and BeatmapSetID (largly irrelevent)
        // 
        // - Difficulty
        //  - v8 seperated od into ar and od
        // 
        // ----------------------- Timing Points ------------------------------
        //  default: ______  ____    4       1           1           100     1           0
        //  v3:      offset  mspb
        //  v4:      offset  mspb    meter   sampletype  sampleset
        //  v5:      offset  mspb    meter   sampletype  sampleset   volume  [inherited]
        //  v6+:     offset  mspb    meter   sampletype  sampleset   volume  inherited   ki
        //  
        //  Non-inherited:
        //   eg. 2390,600,4,2,1,40,1,0
        //   bpm is stored as milliseconds per beat
        //  Inherited (marked by negative mbps and 0 in 7th field):
        //   eg. 59390,-50,4,2,1,50,0,0
        //   multiplier is stored as percentage of milliseconds per beat
        //   for example: a 100bpm slider lasts 600ms at 1x velocity
        //     but at 2x velocity, the 100bpm slider lasts 300ms (0.5x (-50) as long)
        //
        // ----------------------- Hit Objects --------------------------------
        //  - Circles
        //          x   y   time    type    hitSound    addition
        //   v3:    64  120 1665    1       0           ,
        //   v5:    345 110 8955    1       2
        //   v10:   220 100 18894   1       0           0:0:0
        //   v12:   35  127 53077   1       8           0:0:0:0:
        //   
        //
        //  - Sliders
        //          x   y   time    type    hitSound    sliderType  |curvePoints                                repeat  pixelLength         edgeHitsounds   edgeAdditions   addition
        //   docs:  424 96  66      2       0           B           |380:120|332:96|332:96|304:124              1       130                 2|0             0:0|0:0         0:0:0:0:
        //   v3:    256 192 512     6       2           B           |256:192|256:128|224:48|128:32|96:48|48:48  1       276
        //   v14:   267 376 2903    2       0           P           |269:343|262:309                            1       62.9999980773926    2|0             0:0|0:0         0:0:0:0:
        //   
        //   thankfully it appears slider syntax hasnt changed over the versions
        //   converting a v3 to v14 yields the same slider syntax even in the v14 format
        //   so that means that edgeHitsounds, edgeAdditions, and addition may be missing from any version
        //   
        //   Perfect sliders always have 2 and only 2 curve points (1st is x,y)
        //      128,96,6689,6,0,P|288:64|384:160,1,280
        //   Bezier sliders have 3 or more curve points (1 or more in old maps)
        //      256,192,512,6,2,B|256:192|256:128|224:48|128:32|96:48|48:48,1,276
        //   Linear sliders have 1 or more curve points
        //      192,96,2050,2,0,L|192:96|352:64|416:192,1,280
        //   Catmull sliders have 1 or more curve points
        //      96,96,3588,2,0,C|96:96|224:96|352:320,1,420
        //
        //   The first curve point in sliders from v3 and v4 are a duplicate of the starting point and should be ignored
        //
        //
        //  - Spinners
        //          x   y   time    type    hitSound    endTime     addition (0:0:0:0:)
        //  v3:     256 192 9742    12      0           12050
        //  v10:    256 192 46355   12      2           49450       0:0:0
        //  v12+:   256 192 57362   12      2           60219       1:2:0:0:
        //   


        /// <summary>
        ///     Parse beatmap file into passed beatmap object.
        /// </summary>
        /// <param name="beatmapPath">File path of beatmap to parse</param>
        /// <param name="beatmap">Beatmap object to parse into</param>
        internal static void Parse(string beatmapPath, Beatmap beatmap)
        {
            FileInfo fileInfo = new FileInfo(beatmapPath);
            if (!fileInfo.Exists || fileInfo.Length == 0)
            {
                throw new Exception("Beatmap file missing or empty");
            }

            StreamReader reader = new StreamReader(beatmapPath);
            ParseVersion(reader, beatmap);
            if (beatmap.FormatVersion < 3)
            {
                throw new Exception($"Format version v{beatmap.FormatVersion} is not supported!");
            }

            string line = reader.ReadLineTrim();
            while (!reader.EndOfStream)
            {
                switch (line)
                {
                    case "[General]":
                        ParseGeneral(reader, beatmap);
                        break;
                    case "[Metadata]":
                        ParseMetadata(reader, beatmap);
                        break;
                    case "[Difficulty]":
                        ParseDifficulty(reader, beatmap);
                        break;
                    case "[TimingPoints]":
                        ParseTimingPoints(reader, beatmap);
                        break;
                    case "[HitObjects]":
                        ParseHitObjects(reader, beatmap);
                        break;
                    default:
                        break;
                }

                line = reader.ReadLineTrim();
            }

            reader.Close();
        }

        private static void ParseVersion(StreamReader reader, Beatmap beatmap)
        {
            string line = reader.ReadLineTrim();
            if (!line.StartsWith("osu file format v"))
            {
                throw new Exception("Unknown .osu file format");
            }
            string version = line.Substring(17);    // index of version number
            beatmap.FormatVersion = Int32.Parse(version);
        }

        private static Dictionary<string, string> ParseKeyValues(StreamReader reader)
        {
            Dictionary<string, string> kv = new Dictionary<string, string>();
            string[] parts;

            string line = "";
            while (!reader.EndOfStream)
            {
                line = reader.ReadLineTrim();
                parts = line.Split(new char[] { ':' }, 2);

                if (parts.Length > 1)   // We have a valid kv pair
                {
                    // Store as kv[key] = value (with possible leading space removed)
                    kv[parts[0]] = parts[1].StartsWith(" ") ? parts[1].Substring(1) : parts[1];
                }

                if ((char)reader.Peek() == '[')   // Break KV read at next section tag
                {
                    break;
                }
            }

            return kv;
        }

        private static void ParseGeneral(StreamReader reader, Beatmap beatmap)
        {
            Dictionary<string, string> kv = ParseKeyValues(reader);

            beatmap.Mode = kv.ContainsKey("Mode")
                ? (Mode)Enum.Parse(typeof(Mode), kv["Mode"])
                : Mode.Standard;

            beatmap.StackLeniency = kv.ContainsKey("StackLeniency")
                ? Single.Parse(kv["StackLeniency"])
                : 0.7f;
        }

        private static void ParseMetadata(StreamReader reader, Beatmap beatmap)
        {
            Dictionary<string, string> kv = ParseKeyValues(reader);

            beatmap.Title = kv["Title"];
            beatmap.Artist = kv["Artist"];
            beatmap.Creator = kv["Creator"];
            beatmap.Version = kv["Version"];
            beatmap.BeatmapID = kv.ContainsKey("BeatmapID") ? kv["BeatmapID"] : null;
            beatmap.SetID = kv.ContainsKey("BeatmapSetID") ? kv["BeatmapSetID"] : null;
        }

        private static void ParseDifficulty(StreamReader reader, Beatmap beatmap)
        {
            Dictionary<string, string> kv = ParseKeyValues(reader);

            BeatmapDifficulty difficulty;
            
            difficulty.HP = Single.Parse(kv["HPDrainRate"]);
            difficulty.CS = Single.Parse(kv["CircleSize"]);
            difficulty.OD = Single.Parse(kv["OverallDifficulty"]);
            difficulty.AR = kv.ContainsKey("ApproachRate") ? Single.Parse(kv["ApproachRate"]) : difficulty.OD;
            difficulty.SliderMultiplier = Double.Parse(kv["SliderMultiplier"]);
            difficulty.SliderTickRate = Double.Parse(kv["SliderTickRate"]);

            beatmap.Difficulty = difficulty;
        }

        private static void ParseTimingPoints(StreamReader reader, Beatmap beatmap)
        {
            TimingPoint timingPoint;
            double lastMsPerBeat = 0;   // Store mspb for inherited timing points

            string line = "";
            while (!reader.EndOfStream)
            {
                line = reader.ReadLineTrim();
                timingPoint = ParseTimingPoint(line, lastMsPerBeat, beatmap.FormatVersion);

                if (timingPoint != null)
                {
                    beatmap.TimingPoints.Add(timingPoint);
                    if (!timingPoint.Inherited)
                    {
                        lastMsPerBeat = timingPoint.MillisecondsPerBeat;
                    }
                }

                if ((char)reader.Peek() == '[')   // Break KV read at next section tag
                {
                    break;
                }
            }
        }

        private static TimingPoint ParseTimingPoint(string line, double lastMsPerBeat, int formatVersion)
        {
            string[] parts = line.Split(',');

            if (parts.Length < 2) // Invalid timing point
            {
                return null;
            }

            TimingPoint timingPoint = new TimingPoint
            {

                // Offset is unchanged across versions
                Offset = Double.Parse(parts[0])
            };

            // Prior to v4, beats per measure was always 4
            if (formatVersion <= 3)
            {
                timingPoint.BeatsPerMeasure = 4;
            }
            else
            {
                timingPoint.BeatsPerMeasure = Int32.Parse(parts[2]);
            }

            // Prior to v6, inherited timing points didn't exist (although in v5 some maps had the field)
            if (formatVersion <= 5)
            {
                timingPoint.Inherited = false;
            }
            else
            {
                timingPoint.Inherited = parts[6] == "0";    // 1 = non inherited
            }

            // MillisecondsPerBeat on inherited points is a negative value that denotes sv
            if (timingPoint.Inherited)
            {
                timingPoint.MillisecondsPerBeat = lastMsPerBeat;    // inherit msbp
                timingPoint.SliderVelocity = -100 / Double.Parse(parts[1]);
            }
            else
            {
                timingPoint.MillisecondsPerBeat = Double.Parse(parts[1]);
                timingPoint.SliderVelocity = 1; // default sv is 1x
            }

            if (formatVersion <= 4)
            {
                timingPoint.Offset += 24;   // weird thing about v3 and v4 maps
            }

            return timingPoint;
        }

        private static void ParseHitObjects(StreamReader reader, Beatmap beatmap)
        {
            HitObject hitObject;

            string line = "";
            while (!reader.EndOfStream)
            {
                line = reader.ReadLineTrim();
                hitObject = ParseHitObject(line, beatmap.FormatVersion);

                if (hitObject != null)
                {
                    hitObject.ApplyDifficultySettings(beatmap.Difficulty);
                    beatmap.HitObjects.Add(hitObject);
                }

                if ((char)reader.Peek() == '[')   // Break KV read at next section tag
                {
                    break;
                }
            }
        }

        private static HitObject ParseHitObject(string line, int formatVersion)
        {
            // RemoveEmptyEntries because v3 and v4 hitcircles have a trailing comma that could be mistaked for additions
            string[] parts = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 5) // Invalid hit object
            {
                return null;
            }

            HitObject hitObject;

            int type = Int32.Parse(parts[3]);

            if ((type & 1) == 1)
            {
                hitObject = ParseHitCircle(parts);
            }
            else if ((type & 2) == 2)
            {
                hitObject = ParseSlider(parts, formatVersion);
            }
            else if ((type & 8) == 8)
            {
                hitObject = ParseSpinner(parts, formatVersion);
            }
            else
            {
                throw new Exception("Unknown object type: " + type);
            }

            if ((type & 4) == 4)
            {
                hitObject.NewCombo = true;
            }

            if (formatVersion <= 4)
            {
                hitObject.Time += 24;
            }

            return hitObject;
        }

        // Format: x,y,time,type,hitSound,addition
        private static HitCircle ParseHitCircle(string[] parts)
        {
            HitCircle circle = new HitCircle
            {
                X = Int32.Parse(parts[0]),
                Y = Int32.Parse(parts[1]),
                Time = Int32.Parse(parts[2])
            };

            return circle;
        }

        // Format: x,y,time,type,hitSound,sliderType|curvePoints,repeat,pixelLength,edgeHitsounds,edgeAdditions,addition
        private static Slider ParseSlider(string[] parts, int formatVersion)
        {
            Slider slider = new Slider
            {
                X = Int32.Parse(parts[0]),
                Y = Int32.Parse(parts[1]),
                Time = Int32.Parse(parts[2]),
                Repeat = Int32.Parse(parts[6]),
                PixelLength = Double.Parse(parts[7])
            };

            string[] sliderShapeParts = parts[5].Split('|');
            string sliderType = sliderShapeParts[0];

            // Slider type
            switch (sliderType)
            {
                case "L":
                    slider.SliderType = SliderType.Linear;
                    break;
                case "P":
                    slider.SliderType = SliderType.Perfect;
                    break;
                case "B":
                    slider.SliderType = SliderType.Bezier;
                    break;
                case "C":
                    slider.SliderType = SliderType.Catmull;
                    break;
                default:
                    throw new Exception("Unknown slider type: " + sliderType);
            }

            // Parse curve points
            int firstPointIndex = formatVersion <= 4 ? 2 : 1;   // First point is type, second point is dupe of x,y for v3 and v4
            for (int i = firstPointIndex; i < sliderShapeParts.Length; i++)
            {
                CurvePoint curvePoint;
                string[] curvePointParts = sliderShapeParts[i].Split(':');

                curvePoint.x = Int32.Parse(curvePointParts[0]);
                curvePoint.y = Int32.Parse(curvePointParts[1]);

                // Red points are denoted by duplicates
                if (i < sliderShapeParts.GetUpperBound(0) && sliderShapeParts[i] == sliderShapeParts[i + 1])
                {
                    // Red point
                    curvePoint.type = CurvePointType.Red;
                    i++;    // Skip duplicate point
                }
                else
                {
                    // Grey point
                    curvePoint.type = CurvePointType.Grey;
                }

                slider.CurvePoints.Add(curvePoint);
            }

            return slider;
        }

        // Format: x,y,time,type,hitSound,endTime,addition
        private static Spinner ParseSpinner(string[] parts, int formatVersion)
        {
            Spinner spinner = new Spinner
            {
                X = Int32.Parse(parts[0]),
                Y = Int32.Parse(parts[1]),
                Time = Int32.Parse(parts[2]),
                EndTime = Int32.Parse(parts[5])
            };

            if (formatVersion <= 4)
            {
                spinner.EndTime += 24;
            }

            return spinner;
        }
    }
}
