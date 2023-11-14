namespace GuitarProSharp
{
    public class Duration
    {
        public static readonly int QUATER_TIME = 960;

        public static readonly int WHOLE = 1;
        public static readonly int HALF = 2;
        public static readonly int QUATER = 4;
        public static readonly int EIGHTH = 8;
        public static readonly int SIXTEENTH = 16;
        public static readonly int THIRTY_SECOND = 32;
        public static readonly int SIXTY_FOURTH = 64;
        public static readonly int HUNDRED_TWENTY_EIGHTH = 128;

        private static readonly int MIN_TIME = (int)((int)(QUATER_TIME * (4 / SIXTY_FOURTH)) * 2 / 3);

        public int Value { get; set; } = QUATER;
        public bool isDotted { get; set; } = false;

        public Tuplet Tuplet { get; set; } 

        public Duration(int value, bool dotted, Tuplet tuplet)
        {
            this.Value = value;
            this.isDotted = dotted;

            this.Tuplet = tuplet;
        }
        public Duration (int value)
        {
            this.Value = value;
            this.Tuplet = new Tuplet();
        }
        public Duration()
        {
            this.Tuplet = new Tuplet();
        }

        public float GetTime()
        {
            int result = (int)(QUATER_TIME * (4.0 / Value));
            if (isDotted)
            {
                result += result / 2;
            }
            return Tuplet.ConvertTime(result);
        }
        public int GetIndex()
        {
            int index = 0;
            int value = Value;
            while (true)
            {
                value = (value >> 1);
                if (value <= 0) break;
                index ++;
            }
            return index;
        }

        public static Duration FromTime(float time)
        {
            /*
            Fraction timeFrac = new Fraction(time, QUATER_TIME * 4);
            int exp = (int)(Math.Log(timeFrac, 2));
            double value = Math.Pow(2 , -exp);
            Tuplet tuplet = Tuplet.FromFraction(timeFrac * value);
            bool isDotted = false;
            if (!tuplet.isSupported)
            {
                // Check if it's dotted
                timeFrac = new Fraction(time, QUATER_TIME * 4) * new Fraction(2, 3);
                exp = (int)(Math.Log(timeFrac, 2));
                value = Math.Pow(2, -exp);
                tuplet = Tuplet.FromFraction(timeFrac * value);
                isDotted = true;
            }
            if (!tuplet.isSupported)
                throw new Exception($"cannot represent time '{time}' as a Guitar Pro duration");

            return new Duration((uint)value, isDotted, tuplet);
            */
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"value={Value},isDotted={isDotted},tuplet={{{Tuplet}}}";
        }

    }

}
