namespace GuitarProSharp
{
    public struct TimeSignature
    {
        public sbyte Numerator { get; set; } = 4;
        //TODO: Denominator is of type Duration?
        public Duration Denominator { get; set; } 
        public TimeSignature(sbyte numerator, sbyte denominator)
        {
            this.Numerator = numerator;
            this.Denominator = new Duration(denominator);
        }
        public TimeSignature()
        {
            this.Denominator = new Duration();
        }
        public override string ToString()
        {
            return $"numerator={Numerator},denominator={Denominator}";
        }
    }
}
