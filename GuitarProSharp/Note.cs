using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarProSharp
{
    public partial class Note
    {
        private static readonly int DEFAULT_VELOCITY = 1;
        public enum NoteType
        {
            REST = 0,
            NORMAL = 1,
            TIE = 2,
            DEAD = 3
        }
        //Left and right hand fingering used in tabs and chord diagram editor.
        public enum Fingering
        {
            OPEN = -1, // Open or muted.
            THUMB = 0, // Thumb.
            INDEX = 1, // Index finger.
            MIDDLE = 2, // Middle finger.
            ANNULAR = 3, // Annular finger.
            LITTLE = 4, // Little finger.
        }

        //public Beat Beat { get; private set; }
        public uint Value { get; set; } = 0;
        public int Velocity { get; set; } = DEFAULT_VELOCITY;
        public uint String { get; set; } = 0;

        public NoteEffect Effect { get; set; } = new NoteEffect();

        //TODO: Is this needed?
        //public float DurationPercent { get; set; } = 1.0f;
        public bool SwapAccidentals { get; set; } = false;
        public NoteType Type { get; set; } = NoteType.REST;

        //TODO: Wtf?
        public sbyte Duration { get; set; } = -1;
        public sbyte Tuplet { get; set; } = -1;


        public int GetRealValue()
        {
            throw new NotImplementedException();
        }
    }
}
