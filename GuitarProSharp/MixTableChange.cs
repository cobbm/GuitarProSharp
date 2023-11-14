namespace GuitarProSharp
{
    //public class Effect
    //{
    //      public Chord Chord { get; set; }
    //}

    public struct MixTableChange
    {
        public sbyte Instrument { get; set; } = -1;
        public sbyte InstrumentDuration { get; set; } = -1;
        public sbyte Volume { get; set; } = -1;
        public sbyte VolumeDuration { get; set; } = -1;
        public sbyte Balance { get; set; } = -1;
        public sbyte BalanceDuration { get; set; } = -1;
        public sbyte Chorus { get; set; } = -1;
        public sbyte ChorusDuration { get; set; } = -1;
        public sbyte Reverb { get; set; } = -1;
        public sbyte ReverbDuration { get; set; } = -1;
        public sbyte Phaser { get; set; } = -1;
        public sbyte PhaserDuration { get; set; } = -1;
        public sbyte Tremolo { get; set; } = -1;
        public sbyte TremoloDuration { get; set; } = -1;
        public int Tempo { get; set; } = -1;
        public sbyte TempoDuration { get; set; } = -1;
        public bool HideTempo { get; set; } = false;
        public MixTableChange(sbyte instrument, sbyte volume, sbyte balance, sbyte chorus, sbyte reverb, sbyte phaser, sbyte tremolo, int tempo)
        {
            Instrument = instrument;
            Volume = volume;
            Balance = balance;
            Chorus = chorus;
            Reverb = reverb;
            Phaser = phaser;
            Tremolo = tremolo;
            Tempo = tempo;
        }
    }
}
