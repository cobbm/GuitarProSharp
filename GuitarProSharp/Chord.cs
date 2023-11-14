namespace GuitarProSharp
{
    public class Chord
    {
        //Type of the chord.
        public enum ChordType
        {
            MAJOR = 0, //Major chord.
            SEVENTH = 1, //Dominant seventh chord.
            MAJOR_SEVENTH = 2, //Major seventh chord.
            SIXTH = 3, //Add sixth chord.
            MINOR = 4, //Minor chord.
            MINOR_SEVENTH = 5, //Minor seventh chord.
            MINOR_MAJOR = 6, //Minor major seventh chord.
            MINOR_SIXTH = 7, //Minor add sixth chord.
            SUSPENDED_SECOND = 8, //Suspended second chord.
            SUSPENDED_FOURTH = 9, //Suspended fourth chord.
            SEVENTH_SUSPENDED_SECOND = 10, //Seventh suspended second chord.
            SEVENTH_SUSPENDED_FOURTH = 11, //Seventh suspended fourth chord.
            DIMINISHED = 12, //Diminished chord.
            AUGMENTED = 13, //Augmented chord.
            POWER = 14, //Power chord.
        }
        public uint Root { get; set; } = 0;
        public ChordType Type { get; set; } = ChordType.MAJOR;
        public uint Extension { get; set; } = 0;
        public uint Bass { get; set; } = 0;
        public uint Tonality { get; set; } = 0;
        public bool Add { get; set; } = false;
        public string Name { get; set; }
        public uint Fifth { get; set; } = 0;
        public uint Ninth { get; set; } = 0;
        public uint Eleventh { get; set; } = 0;

        public bool[] Omissions { get; set; } = { };

        public int FirstFret { get; set; }
        public uint[] Strings { get; set; }

        public Chord(string name, int firstFret, uint[] strings)
        {
            this.Name = name;
            this.FirstFret = firstFret;
            this.Strings = strings;
        }
    }
}
