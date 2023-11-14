namespace GuitarProSharp
{
    public class Beat
    {
        public enum SlapEffect
        {
            NONE = 0,
            TAPPING = 1,
            SLAPPING = 2,
            POPPING = 3
        }
        public enum BeatStrokeDirection
        {
            NONE,
            UP,
            DOWN,
        }
        public enum BeatStatus
        {
            EMPTY = 0,
            NORMAL = 1,
            REST = 2
        }
        public class BeatEffect
        {
            public int Stroke { get; set; }  //int?

            public bool hasResgueado { get; set; } = false;
            public BeatStrokeDirection pickStroke { get; set; } = BeatStrokeDirection.NONE;
            public Chord? Chord { get; set; }
            public bool FadeIn { get; set; } = false;
            public NoteEffect.BendEffect? TremoloBar { get; set; } = null;
            public MixTableChange? MixTableChange { get; set; } = null;
            public SlapEffect SlapEffect { get; set; } = SlapEffect.NONE;
            public bool Vibrato { get; set; }
        }

        //public Voice Voice {get; set;}
        public List<Note> Notes { get; private set; } = new List<Note>();

        public Duration? Duration { get; set; } = null;

        public float Start { get; set; } = 0;

        public BeatStatus Status { get; set; } = BeatStatus.EMPTY;

        public string? Text { get; set; }
        public BeatEffect Effect { get; set; } = new BeatEffect();
        public Beat(float start)
        {
            this.Start = start;
        }
    }

}
