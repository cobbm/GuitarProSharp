using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarProSharp
{
    //    The top-level node of the song model.
    //    It contains basic information about the stored song.
    public class Song
    {
        public enum TripletFeelType
        {
            NONE,
            EIGHTH,
            SIXTEENTH
        }
        public struct SongMetadata
        {
            public string title;
            public string subtitle;
            public string artist;
            public string album;
            public string words;
            public string copyright;
            public string tabbedBy;
            public string instructions;
            public string[] notice;

            public override string ToString()
            {
                return $"title={title},subtitle={subtitle},artist={artist},album={album},words={words},copyright={copyright},tabbedBy={tabbedBy},instructions={instructions},notice={notice}";
            }
        }
        
        public class MeasureHeader
        {
            public enum Flags
            {
                NUMERATOR = 0x01,
                DENOMINATOR = 0x02,
                REPEAT = 0x04,
                END_REPEAT = 0x08,
                ALTERNATE_ENDING = 0x10,
                MARKER = 0x20,
                TONAILITY = 0x40,
                DOUBLE_BAR = 0x80
            };
            

            public readonly byte flags;

            public uint Number { get; private set; }
            //TODO: Python spec says Start is int, but may be Fraction? or float?
            //public uint Start { get; set; } = Duration.QUATER_TIME;
            public float Start { get; set; } = Duration.QUATER_TIME;

            public readonly bool hasDoubleBar;

            public TimeSignature TimeSignature { get; set; } = new TimeSignature();
            public KeySignature KeySignature { get; set; } = KeySignature.CMajor;

            public Marker? Marker { get; set; } = null;

            public readonly bool isRepeatOpen;
            public uint RepeatAlternative { get; set; } = 0;//TODO
            public sbyte RepeatClose { get; set; } = -1;
            public readonly bool isRepeatClose;
            public readonly bool isRepeatAlternative;
            public readonly bool isMarker;
            public readonly bool hasTonality;

            public readonly TripletFeelType TripletFeel = TripletFeelType.NONE; // Needed ???

            public MeasureHeader(uint number, byte flags)
            {
                this.flags = flags;

                this.Number = number;

                //parse the flags
                isRepeatOpen = (flags & (int)Flags.REPEAT) != 0;
                isRepeatClose = (flags & (int)Flags.END_REPEAT) != 0;
                isRepeatAlternative = (flags & (int)Flags.ALTERNATE_ENDING) != 0;
                isMarker = (flags & (int)Flags.MARKER) != 0;
                hasTonality = (flags & (int)Flags.TONAILITY) != 0;
                hasDoubleBar = (flags & (int)Flags.DOUBLE_BAR) != 0;
            }

            public float GetLength()
            {
                return TimeSignature.Numerator * TimeSignature.Denominator.GetTime();
            }

            public float GetEnd()
            {
                return Start + GetLength();
            }
        }

        public struct Marker
        {
            public string Title { get; private set; }
            public Color Color { get; private set; }

            public Marker(string title, Color color)
            {
                this.Title = title;
                this.Color = color;
            }

            public override string ToString()
            {
                return $"title={Title},color={Color.ToString()}";
            }
        }

        public string? Version { get; set; }
        public SongMetadata Metadata { get; set; }
        public TripletFeelType TripletFeel { get; set; } = TripletFeelType.NONE;
        public uint Tempo { get; set; } = 120;
        public bool HideTempo { get; set; } = false;
        public KeySignature Key { get; set; } = KeySignature.CMajor;

        //TODO: Store this here or just use temporaily?
        //public MidiChannel[] MidiChannels { get; set; }
        //public uint MeasureCount { get; set; }
        //public uint TrackCount { get; set; }
        public MeasureHeader[]? MeasureHeaders { get; set; }
        public Track[]? Tracks { get; set; }

        public override string ToString()
        {
            return $"version={Version},metadata={Metadata},tripletFeel={TripletFeel},tempo={Tempo},key={{{Key}}}";
        }
    }
}
