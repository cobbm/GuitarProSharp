using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarProSharp
{
    public class GP3File
    {
        // A collection of velocities / dynamics.
        public static class Velocities
        {
            public static readonly int MIN_VELOCITY = 15;
            public static readonly int VELOCITY_INCREMENT = 16;
            public static readonly int PIANO_PIANISSIMO = MIN_VELOCITY;
            public static readonly int PIANISSIMO = MIN_VELOCITY + VELOCITY_INCREMENT;
            public static readonly int PIANO = MIN_VELOCITY + VELOCITY_INCREMENT * 2;
            public static readonly int MEZZO_PIANO = MIN_VELOCITY + VELOCITY_INCREMENT * 3;
            public static readonly int MEZZO_FORTE = MIN_VELOCITY + VELOCITY_INCREMENT * 4;
            public static readonly int FORTE = MIN_VELOCITY + VELOCITY_INCREMENT * 5;
            public static readonly int FORTISSIMO = MIN_VELOCITY + VELOCITY_INCREMENT * 6;
            public static readonly int FORTE_FORTISSIMO = MIN_VELOCITY + VELOCITY_INCREMENT * 7;
            public static readonly int DEFAULT = FORTE;
        }
        public static readonly int BEND_POSITION = 60;
        public static readonly int BEND_SEMITONE = 25;

        public static Song? LoadFromFile(string filename)
        {

            Song? song = null;
            using (FileStream s = File.OpenRead(filename))
            {
                Console.WriteLine($"Loading file: \"{filename}\" ({s.Length} bytes)...");
                using (BinaryReader r = new BinaryReader(s))
                {
                    song = GP3File.ReadFromStream(r);
                }
            }
            return song;
        }

        // A song consists of score information, triplet feel, tempo, song
        // key, MIDI channels, measure and track count, measure headers,
        // tracks, measures.
        //      -Version: :ref:`byte-size - string` of size 30.
        //      - Score information.See :meth:`readInfo`.
        //      -Triplet feel: :ref:`bool`. If value is true, then triplet feel
        //              is set to eigth.
        //      - Tempo: :ref:`int`.
        //      -Key: :ref:`int`. Key signature of the song.
        //      - MIDI channels.See :meth:`readMidiChannels`.
        //      -Number of measures: :ref:`int`.
        //      -Number of tracks: :ref:`int`.
        //      -Measure headers.See :meth:`readMeasureHeaders`.
        //      -Tracks.See :meth:`readTracks`.
        //      -Measures.See :meth:`readMeasures`.
        public static Song ReadFromStream(BinaryReader inputStream)
        {
            //inputStream = inputFileStream;
            Console.WriteLine($"[GP3File] Reading GP3 file: {inputStream.BaseStream.Length} bytes...");

            var song = new Song();

            song.Version = ReadVersion(inputStream);
            // song.versionTuple = self.versionTuple

            song.Metadata = ReadSongMetadata(inputStream);

            //Console.WriteLine($"Song Metadata: {song.Metadata.ToString()}");
            bool _triplet = inputStream.ReadBoolean();
            song.TripletFeel = _triplet ? Song.TripletFeelType.EIGHTH : Song.TripletFeelType.NONE;

            song.Tempo = inputStream.ReadUInt32();
            song.Key = ReadSongKeySignature(inputStream);

            //MidiChannels = ReadSongMidiChannels(inputStream);
            //song.MeasureCount = inputStream.ReadUInt32();
            //song.TrackCount = inputStream.ReadUInt32();
            MidiChannel[] midiChannels = ReadSongMidiChannels(inputStream);
            uint measureCount = inputStream.ReadUInt32();
            uint trackCount = inputStream.ReadUInt32();

            song.MeasureHeaders = ReadMeasureHeaders(inputStream, measureCount);
            song.Tracks = ReadTracks(inputStream, trackCount, midiChannels);
            //Measure[] measures = ReadMeasures(inputStream);
            ReadMeasures(inputStream, song);
            return song;
        }
        //private static string ReadByteSizeString(BinaryReader s)
        //{
        //    byte size = s.ReadByte();
        //    Byte[] bytes = s.ReadBytes(size);
        //    return Encoding.ASCII.GetString(bytes);
        //}
        private static string ReadIntByteSizeString(BinaryReader s)
        {
            //BinaryReader reads this data type in little-endian format.
            uint isize = s.ReadUInt32();
            //byte size =s.ReadByte();
            //Byte[] bytes = s.ReadBytes((int)size);
            //return Encoding.ASCII.GetString(bytes);

            return ReadByteSizeString(s);
        }
        private static string ReadByteSizeString(BinaryReader s, int size = 0)
        {
            //BinaryReader reads this data type in little-endian format.
            byte actualSize = s.ReadByte();
            Byte[] bytes = s.ReadBytes(size > 0 ? size : actualSize);
            return Encoding.ASCII.GetString(bytes, 0, actualSize);
        }

        public static short ReadMidiChannelShort(BinaryReader s)
        {
            sbyte b = s.ReadSByte();

            //value = max(-32768, min(32767, (b << 3) - 1))
            //return max(value, -1) + 1
            short value = (short)((b << 3) - 1);
            short outValue = (short)(value + 1);

            short testValue = (short)Math.Max(-32768, Math.Min(32767, (b << 3) - 1));
            short testOutValue = (short)(Math.Max(testValue, (short)-1) + 1);

            return testOutValue;
        }

        private static string ReadVersion(BinaryReader s)
        {
            return ReadByteSizeString(s, 30);
        }

        private static Song.SongMetadata ReadSongMetadata(BinaryReader s)
        {
            Song.SongMetadata metadata;
            metadata.title = ReadIntByteSizeString(s);
            metadata.subtitle = ReadIntByteSizeString(s);
            metadata.artist = ReadIntByteSizeString(s);
            metadata.album = ReadIntByteSizeString(s);
            metadata.words = ReadIntByteSizeString(s);
            metadata.copyright = ReadIntByteSizeString(s);
            metadata.tabbedBy = ReadIntByteSizeString(s);
            metadata.instructions = ReadIntByteSizeString(s);

            uint noticeLines = s.ReadUInt32();
            string[] notice = new string[noticeLines];
            for (int i = 0; i < noticeLines; i++)
            {
                notice[i] = ReadIntByteSizeString(s);
            }
            metadata.notice = notice;
            return metadata;
        }
        private static KeySignature ReadSongKeySignature(BinaryReader s)
        {
            uint key = s.ReadUInt32();
            return new KeySignature((int)key, 0);
        }

        // Guitar Pro format provides 64 channels(4 MIDI ports by 16 channels), the channels are stored in this order:
        //      port1/channel1
        //      port1/channel2
        //      …
        //      port1/channel16
        //      port2/channel1
        //      …
        //      port4/channel16
        // Each channel has the following form:
        //      Instrument: Int.
        //      Volume: Byte.
        //      Balance: Byte.
        //      Chorus: Byte.
        //      Reverb: Byte.
        //      Phaser: Byte.
        //      Tremolo: Byte.
        //      blank1: Byte.
        //      blank2: Byte.
        private static MidiChannel[] ReadSongMidiChannels(BinaryReader s)
        {
            MidiChannel[] channels = new MidiChannel[4 * 16];
            for (uint i = 0; i < 4 * 16; i++)
            {
                MidiChannel channel = new MidiChannel(i);

                int instrument = s.ReadInt32();
                if (channel.isPercussionChannel && instrument == -1)
                {
                    instrument = 0;
                }
                channel.Instrument = instrument;
                channel.Volume = ReadMidiChannelShort(s);
                channel.Balance = ReadMidiChannelShort(s);
                channel.Chorus = ReadMidiChannelShort(s);
                channel.Reverb = ReadMidiChannelShort(s);
                channel.Phaser = ReadMidiChannelShort(s);
                channel.Tremolo = ReadMidiChannelShort(s);
                //channel.blank1 = s.ReadByte();
                //channel.blank2 = s.ReadByte();

                // Backward compatibility with version 3.0
                s.ReadBytes(2); //skip 2 bytes

                //Console.WriteLine($"Creating MIDI Channel index {i} ({channel})");
                channels[i] = channel;
            }

            return channels;
        }

        private static Song.MeasureHeader[] ReadMeasureHeaders(BinaryReader s, uint measureCount)
        {
            // The *measures* are written one after another, their number have
            // been specified previously.
            Song.MeasureHeader[] headers = new Song.MeasureHeader[measureCount];
            Song.MeasureHeader? prev = null;
            for (uint i = 0; i < measureCount; i++)
            {
                Song.MeasureHeader header = ReadMeasureHeader(s, i + 1, prev, headers);
                headers[i] = header;
                prev = header;
            }
            return headers;
        }

        // The first byte is the measure's flags. It lists the data given in the
        // current measure.
        //      -*0x01 *: numerator of the key signature
        //      - *0x02 *: denominator of the key signature
        //      - *0x04 *: beginning of repeat
        //      - *0x08 *: end of repeat
        //      - *0x10 *: number of alternate ending
        //      -*0x20 *: presence of a marker
        //      -*0x40 *: tonality of the measure
        //      -*0x80 *: presence of a double bar
        //          Each of these elements is present only if the corresponding bit
        //          is a 1.
        //          The different elements are written(if they are present) from
        //          lowest to highest bit.
        //          Exceptions are made for the double bar and the beginning of
        //          repeat whose sole presence is enough, complementary data is not
        //          necessary.
        //      - Numerator of the key signature: :ref:`byte`.
        //      -Denominator of the key signature: :ref:`byte`.
        //      -End of repeat: :ref:`byte`.
        //              Number of repeats until the previous beginning of repeat.
        //      - Number of alternate ending: :ref:`byte`.
        //      -Marker: see :meth:`GP3File.readMarker`.
        //      -Tonality of the measure: 2 :ref:`Bytes<byte>`. These values
        //              encode a key signature change on the current piece.First byte
        //              is key signature root, second is key signature type.
        private static Song.MeasureHeader ReadMeasureHeader(BinaryReader s, uint number, Song.MeasureHeader? prev, Song.MeasureHeader?[] headers)
        {
            byte flags = s.ReadByte();

            Song.MeasureHeader header = new Song.MeasureHeader(number, flags);

            if ((flags & (int)Song.MeasureHeader.Flags.NUMERATOR) != 0)
            {
                TimeSignature ts = header.TimeSignature;
                ts.Numerator = s.ReadSByte();
                header.TimeSignature = ts;
            }
            else
            {
                //numerator = previous.timeSignature.denominator;
                if (prev != null)
                {
                    TimeSignature ts = header.TimeSignature;
                    ts.Numerator = prev.TimeSignature.Numerator;
                    header.TimeSignature = ts;
                }
            }
            if ((flags & (int)Song.MeasureHeader.Flags.DENOMINATOR) != 0)
            {
                header.TimeSignature.Denominator.Value = s.ReadSByte();
            }
            else
            {
                //denominator = previous.timeSignature.denominator;
                if (prev != null)
                {
                    header.TimeSignature.Denominator.Value = prev.TimeSignature.Denominator.Value;
                }
            }
            if (header.isRepeatClose)
            {
                header.RepeatClose = s.ReadSByte();
            }
            if (header.isRepeatAlternative)
            {
                header.RepeatAlternative = (uint)ReadHeaderRepeatAlternative(s, header, headers);
            }
            if (header.isMarker)
            {
                header.Marker = ReadHeaderMarker(s);
            }
            if (header.hasTonality)
            {
                sbyte tonailtyRoot = s.ReadSByte();
                sbyte tonailityType = s.ReadSByte();
                header.KeySignature = new KeySignature((int)tonailtyRoot, (int)tonailityType);
            }
            else if (number > 1)
            {
                if (prev != null)
                {
                    header.KeySignature = prev.KeySignature;
                }
            }

            return header;
        }

        // The markers are written in two steps. First is written an
        // integer equal to the marker's name length + 1, then a string
        // containing the marker's name. Finally the marker's color is
        // written.
        private static Song.Marker ReadHeaderMarker(BinaryReader s)
        {
            string title = ReadIntByteSizeString(s);
            Color color = ReadColor(s);
            return new Song.Marker(title, color);
        }

        // Colors are used by :class:`guitarpro.models.Marker` and
        // :class:`guitarpro.models.Track`. They consist of 3 consecutive
        // bytes and one blank byte.
        private static Color ReadColor(BinaryReader s)
        {
            byte[] colorBytes = s.ReadBytes(4);
            return Color.FromArgb(colorBytes[0], colorBytes[1], colorBytes[2]);
        }

        private static int ReadHeaderRepeatAlternative(BinaryReader s, Song.MeasureHeader header, Song.MeasureHeader?[] headers)
        {
            byte value = s.ReadByte();
            uint existingAlternatives = 0;
            for (int i = headers.Length - 1; i >= 0; i--)
            {
                Song.MeasureHeader? h = headers[i];
                if (h != null)
                {
                    if (h.isRepeatOpen) break;
                    existingAlternatives |= h.RepeatAlternative;
                }
            }
            return (int)((1 << value) - 1 ^ existingAlternatives);
        }

        private static Track[] ReadTracks(BinaryReader s, uint trackCount, MidiChannel?[] channels)
        {
            Track[] tracks = new Track[trackCount];
            for (int i = 0; i < trackCount; i++)
            {
                tracks[i] = ReadTrack(s, channels);
            }
            return tracks;
        }

        // The first byte is the track's flags. It presides the track's
        // attributes:
        //      - *0x01*: drums track
        //      - *0x02*: 12 stringed guitar track
        //      - *0x04*: banjo track
        //      - *0x08*: *blank*
        //      - *0x10*: *blank*
        //      - *0x20*: *blank*
        //      - *0x40*: *blank*
        //      - *0x80*: *blank*
        // Flags are followed by:
        //      - Name: :ref:`byte-size-string`. A 40 characters long string
        //              containing the track's name.
        //      - Number of strings: :ref:`int`. An integer equal to the number
        //              of strings of the track.
        //      - Tuning of the strings: List of 7 :ref:`Ints<int>`. The tuning
        //              of the strings is stored as a 7-integers table, the "Number of
        //              strings" first integers being really used. The strings are
        //              stored from the highest to the lowest.
        //      - Port: :ref:`int`. The number of the MIDI port used.
        //      - Channel.See :meth:`GP3File.readChannel`.
        //      - Number of frets: :ref:`int`. The number of frets of the
        //              instrument.
        //      - Height of the capo: :ref:`int`. The number of the fret on
        //              which a capo is set.If no capo is used, the value is 0.
        //      - Track's color. The track's displayed color in Guitar Pro.
        private static Track ReadTrack(BinaryReader s, MidiChannel?[] channels)
        {
            byte flags = s.ReadByte();
            string name = ReadByteSizeString(s, 40);
            uint stringCount = s.ReadUInt32();

            Track.GuitarString?[] strings = new Track.GuitarString?[stringCount];
            for (int str = 0; str < 7; str++)
            {
                uint stringTuning = s.ReadUInt32();
                if (stringCount > str)
                {
                    strings[str] = new Track.GuitarString((uint)(str + 1), stringTuning);
                }
            }

            Track t = new Track(name, flags, strings);

            uint port = s.ReadUInt32();
            MidiChannel? channel = ReadTrackMidiChannel(s, channels);
            if (channel != null && channel.channel == 9)
            {
                channel.isPercussionChannel = true;
            }

            uint fretCount = s.ReadUInt32();
            uint offset = s.ReadUInt32();
            Color color = ReadColor(s);

            t.MidiPort = port;
            t.MidiChannel = channel;
            t.FretCount = fretCount;
            t.Offset = offset;
            t.Color = color;

            return t;
        }

        // MIDI channel in Guitar Pro is represented by two integers. First
        // is zero-based number of channel, second is zero-based number of
        // channel used for effects.
        private static MidiChannel? ReadTrackMidiChannel(BinaryReader s, MidiChannel?[] channels)
        {

            uint channelIndex = s.ReadUInt32() - 1;
            uint effectChannel = s.ReadUInt32() - 1;

            if (0 <= channelIndex && channelIndex < channels.Length)
            {
                MidiChannel? channel = channels[channelIndex];
                if (channel != null)
                {
                    if (channel.Instrument < 0)
                    {
                        channel.Instrument = 0;
                    }
                    if (!channel.isPercussionChannel)
                    {
                        channel.EffectChannel = effectChannel;
                        channel.hasEffectChannel = true;
                    }
                    return channel;
                }
            }
            return null;
        }

        // Measures are written in the following order:
        //      - measure 1/track 1
        //      - measure 1/track 2
        //      - ...
        //      - measure 1/track m
        //      - measure 2/track 1
        //      - measure 2/track 2
        //      - ...
        //      - measure 2/track m
        //      - ...
        //      - measure n/track 1
        //      - measure n/track 2
        //      - ...
        //      - measure n/track m
        private static void ReadMeasures(BinaryReader s, Song song)
        {
            float start = Duration.QUATER_TIME;
            if (song.MeasureHeaders != null)
            {
                for (int i = 0; i < song.MeasureHeaders.Length; i++)
                {
                    Song.MeasureHeader? header = song.MeasureHeaders[i];

                    if (header != null)
                    {
                        header.Start = start;
                        if (song.Tracks != null)
                        {
                            for (int j = 0; j < song.Tracks.Length; j++)
                            {
                                Track track = song.Tracks[j];

                                Measure measure = ReadMeasure(s, track, header);
                                track.Measures.Add(measure);
                            }
                        }
                        start += header.GetLength();
                    }
                }
            }
        }

        private static Measure ReadMeasure(BinaryReader s, Track track, Song.MeasureHeader header)
        {
            // The measure is written as number of beats followed by sequence
            // of beats.
            Measure measure = new Measure(track, header);

            Voice v = ReadVoice(s, measure);
            measure.Voices.Add(v);

            return measure;
        }

        private static Voice ReadVoice(BinaryReader s, Measure measure)
        {
            Voice voice = new Voice();

            float start = measure.Start;

            uint beatCount = s.ReadUInt32();

            //Beat?[] beats = new Beat[beatCount];
            for (int i = 0; i < beatCount; i++)
            {
                //start += ReadBeat(s, start, voice);
                Beat b = ReadBeat(s, start, measure, voice);
                if (b.Status != Beat.BeatStatus.EMPTY)
                {
                    if (b.Duration != null)
                    {
                        start += b.Duration.GetTime();
                    } else
                    {
                        throw new Exception("Beat Duration is null!");
                    }
                }
            }

            return voice;
        }

        // The first byte is the beat flags. It lists the data present in
        // the current beat:
        //      - *0x01*: dotted notes
        //      - *0x02*: presence of a chord diagram
        //      - *0x04*: presence of a text
        //      - *0x08*: presence of effects
        //      - *0x10*: presence of a mix table change event
        //      - *0x20*: the beat is a n-tuplet
        //      - *0x40*: status: True if the beat is empty of if it is a rest
        //      - *0x80*: *blank*
        // Flags are followed by:
        //      - Status: :ref:`byte`. If flag at*0x40* is true, read one byte.
        //              If value of the byte is *0x00* then beat is empty, if value is
        //              *0x02* then the beat is rest.
        //      - Beat duration: :ref:`byte`. See :meth:`readDuration`.
        //      - Chord diagram.See :meth:`readChord`.
        //      - Text: :ref:`int-byte-size-string`.
        //      - Beat effects. See :meth:`readBeatEffects`.
        //      - Mix table change effect. See :meth:`readMixTableChange`.
        private static Beat ReadBeat(BinaryReader s, float start, Measure measure, Voice voice)
        {
            byte flags = s.ReadByte();
            Beat beat = GetBeat(voice.Beats, start);

            if ((flags & 0x40) != 0)
            {
                beat.Status = (Beat.BeatStatus)s.ReadByte(); //gp.BeatStatus(s.ReadByte());
            }
            else
            {
                beat.Status = Beat.BeatStatus.NORMAL;
            }

            beat.Duration = ReadDuration(s, flags);


            NoteEffect noteEffect = new NoteEffect();

            if ((flags & 0x02) != 0)
            {
                beat.Effect.Chord = ReadChord(s, (uint)measure.Track.Strings.Length);
            }
            if ((flags & 0x04) != 0)
            {
                beat.Text = ReadIntByteSizeString(s);
            }
            if ((flags & 0x08) != 0)
            {
                Chord? chord = beat.Effect.Chord;
                beat.Effect = ReadBeatEffects(s, noteEffect);
                beat.Effect.Chord = chord;
            }
            if ((flags & 0x10) != 0)
            {
                var mixTableChange = ReadMixTableChange(s, measure);
                beat.Effect.MixTableChange = mixTableChange;
            }
            //read the notes
            ReadNotes(s, measure.Track, beat, noteEffect);
            return beat;
        }

        //Get beat from measure by start time.
        private static Beat GetBeat(List<Beat> beats, float start)
        {
            for (int i = beats.Count - 1; i >= 0; i--)
            {
                Beat b = beats[i];
                if (b.Start == start) return b;
            }
            Beat newBeat = new Beat(start);
            beats.Add(newBeat);
            return newBeat;
        }

        //Read beat duration.
        // Duration is composed of byte signifying duration and an integer
        // that maps to :class:`guitarpro.models.Tuplet`.
        // The byte maps to following values:
        //      - *-2*: whole note
        //      - *-1*: half note
        //      -  *0*: quarter note
        //      -  *1*: eighth note
        //      -  *2*: sixteenth note
        //      -  *3*: thirty-second note

        //If flag at*0x20* is true, the tuplet is read.
        private static Duration ReadDuration(BinaryReader s, byte flags)
        {
            sbyte rawValue = s.ReadSByte();

            Tuplet tuplet = new Tuplet();

            if ((flags & 0x20) != 0)
            {
                uint rawTuplet = s.ReadUInt32();

                switch (rawTuplet)
                {
                    case 3:
                        tuplet = new Tuplet(3, 2);
                        break;
                    case 5:
                        tuplet = new Tuplet(5, 4);
                        break;
                    case 6:
                        tuplet = new Tuplet(6, 4);
                        break;
                    case 7:
                        tuplet = new Tuplet(7, 4);
                        break;
                    case 9:
                        tuplet = new Tuplet(9, 8);
                        break;
                    case 10:
                        tuplet = new Tuplet(10, 8);
                        break;
                    case 11:
                        tuplet = new Tuplet(11, 8);
                        break;
                    case 12:
                        tuplet = new Tuplet(12, 8);
                        break;
                    case 13:
                        tuplet = new Tuplet(13, 8);
                        break;
                    default:
                        throw new Exception($"Unknown BeatDuration value: '{rawTuplet}'");
                }
            }

            return new Duration((int)(1 << rawValue + 2), (flags & 0x01) != 0, tuplet);
        }

        // First byte is chord header.If it's set to 0, then following
        // chord is written in default (GP3) format. If chord header is set
        // to 1, then chord diagram in encoded in more advanced (GP4)
        // format.
        private static Chord ReadChord(BinaryReader s, uint stringCount)
        {
            bool gp4Format = s.ReadBoolean();
            if (gp4Format)
            {
                return ReadGP4Chord(s, stringCount);
            }
            else
            {
                return ReadGP3Chord(s, stringCount);
            }
        }

        // Read chord diagram encoded in GP3 format.
        // Chord diagram is read as follows:
        //  - Name: :ref:`int-byte-size-string`. Name of the chord, e.g.
        //      * Em*.

        //      - First fret: :ref:`int`. The fret from which the chord is
        //          displayed in chord editor.

        //      - List of frets: 6 :ref:`Ints<int>`. Frets are listed in order:
        //          fret on the string 1, fret on the string 2, ..., fret on the
        //          string 6. If string is untouched then the values of fret is
        //          *-1*.
        private static Chord ReadGP3Chord(BinaryReader s, uint stringCount)
        {
            string chordName = ReadIntByteSizeString(s);
            int firstFret = s.ReadInt32();

            uint[] strings = new uint[stringCount];

            if (firstFret > 0) //?
            {
                for (int i = 0; i < 6; i++)
                {
                    uint fret = s.ReadUInt32();
                    if (i < stringCount)
                    {
                        strings[i] = fret;
                    }
                }
            }
            return new Chord(chordName, firstFret, strings);
        }

        // Read new-style(GP4) chord diagram.
        // New-style chord diagram is read as follows:
        //      - Sharp: :ref:`bool`. If true, display all semitones as sharps,
        //              otherwise display as flats.
        //      - Blank space, 3 :ref:`Bytes<byte>`.
        //      - Root: :ref:`int`. Values are:
        //              * -1 for customized chords
        //              *  0: C
        //              *  1: C#
        //              * ...
        //      - Type: :ref:`int`. Determines the chord type as followed.See
        //              :class:`guitarpro.models.ChordType` for mapping.
        //      - Chord extension: :ref:`int`. See
        //              :class:`guitarpro.models.ChordExtension` for mapping.
        //      - Bass note: :ref:`int`. Lowest note of chord as in *C/Am*.
        //      - Tonality: :ref:`int`. See
        //              :class:`guitarpro.models.ChordAlteration` for mapping.
        //      - Add: :ref:`bool`. Determines if an "add" (added note) is
        //              present in the chord.
        //      - Name: :ref:`byte-size-string`. Max length is 22.
        //      - Fifth alteration: :ref:`int`. Maps to
        //              :class:`guitarpro.models.ChordAlteration`.
        //      - Ninth alteration: :ref:`int`. Maps to
        //              :class:`guitarpro.models.ChordAlteration`.
        //      - Eleventh alteration: :ref:`int`. Maps to
        //              :class:`guitarpro.models.ChordAlteration`.
        //      - List of frets: 6 :ref:`Ints<int>`. Fret values are saved as
        //              in default format.
        //      - Count of barres: :ref:`int`. Maximum count is 2.
        //      - Barre frets: 2 :ref:`Ints<int>`.
        //      - Barre start strings: 2 :ref:`Ints<int>`.
        //      - Barre end string: 2 :ref:`Ints<int>`.
        //      - Omissions: 7 :ref:`Bools<bool>`. If the value is true then
        //              note is played in chord.
        //      - Blank space, 1 :ref:`byte`.
        private static Chord ReadGP4Chord(BinaryReader s, uint stringCount)
        {
            bool showSharp = s.ReadBoolean();
            s.ReadBytes(3);//skip 3 bytes

            uint root = s.ReadUInt32();
            uint type = s.ReadUInt32();
            uint extension = s.ReadUInt32();
            uint bass = s.ReadUInt32();
            uint tonality = s.ReadUInt32();
            bool add = s.ReadBoolean();
            string name = ReadByteSizeString(s, 22);
            uint fifth = s.ReadUInt32();
            uint ninth = s.ReadUInt32();
            uint eleventh = s.ReadUInt32();

            int firstFret = s.ReadInt32();

            uint[] strings = new uint[stringCount];

            for (int i = 0; i < 6; i++)
            {
                uint fret = s.ReadUInt32();
                if (i < stringCount)
                {
                    strings[i] = fret;
                }
            }
            uint barresCount = s.ReadUInt32();
            uint[] barreFrets = { s.ReadUInt32(), s.ReadUInt32() };
            uint[] barreStarts = { s.ReadUInt32(), s.ReadUInt32() };
            uint[] barreEnds = { s.ReadUInt32(), s.ReadUInt32() };
            //TODO: zip barre!

            bool[] omissions = new bool[7];
            for (int i = 0; i < 7; i++)
            {
                omissions[i] = s.ReadBoolean();
            }
            s.ReadBytes(1);//skip 1

            Chord chord = new Chord(name, firstFret, strings);
            chord.Root = root;
            chord.Type = (Chord.ChordType)type;
            chord.Extension = extension;
            chord.Bass = bass;
            chord.Tonality = tonality;
            chord.Add = add;
            chord.Fifth = fifth;
            chord.Ninth = ninth;
            chord.Eleventh = eleventh;
            chord.Omissions = omissions;
            //TODO: chord.Barre;
            //TODO: chord.omissions;


            return chord;
        }

        //Read beat effects.
        // The first byte is effects flags:
        //      -*0x01 *: vibrato
        //      - *0x02 *: wide vibrato
        //      -*0x04 *: natural harmonic
        //      -*0x08 *: artificial harmonic
        //      -*0x10 *: fade in
        //      -*0x20 *: tremolo bar or slap effect
        //      - *0x40 *: beat stroke direction
        //      - *0x80 *: *blank *

        //      -Tremolo bar or slap effect: :ref:`byte`. If it's 0 then
        //          tremolo bar should be read(see: meth:`readTremoloBar`).Else
        //          it's tapping and values of the byte map to:
        //              - *1 *: tap
        //              - *2 *: slap
        //              - *3 *: pop
        //      - Beat stroke direction. See: meth:`readBeatStroke`.
        private static Beat.BeatEffect ReadBeatEffects(BinaryReader s, NoteEffect noteEffect)
        {
            Beat.BeatEffect beatEffects = new Beat.BeatEffect();

            byte flags1 = s.ReadByte();

            noteEffect.Vibrato = (flags1 & 0x01) != 0;
            beatEffects.Vibrato = (flags1 & 0x02) != 0;
            beatEffects.FadeIn = (flags1 & 0x10) != 0;
            if ((flags1 & 0x20) != 0)
            {
                //byte flags2 = s.ReadByte();
                beatEffects.SlapEffect = ReadSlapEffect(s);
                if (beatEffects.SlapEffect == 0) // if beatEffects.slapEffect == gp.SlapEffect.none:
                {
                    beatEffects.TremoloBar = ReadTremoloBar(s);
                }
                else
                {
                    s.ReadUInt32();
                }
            }
            if ((flags1 & 0x40) != 0)
            {
                beatEffects.Stroke = ReadBeatStroke(s);
            }
            if ((flags1 & 0x04) != 0)
            {
                // In GP3 harmonics apply to the whole beat, not the individual
                // notes. Here we set the noteEffect for all the notes in the beat.
                noteEffect.Harmonic = new NoteEffect.NaturalHarmonicEffect();
            }
            if ((flags1 & 0x08) != 0)
            {
                noteEffect.Harmonic = new NoteEffect.ArtificialHarmonicEffect();
            }
            return beatEffects;
        }

        // Read tremolo bar beat effect.
        // The only type of tremolo bar effect Guitar Pro 3 supports is
        // :attr:`dip<guitarpro.models.BendType.dip>`. The value of the
        // effect is encoded in :ref:`Int` and shows how deep tremolo bar
        // is pressed.
        private static NoteEffect.BendEffect ReadTremoloBar(BinaryReader s)
        {
            int value = s.ReadInt32();

            //barEffect = gp.BendEffect()
            //barEffect.type = gp.BendType.dip
            //barEffect.value = self.readInt()
            //barEffect.points = [
            //    gp.BendPoint(0, 0),
            //    gp.BendPoint(round(gp.BendEffect.maxPosition / 2),
            //                 round(-barEffect.value / self.bendSemitone)),
            //    gp.BendPoint(gp.BendEffect.maxPosition, 0),
            //]
            //return barEffect
            NoteEffect.BendEffect.BendPoint[] points =
            {
                new NoteEffect.BendEffect.BendPoint(0, 0, false),
                new NoteEffect.BendEffect.BendPoint(NoteEffect.BendEffect.MAX_POSITION / 2,
                             -value / GP3File.BEND_SEMITONE, false),
                new NoteEffect.BendEffect.BendPoint(NoteEffect.BendEffect.MAX_POSITION, 0, false)
            };
            return new NoteEffect.BendEffect(NoteEffect.BendType.DIP, value, points);
        }

        // Unpack stroke value.
        // Stroke value maps to:
        //      - *1*: hundred twenty-eighth
        //      - *2*: sixty-fourth
        //      - *3*: thirty-second
        //      - *4*: sixteenth
        //      - *5*: eighth
        //      - *6*: quarter
        public static int ToStrokeValue(int value)
        {
            switch (value)
            {
                case 1:
                    return Duration.HUNDRED_TWENTY_EIGHTH;
                case 2:
                    return Duration.SIXTY_FOURTH;
                case 3:
                    return Duration.THIRTY_SECOND;
                case 4:
                    return Duration.SIXTEENTH;
                case 5:
                    return Duration.EIGHTH;
                case 6:
                    return Duration.QUATER;
                default:
                    return Duration.SIXTY_FOURTH;
            }
        }

        // Read beat stroke.
        // Beat stroke consists of two :ref:`Bytes<byte>` which correspond
        // to stroke up and stroke down speed.See
        // :class:`guitarpro.models.BeatStrokeDirection` for value mapping.
        public static int ReadBeatStroke(BinaryReader s)
        {

            // strokeDown = self.readSignedByte()
            // strokeUp = self.readSignedByte()
            // if strokeUp > 0:
            //     return gp.BeatStroke(gp.BeatStrokeDirection.up, self.toStrokeValue(strokeUp))
            // elif strokeDown > 0:
            //     return gp.BeatStroke(gp.BeatStrokeDirection.down, self.toStrokeValue(strokeDown))
            sbyte strokeUp = s.ReadSByte();
            sbyte strokeDown = s.ReadSByte();
            //TODO: pack in struct? / enum?
            if (strokeUp > 0)
            {
                return ToStrokeValue(strokeUp);
            }
            else if (strokeDown > 0)
            {
                return -ToStrokeValue(strokeDown);
            }
            return 0;
        }
        // Read mix table change.
        // List of values is read first.See
        // :meth:`readMixTableChangeValues`.
        // List of values is followed by the list of durations for
        // parameters that have changed. See
        // :meth:`readMixTableChangeDurations`.
        public static MixTableChange ReadMixTableChange(BinaryReader s, Measure measure)
        {
            MixTableChange tableChange = ReadMixTableChangeValues(s);
            ReadMixTableChangeDurations(s, tableChange);
            return tableChange;
        }

        // Read mix table change values.
        // Mix table change values consist of 7 :ref:`SignedBytes
        // <signed-byte>` and an :ref:`int`, which correspond to:
        //      - instrument
        //      - volume
        //      - balance
        //      - chorus
        //      - reverb
        //      - phaser
        //      - tremolo
        //      - tempo
        // If signed byte is *-1* then corresponding parameter hasn't
        // changed.
        public static MixTableChange ReadMixTableChangeValues(BinaryReader s)
        {
            sbyte instrument = s.ReadSByte();
            sbyte volume = s.ReadSByte();
            sbyte balance = s.ReadSByte();
            sbyte chorus = s.ReadSByte();
            sbyte reverb = s.ReadSByte();
            sbyte phaser = s.ReadSByte();
            sbyte tremolo = s.ReadSByte();
            int tempo = s.ReadInt32();

            return new MixTableChange(instrument, volume, balance, chorus, reverb, phaser, tremolo, tempo);
            /*
            if (instrument >= 0)
            {
                tableChange.instrument = gp.MixTableItem(instrument);
            }
            if (volume >= 0)
            {
                tableChange.volume = gp.MixTableItem(volume);
            }
            if (balance >= 0)
            {
                tableChange.balance = gp.MixTableItem(balance);
            }
            if (chorus >= 0)
            {
                tableChange.chorus = gp.MixTableItem(chorus);
            }
            if (reverb >= 0)
            {
                tableChange.reverb = gp.MixTableItem(reverb);
            }
            if (phaser >= 0)
            {
                tableChange.phaser = gp.MixTableItem(phaser);
            }
            if (tremolo >= 0)
            {
                tableChange.tremolo = gp.MixTableItem(tremolo);
            }
            if (tempo >= 0)
            {
                tableChange.tempo = gp.MixTableItem(tempo);
            }
            */

            //return 0;
        }
        public static void ReadMixTableChangeDurations(BinaryReader s, MixTableChange tableChange)
        {
            if (tableChange.Volume >= 0)
            {
                tableChange.VolumeDuration = s.ReadSByte();
            }
            if (tableChange.Balance >= 0)
            {
                tableChange.BalanceDuration = s.ReadSByte();
            }
            if (tableChange.Chorus >= 0)
            {
                tableChange.ChorusDuration = s.ReadSByte();
            }
            if (tableChange.Reverb >= 0)
            {
                tableChange.ReverbDuration = s.ReadSByte();
            }
            if (tableChange.Phaser >= 0)
            {
                tableChange.PhaserDuration = s.ReadSByte();
            }
            if (tableChange.Tremolo >= 0)
            {
                tableChange.TremoloDuration = s.ReadSByte();
            }
            if (tableChange.Tempo >= 0)
            {
                tableChange.TempoDuration = s.ReadSByte();
                tableChange.HideTempo = false;
            }
        }

        private static Beat.SlapEffect ReadSlapEffect(BinaryReader s)
        {
            byte flags = s.ReadByte();
            return (Beat.SlapEffect)(flags);
        }

        // Read notes.
        // First byte lists played strings:
        //      - *0x01*: 7th string
        //      - *0x02*: 6th string
        //      - *0x04*: 5th string
        //      - *0x08*: 4th string
        //      - *0x10*: 3th string
        //      - *0x20*: 2th string
        //      - *0x40*: 1th string
        //      - *0x80*: *blank*
        private static void ReadNotes(BinaryReader s, Track track, Beat beat, NoteEffect noteEffect)
        {
            byte stringFlags = s.ReadByte();
            foreach (Track.GuitarString? str in track.Strings)
            {
                if (str == null) throw new Exception("GuitarString is null!");
                if ((stringFlags & 1 << (int)(7 - str.Value.Number)) != 0)
                {
                    //Note note = new Note(beat, noteEffect);
                    //beat.Notes.Add(note);
                    //ReadNote(s, note, str, track);
                    Note note = ReadNote(s, str.Value, track);
                    beat.Notes.Add(note);
                }
                //TODO: test this?!
                //beat.Duration = duration; // ????
            }
        }

        // Read note.
        // The first byte is note flags:
        //      - *0x01*: time-independent duration
        //      - *0x02*: heavy accentuated note
        //      - *0x04*: ghost note
        //      - *0x08*: presence of note effects
        //      - *0x10*: dynamics
        //      - *0x20*: fret
        //      - *0x40*: accentuated note
        //      - *0x80*: right hand or left hand fingering
        // Flags are followed by:
        //      - Note type: :ref:`byte`. Note is normal if values is 1, tied if
        //          value is 2, dead if value is 3.
        //      - Time-independent duration: 2 :ref:`SignedBytes<signed-byte>`.
        //          Correspond to duration and tuplet.See :meth:`readDuration`
        //          for reference.
        //      - Note dynamics: :ref:`signed-byte`. See :meth:`unpackVelocity`.
        //      - Fret number: :ref:`signed-byte`. If flag at*0x20* is set then
        //          read fret number.
        //      - Fingering: 2 :ref:`SignedBytes<signed-byte>`. See
        //          :class:`guitarpro.models.Fingering`.
        //      - Note effects.See :meth:`readNoteEffects`.
        private static Note ReadNote(BinaryReader s, Track.GuitarString str, Track track)
        {
            Note note = new Note();

            byte flags = s.ReadByte();
            note.String = str.Number;
            note.Effect.GhostNote = (flags & 0x04) != 0;
            if ((flags & 0x20) != 0)
            {
                byte noteType = s.ReadByte();
                note.Type = (Note.NoteType)noteType;
            }
            if ((flags & 0x01) != 0)
            {
                note.Duration = s.ReadSByte();
                note.Tuplet = s.ReadSByte();
            }
            if ((flags & 0x10) != 0)
            {
                sbyte dyn = s.ReadSByte();
                note.Velocity = UnpackVelocity(dyn);
            }
            if ((flags & 0x20) != 0)
            {
                sbyte fret = s.ReadSByte();
                int value;
                if (note.Type == Note.NoteType.TIE)
                {
                    value = GetTiedNoteValue(str.Number, track);
                }
                else
                {
                    value = fret;
                }
                note.Value = (uint)Math.Max(0, Math.Min(99, value));
            }
            if ((flags & 0x80) != 0)
            {
                sbyte lHandFinger = s.ReadSByte();
                sbyte rHandFinger = s.ReadSByte();
                note.Effect.LeftHandFinger = (Note.Fingering)lHandFinger;
                note.Effect.RightHandFinger = (Note.Fingering)rHandFinger;
            }
            if ((flags & 0x08) != 0)
            {
                note.Effect = ReadNoteEffects(s, note);
                //if (note.Effect.isHarmonic && isinstance(note.Effect.Harmonic, gp.TappedHarmonic))
                //{
                //    note.Effect.Harmonic.Fret = note.Value + 12;
                //}
                if (note.Effect.Harmonic != null && note.Effect.Harmonic.Type == NoteEffect.HarmonicEffectType.TAPPED)
                {
                    note.Effect.Harmonic.Fret = note.Value + 12;
                }
            }
            return note;
        }

        //Convert Guitar Pro dynamic value to raw MIDI velocity.
        private static int UnpackVelocity(int dyn)
        {
            //return (gp.Velocities.minVelocity +
            //    gp.Velocities.velocityIncrement * dyn -
            //    gp.Velocities.velocityIncrement)
            //return dyn;

            return (Velocities.MIN_VELOCITY +
                Velocities.VELOCITY_INCREMENT * dyn -
                Velocities.VELOCITY_INCREMENT);
        }

        //Get note value of tied note.
        private static int GetTiedNoteValue(uint stringNumber, Track track)
        {
            //for measure in reversed(track.measures):
            //  for voice in reversed(measure.voices):
            //      for beat in voice.beats:
            //          if beat.status != gp.BeatStatus.empty:
            //              for note in beat.notes:
            //                  if note.string == stringIndex:
            //                      return note.value
            for (int i = track.Measures.Count - 1; i <= 0; i--)
            {
                Measure measure = track.Measures[i];
                for (int j = measure.Voices.Count - 1; j <= 0; j--)
                {
                    Voice voice = measure.Voices[j];
                    for (int k = 0; k < voice.Beats.Count; k++)
                    {
                        Beat beat = voice.Beats[k];
                        if (beat.Status == Beat.BeatStatus.EMPTY) continue;
                        for (int l = 0; l < beat.Notes.Count; l++)
                        {
                            Note note = beat.Notes[l];
                            if (note.String == stringNumber)
                                return (int)note.Value;
                        }
                    }
                }
            }
            return -1;
        }

        //Read note effects.
        // First byte is note effects flags:
        //      - *0x01*: bend presence
        //      - *0x02*: hammer-on/pull-off
        //      - *0x04*: slide
        //      - *0x08*: let-ring
        //      - *0x10*: grace note presence
        // Flags are followed by:
        //      - Bend.See :meth:`readBend`.
        //      - Grace note.See :meth:`readGrace`.
        private static NoteEffect ReadNoteEffects(BinaryReader s, Note note)
        {
            if (note.Effect == null) note.Effect = new NoteEffect();
            NoteEffect noteEffect = note.Effect;

            byte flags = s.ReadByte();
            noteEffect.Hammer = (flags & 0x02) != 0;
            noteEffect.LetRing = (flags & 0x08) != 0;
            if ((flags & 0x01) != 0)
            {
                noteEffect.Bend = ReadBendEffect(s);
            }
            if ((flags & 0x10) != 0)
            {
                noteEffect.Grace = ReadGraceEffect(s);
            }
            if ((flags & 0x04) != 0)
            {
                noteEffect.Slides = ReadSlideEffect(s);
            }
            return noteEffect;
        }
        // Read bend.
        // Encoded as:
        //      - Bend type: :ref:`signed-byte`. See
        //        :class:`guitarpro.models.BendType`.
        //      - Bend value: :ref:`int`.
        //      - Number of bend points: :ref:`int`.
        //      - List of points.Each point consists of:
        //          * Position: :ref:`int`. Shows where point is set along
        //                *x*-axis.
        //          * Value: :ref:`int`. Shows where point is set along *y*-axis.
        //          * Vibrato: :ref:`bool`.
        private static NoteEffect.BendEffect ReadBendEffect(BinaryReader s)
        {
            sbyte bendTypeRaw = s.ReadSByte();
            NoteEffect.BendType bendType = (NoteEffect.BendType)bendTypeRaw;

            int value = s.ReadInt32();
            int pointCount = s.ReadInt32();
            NoteEffect.BendEffect.BendPoint[] bends = new NoteEffect.BendEffect.BendPoint[pointCount];
            for (int i = 0; i < pointCount; i++)
            {
                int positionRaw = s.ReadInt32();
                int pointValueRaw = s.ReadInt32();
                bool vibrato = s.ReadBoolean();

                int position = NoteEffect.BendEffect.MAX_POSITION / GP3File.BEND_POSITION;
                int pointValue = pointValueRaw * NoteEffect.BendEffect.SEMITONE_LENGTH / GP3File.BEND_SEMITONE;
                bends[i] = new NoteEffect.BendEffect.BendPoint(positionRaw, pointValueRaw, vibrato);
            }

            return new NoteEffect.BendEffect(bendType, value, bends);
        }

        // Read grace note effect.
        //      - Fret: :ref:`signed-byte`. Number of fret.
        //      - Dynamic: :ref:`byte`. Dynamic of a grace note, as in
        //          :attr:`guitarpro.models.Note.velocity`.
        //      - Transition: :ref:`byte`. See
        //          :class:`guitarpro.models.GraceEffectTransition`.
        //      - Duration: :ref:`byte`. Values are:
        //          - *1*: Thirty-second note.
        //          - *2*: Twenty-fourth note.
        //          - *3*: Sixteenth note.
        private static NoteEffect.GraceEffect ReadGraceEffect(BinaryReader s)
        {
            sbyte fret = s.ReadSByte();
            byte velocityRaw = s.ReadByte();
            byte durationRaw = s.ReadByte();
            sbyte transitionRaw = s.ReadSByte();

            NoteEffect.GraceEffectTransition transition = (NoteEffect.GraceEffectTransition)transitionRaw;

            return new NoteEffect.GraceEffect(fret, UnpackVelocity(velocityRaw), 1 << (7 - durationRaw), transition);
        }

        private static NoteEffect.SlideType[] ReadSlideEffect(BinaryReader s)
        {
            //def readSlides(self):
            //    return [gp.SlideType.shiftSlideTo]
            return new NoteEffect.SlideType[] { NoteEffect.SlideType.SHIFT_SLIDE_TO };
        }
    }
}
