using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarProSharp
{
    // A *n:m* tuplet
    public struct Tuplet
    {
        public readonly int enters = 1;
        public readonly int times = 1;
        public readonly bool isSupported = true;

        private static (int, int)[] supportedTuplets =
        {
                (1, 1),
                (3, 2),
                (5, 4),
                (6, 4),
                (7, 4),
                (9, 8),
                (10, 8),
                (11, 8),
                (12, 8),
                (13, 8)
            };

        public Tuplet(int enters, int times)
        {
            this.enters = enters;
            this.times = times;

            isSupported = supportedTuplets.Contains((enters, times));
        }
        public Tuplet()
        {
            //isSupported = true;
        }
        //TODO: implement Fraction class?
        public float ConvertTime(float time)
        {
            //Fraction result = new Fraction(time * times, enters)
            //if (result.denominator == 1)
            //    return result.numerator;
            //return result;
            float result = time * times / enters;

            return result;
        }

        public static Tuplet FromFraction(float frac)
        {
            throw new System.NotImplementedException();
        }
        public static Tuplet FromFraction(int numerator, int denominator)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return $"enters={enters},times={times}";
        }
    }
}
