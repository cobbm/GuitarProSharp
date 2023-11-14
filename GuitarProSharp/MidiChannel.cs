namespace GuitarProSharp
{
    public class MidiChannel
    {
        public readonly uint channelIndex;

        public readonly uint port;
        public readonly uint channel;

        public uint EffectChannel { get; set; } = 0;
        public bool hasEffectChannel { get; set; } = false;

        public bool isPercussionChannel { get; set; } = false;

        public long Instrument { get; set; } = 25;
        public short Volume { get; set; } = 104;
        public short Balance { get; set; } = 64;
        public short Chorus { get; set; } = 0;
        public short Reverb { get; set; } = 0;
        public short Phaser { get; set; } = 0;
        public short Tremolo { get; set; } = 0;
        public short Bank { get; set; } = 0;

        //public short blank1 = 0;
        //public short blank2 = 0;
        public MidiChannel(uint channelIndex)
        {
            this.channelIndex = channelIndex;

            this.port = (channelIndex / 16) + 1;
            this.channel = (channelIndex % 16) + 1;
        }


        public override string ToString()
        {
            return $"port={port},channel={channel},instrument={Instrument},volume={Volume},balance={Balance},chorus={Chorus},reverb={Reverb},phaser={Phaser},tremolo={Tremolo}";
        }
    }

}
