namespace GuitarProSharp
{
    public enum VoiceDirection
    {
        NONE,
        UP,
        DOWN
    }
    public class Voice
    {
        //public Measure Measure { get; set; }
        public List<Beat> Beats { get; set; } = new List<Beat>();
        public VoiceDirection VoiceDirection { get; set; } = VoiceDirection.NONE;
    }
}
