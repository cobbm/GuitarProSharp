using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarProSharp
{
    // Flags are encoded as:
    //- *0x01*: drums track
    //- *0x02*: 12 stringed guitar track
    //- *0x04*: banjo track
    //- *0x08*: * blank*
    //- *0x10*: * blank*
    //- *0x20*: * blank*
    //- *0x40*: * blank*
    //- *0x80*: * blank*

    // A track contains multiple measures.
    public class Track
    {
        public struct GuitarString
        {
            public uint Number { get; private set; }
            public uint Tuning { get; private set; }

            public GuitarString(uint number, uint tuning)
            {
                this.Number = number;
                this.Tuning = tuning;
            }

            private static readonly string[] NOTES =
            {
                "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"
            };
            public override string ToString()
            {
                uint octave = Tuning / 12;
                uint note = Tuning % 12;
                return $"{NOTES[note]}{octave - 1}";
            }
        }

        public byte Flags { get; private set; }
        public uint Number { get; private set; } = 1;
        public uint FretCount { get; set; } = 24;
        public uint Offset { get; set; } = 0;
        public bool isPercussionTrack;// = (flags & 0x01)
        public bool is12StringedGuitarTrack;// = (flags & 0x02)
        public bool isBanjoTrack;// = (flags & 0x04)
        public bool isVisible = true;
        public bool isSolo = false;
        public bool isMute = false;
        public bool indicateTuning = false;
        public string Name { get; private set; }
        public List<Measure> Measures { get; } = new List<Measure>();
        public GuitarString?[] Strings { get; private set; }

        public uint MidiPort { get; set; } = 1;
        public MidiChannel? MidiChannel { get; set; } = null;
        public Color Color { get; set; } = Color.Red;

        public Track(string name, byte flags, GuitarString?[] strings)
        {
            this.Name = name;
            this.Flags = flags;

            isPercussionTrack = (flags & 0x01) != 0;
            is12StringedGuitarTrack = (flags & 0x02) != 0;
            isBanjoTrack = (flags & 0x04) != 0;
            Strings = strings;
            //    self.strings = [GuitarString(n, v)
            //      for n, v in [(1, 64), (2, 59), (3, 55),
            //          (4, 50), (5, 45), (6, 40)]]

            //  if self.measures is None:
            //      self.measures = [Measure(self, header) for header in self.song.measureHeaders]
        }

        public override string ToString()
        {
            return $"Track {Number}: {Name}";
        }
    }
}
