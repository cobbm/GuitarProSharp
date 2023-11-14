namespace GuitarProSharp
{
    /*
    FMajorFlat = (-8, 0)
    CMajorFlat = (-7, 0)
    GMajorFlat = (-6, 0)
    DMajorFlat = (-5, 0)
    AMajorFlat = (-4, 0)
    EMajorFlat = (-3, 0)
    BMajorFlat = (-2, 0)
    FMajor = (-1, 0)
    CMajor = (0, 0)
    GMajor = (1, 0)
    DMajor = (2, 0)
    AMajor = (3, 0)
    EMajor = (4, 0)
    BMajor = (5, 0)
    FMajorSharp = (6, 0)
    CMajorSharp = (7, 0)
    GMajorSharp = (8, 0)

    DMinorFlat = (-8, 1)
    AMinorFlat = (-7, 1)
    EMinorFlat = (-6, 1)
    BMinorFlat = (-5, 1)
    FMinor = (-4, 1)
    CMinor = (-3, 1)
    GMinor = (-2, 1)
    DMinor = (-1, 1)
    AMinor = (0, 1)
    EMinor = (1, 1)
    BMinor = (2, 1)
    FMinorSharp = (3, 1)
    CMinorSharp = (4, 1)
    GMinorSharp = (5, 1)
    DMinorSharp = (6, 1)
    AMinorSharp = (7, 1)
    EMinorSharp = (8, 1)
    */
    public struct KeySignature
    {
        public int Root { get; set; }
        public int Type { get; set; }
        public KeySignature(int root, int type)
        {
            this.Root = root;
            this.Type = type;
        }

        public override string ToString()
        {
            return $"root={Root},type={Type} ({KeysToString.GetValueOrDefault(this) ?? "?"})";
        }

        public static readonly KeySignature FMajorFlat = new KeySignature(-8, 0);
        public static readonly KeySignature CMajorFlat = new KeySignature(-7, 0);
        public static readonly KeySignature GMajorFlat = new KeySignature(-6, 0);
        public static readonly KeySignature DMajorFlat = new KeySignature(-5, 0);
        public static readonly KeySignature AMajorFlat = new KeySignature(-4, 0);
        public static readonly KeySignature EMajorFlat = new KeySignature(-3, 0);
        public static readonly KeySignature BMajorFlat = new KeySignature(-2, 0);
        public static readonly KeySignature FMajor = new KeySignature(-1, 0);
        public static readonly KeySignature CMajor = new KeySignature(0, 0);
        public static readonly KeySignature GMajor = new KeySignature(1, 0);
        public static readonly KeySignature DMajor = new KeySignature(2, 0);
        public static readonly KeySignature AMajor = new KeySignature(3, 0);
        public static readonly KeySignature EMajor = new KeySignature(4, 0);
        public static readonly KeySignature BMajor = new KeySignature(5, 0);
        public static readonly KeySignature FMajorSharp = new KeySignature(6, 0);
        public static readonly KeySignature CMajorSharp = new KeySignature(7, 0);
        public static readonly KeySignature GMajorSharp = new KeySignature(8, 0);

        public static readonly KeySignature DMinorFlat = new KeySignature(-8, 1);
        public static readonly KeySignature AMinorFlat = new KeySignature(-7, 1);
        public static readonly KeySignature EMinorFlat = new KeySignature(-6, 1);
        public static readonly KeySignature BMinorFlat = new KeySignature(-5, 1);
        public static readonly KeySignature FMinor = new KeySignature(-4, 1);
        public static readonly KeySignature CMinor = new KeySignature(-3, 1);
        public static readonly KeySignature GMinor = new KeySignature(-2, 1);
        public static readonly KeySignature DMinor = new KeySignature(-1, 1);
        public static readonly KeySignature AMinor = new KeySignature(0, 1);
        public static readonly KeySignature EMinor = new KeySignature(1, 1);
        public static readonly KeySignature BMinor = new KeySignature(2, 1);
        public static readonly KeySignature FMinorSharp = new KeySignature(3, 1);
        public static readonly KeySignature CMinorSharp = new KeySignature(4, 1);
        public static readonly KeySignature GMinorSharp = new KeySignature(5, 1);
        public static readonly KeySignature DMinorSharp = new KeySignature(6, 1);
        public static readonly KeySignature AMinorSharp = new KeySignature(7, 1);
        public static readonly KeySignature EMinorSharp = new KeySignature(8, 1);

        private static readonly Dictionary<KeySignature, string> KeysToString = new Dictionary<KeySignature, string>(){
            { FMajorFlat, "FMajorFlat"},
            {CMajorFlat, "CMajorFlat"},
            {GMajorFlat, "GMajorFlat"},
            {DMajorFlat, "DMajorFlat"},
            {AMajorFlat, "AMajorFlat"},
            {EMajorFlat, "EMajorFlat"},
            {BMajorFlat, "BMajorFlat"},
            {FMajor, "FMajor"},
            {CMajor, "CMajor"},
            {GMajor, "GMajor"},
            {DMajor, "DMajor"},
            {AMajor, "AMajor"},
            {EMajor, "EMajor"},
            {BMajor, "BMajor"},
            {FMajorSharp, "FMajorSharp"},
            {CMajorSharp, "CMajorSharp"},
            {GMajorSharp, "GMajorSharp"},
            {DMinorFlat, "DMinorFlat"},
            {AMinorFlat, "AMinorFlat"},
            {EMinorFlat, "EMinorFlat"},
            {BMinorFlat, "BMinorFlat"},
            {FMinor, "FMinor"},
            {CMinor, "CMinor"},
            {GMinor, "GMinor"},
            {DMinor, "DMinor"},
            {AMinor, "AMinor"},
            {EMinor, "EMinor"},
            {BMinor, "BMinor"},
            {FMinorSharp, "FMinorSharp"},
            {CMinorSharp, "CMinorSharp"},
            {GMinorSharp, "GMinorSharp"},
            {DMinorSharp, "DMinorSharp"},
            {AMinorSharp, "AMinorSharp"},
            {EMinorSharp, "EMinorSharp"},
        };

    }

}
