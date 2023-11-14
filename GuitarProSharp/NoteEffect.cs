namespace GuitarProSharp
{
    public class NoteEffect
    {
        public enum HarmonicEffectType
        {
            NATURAL = 1,
            ARTIFICIAL = 2,
            TAPPED = 3,
            PINCH = 4,
            SEMI_HARMONIC = 5
        }

        //An enumeration of all supported slide types.
        public enum SlideType {
            INTO_FROM_ABOVE = -2,
            INTO_FROM_BELOW = -1,
            NONE = 0,
            SHIFT_SLIDE_TO = 1,
            LEGATO_SLIDE_TO = 2,
            OUT_DOWNWARDS = 3,
            OUT_UPWARDS = 4,
        }

        // All Bend presets.
        public enum BendType { 
            NONE = 0, //No Preset.
            // Bends
            // =====
            BEND = 1, //A simple bend.
            BEND_RELEASE = 2, //A bend and release afterwards.
            BEND_RELEASE_BEND = 3, //A bend, then release and rebend.
            PREBEND = 4, //Prebend.
            PREBEND_RELEASE = 5, //Prebend and then release.
            // Tremolo Bar
            // ===========
            DIP = 6, //Dip the bar down and then back up.
            DIVE = 7, //Dive the bar.
            RELEASE_UP = 8, //Release the bar up.
            INVERTED_DIP = 9, //Dip the bar up and then back down.
            RETURN = 10, //Return the bar.
            RELEASE_DOWN = 11 //Release the bar down.
        }

        // All transition types for grace notes.
        public enum GraceEffectTransition {
            NONE = 0, //No transition.
            SLIDE = 1, //Slide from the grace note to the real one.
            BEND = 2, //Perform a bend from the grace note to the real one.
            HAMMER = 3 //Perform a hammer on.
        }

        // This effect is used to describe string bends and tremolo bars.
        public class BendEffect
        {
            // A single point within the BendEffect.
            public class BendPoint
            {

                public int Position { get; set; } = 0;
                public int Value { get; set; } = 0;
                public bool Vibrato { get; set; } = false;

                public BendPoint(int position, int value, bool vibrato)
                {
                    Position = position;
                    Value = value;
                    Vibrato = vibrato;
                }

                // Gets the exact time when the point need to be played (MIDI).
                //      param duration: the full duration of the effect.
                public int GetTime(float duration) {
                    return (int)(duration * this.Position / BendEffect.MAX_POSITION);

                }
            }
            //The note offset per bend point offset.
            public static readonly int SEMITONE_LENGTH = 1;

            // The max position of the bend points (x axis)
            public static readonly int MAX_POSITION = 12;

            // The max value of the bend points (y axis)
            public static  readonly int MAX_VALUE = SEMITONE_LENGTH * 12;

            public BendType Type { get; private set; } = BendType.NONE;
            public int Value { get; private set; } = 0;
            //public List<BendPoint> Points = new List<BendPoint>();
            public BendPoint[]? Points { get; private set; }
            public BendEffect(BendType type, int value, BendPoint[]? points)
            {
                Type = type;
                Value = value;
                Points = points;
            }
        }
                
        public class GraceEffect
        {
            public sbyte Fret { get; private set; } = -1;
            public int Velocity { get; private set; } = 0;
            public int Duration { get; private set; } = 0;

            public bool isDead { get; private set; } = false;
            public bool isOnBeat { get; private set; } = false;
            public GraceEffectTransition Transition { get; private set; } = GraceEffectTransition.NONE;

            public GraceEffect(sbyte fret, int velocity, int duration, GraceEffectTransition transition)
            {
                Fret = fret;
                Velocity = velocity;
                Duration = duration;
                Transition = transition;
                isDead = fret == -1;
                isOnBeat = false;
            }
        }
        
        public class HarmonicEffect
        {
            public HarmonicEffectType Type { get; set; }
            public uint Fret { get; set; }
        }
        public class NaturalHarmonicEffect : HarmonicEffect
        {
            public NaturalHarmonicEffect()
            {
                Type = HarmonicEffectType.NATURAL;
            }
        }
        public class ArtificialHarmonicEffect : HarmonicEffect
        {
            //public PitchClass? Pitch { get; set; } = null;
            public uint Pitch { get; set; } = 0;
            public int Octave { get; set; } = -1;
            public ArtificialHarmonicEffect()
            {
                base.Type = HarmonicEffectType.ARTIFICIAL;
            }
        }
        public class TremoloPickingEffect
        {

        }
        public class TrillEffect
        {

        }

     
        public bool AccentuatedNote { get; set; } = false;
        public BendEffect? Bend { get; set; } = null;
        public bool GhostNote { get; set; } = false;
        public GraceEffect? Grace { get; set; } = null;
        public bool Hammer { get; set; } = false;
        public HarmonicEffect? Harmonic { get; set; } = null;
        public bool HeavyAccentuatedNote { get; set; } = false;
        public Note.Fingering LeftHandFinger { get; set; } = Note.Fingering.OPEN;
        public bool LetRing { get; set; } = false;
        public bool PalmMute { get; set; } = false;
        public Note.Fingering RightHandFinger { get; set; } = Note.Fingering.OPEN;
        //public Slide[] Slides { get; set; }
        public SlideType[]? Slides { get; set; }
        public bool Staccato { get; set; } = false;
        public TremoloPickingEffect? TremoloPicking { get; set; } = null;
        public TrillEffect? Trill { get; set; } = null;
        public bool Vibrato { get; set; } = false;
    }

}
