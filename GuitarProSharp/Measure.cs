namespace GuitarProSharp
{
    //A measure contains multiple voices of beats.
    public class Measure
    {
        public enum MeasureClef
        {
            TREBLE = 0,
            BASS = 1,
            TENOR = 2,
            ALTO = 3
        }

        public Track Track { get; private set; }
        public Song.MeasureHeader Header { get; private set; }
        public MeasureClef Clef { get; set; } = MeasureClef.TREBLE;
        //public uint Number { get; set; } = 0;
        public float Start { get; set; } = Duration.QUATER_TIME;
        public List<Voice> Voices { get; } = new List<Voice>();


        public Measure(Track track, Song.MeasureHeader header)
        {
            this.Track = track;
            this.Header = header;
        }
    }
}
