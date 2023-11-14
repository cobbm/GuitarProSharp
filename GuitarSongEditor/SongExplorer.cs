using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using NAudio;
using NAudio.Midi;

using GuitarProSharp;

namespace GuitarSongEditor
{
    public partial class SongExplorer : Form
    {
        private static readonly int INSPECTOR_CONTROL_MARGIN = 8;

        private List<(int, MidiOutCapabilities)> MidiDevices;
        private (int,MidiOutCapabilities)? selectedMidiOutDevice = null;

        private string loadedFile;
        private Song? loadedSong = null;
        public SongExplorer(string fileName)
        {
            InitializeComponent();
            MidiDevices = EnumerateMidiOutDevices();
            AddMidiOutMenuItems();
            LoadSongFromFile(fileName);
        }

        private void LoadSongFromFile(string fileName)
        {
            Song? s = GP3File.LoadFromFile(fileName);
            if (s == null)
            {
                MessageBox.Show($"Failed to load song: {fileName}");
                return;
            }
            LoadSong(s);
            this.loadedFile = fileName;   
        }

        private void SongViewer_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Loaded SongViewer form");
        }

        private void AddMidiOutMenuItems()
        {
            mIDIOutputDeviceToolStripMenuItem.DropDownItems.Clear();


            if(MidiDevices.Count > 0)
            {
                bool first = true;
                foreach((int,MidiOutCapabilities) cap in MidiDevices)
                {
                    ToolStripMenuItem v = new ToolStripMenuItem(cap.Item2.ProductName);
                    //v.AutoSize = true;
                    //v.CheckOnClick = true;
                    v.Tag = cap;
                    v.Click += MidiOutputDeviceMenuItem_Click;
                    if (first)
                    {
                        v.Checked = true;
                        selectedMidiOutDevice = cap;
                        first = false;
                    }
                    mIDIOutputDeviceToolStripMenuItem.DropDownItems.Add(v);
                }
                //UpdateMidiOutSelection(MidiDevices[0]);
            }
        }

        private void MidiOutputDeviceMenuItem_Click(object sender, EventArgs args)
        {
            //ToolStri
            ToolStripMenuItem buttonSender = (ToolStripMenuItem)sender;

            foreach (ToolStripMenuItem i in mIDIOutputDeviceToolStripMenuItem.DropDownItems)
            {
                if(i != buttonSender)
                {
                    i.Checked = false;
                } else
                {
                    i.Checked = true;
                }
            }
            
            if(buttonSender.Tag != null)
            {
                selectedMidiOutDevice = ((int,MidiOutCapabilities))buttonSender.Tag;
                Console.WriteLine($"Selected MIDI Output Device: {selectedMidiOutDevice.Value.Item2.ProductName}");
            }
        }

        public void LoadSong(Song song)
        {
            treeView1.SuspendLayout();
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(GenerateSongTree(song));
            treeView1.ResumeLayout();
            this.loadedSong = song;
        }

        private void SetInspectorControlProperties(Control c)
        {
            c.Dock = DockStyle.Top;
            c.BringToFront();
            Padding p = c.Padding;
            p.Top = INSPECTOR_CONTROL_MARGIN;
            c.Padding= p;
        }

        private TreeNode GenerateSongTree(Song s)
        {
            TreeNode root = new TreeNode($"Song: {s.Metadata.title}");
            root.Tag = s;

            TreeNode metadataNode = new TreeNode("Metadata");
            metadataNode.Tag = s.Metadata;

            root.Nodes.Add(metadataNode);
            if (s.MeasureHeaders != null)
                root.Nodes.Add(GenerateMeasureHeadersTree(s.MeasureHeaders));
            if (s.Tracks != null)
                root.Nodes.Add(GenerateTracksTree(s.Tracks));

            return root;
        }

        private TreeNode GenerateTracksTree(Track[] tracks)
        {
            TreeNode root = new TreeNode($"Tracks [{tracks.Length}]");
            root.Tag = tracks;
            root.Nodes.AddRange(tracks.Select(GenerateTrackTree).ToArray());
            return root;
        }

        private TreeNode GenerateTrackTree(Track track)
        {
            TreeNode root = new TreeNode($"Track {track.Number}: {track.Name}");
            root.Tag = track;

            TreeNode midiChannelNode = new TreeNode("MidiChannel");
            midiChannelNode.Tag = track.MidiChannel;
            root.Nodes.Add(midiChannelNode);

            TreeNode measuresNode = new TreeNode($"Measures [{track.Measures.Count}]");
            measuresNode.Tag = track.Measures;
            root.Nodes.Add(measuresNode);
            for (int i = 0; i < track.Measures.Count; i++)
            {
                measuresNode.Nodes.Add(GenerateTrackMeasureTree(track.Measures[i]));
            }

            return root;
        }

        private TreeNode GenerateTrackMeasureTree(Measure measure)
        {
            string name = $"Measure {measure.Header.Number}";
            if (measure.Header.Marker != null)
            {
                name += $": {measure.Header.Marker?.Title}";
            }
            TreeNode root = new TreeNode(name);
            root.Tag = measure;

            string headerText = $"MeasureHeader";
            TreeNode header = new TreeNode(headerText);
            header.Tag = measure.Header;
            root.Nodes.Add(header);

            //TreeNode voiceNode = new TreeNode($"Voices [{measure.Voices.Count}]");
            for (var i = 0; i < measure.Voices.Count; i++)
            {
                TreeNode v = GenerateVoiceTree(measure.Voices[i], i);
                root.Nodes.Add(v);
            }
            return root;
        }

        private TreeNode GenerateVoiceTree(Voice voice, int index)
        {
            TreeNode root = new TreeNode($"Voice {index}");
            root.Tag = voice;

            for (int i = 0; i < voice.Beats.Count; i++)
            {
                Beat b = voice.Beats[i];
                TreeNode n = GenerateBeatTree(b, i);
                root.Nodes.Add(n);
            }
            
            return root;
        }

        private TreeNode GenerateBeatTree(Beat b, int index)
        {
            TreeNode root = new TreeNode($"Beat {index}");
            root.Tag = b;

            if (b.Effect.MixTableChange != null)
            {
                TreeNode n = new TreeNode("Mix Table Change");
                n.Tag = b.Effect.MixTableChange;
                root.Nodes.Add(n);
            }

            //root.Nodes.Add(GenerateNotesTree(b.Notes));
            for (int i = 0; i < b.Notes.Count; i++)
            {
                Note n = b.Notes[i];
                TreeNode node = new TreeNode($"Notes[{i + 1}] (String {n.String})");
                node.Tag = n;
                root.Nodes.Add(node);
            }

            return root;
        }

        private TreeNode GenerateNotesTree(List<Note> notes)
        {
            TreeNode root = new TreeNode($"Notes [{notes.Count}]");
            root.Tag = notes;
            for (int i = 0; i < notes.Count; i++)
            {
                Note n = notes[i];
                TreeNode node = new TreeNode($"Note {i}");
                node.Tag = n;
                root.Nodes.Add(node);
            }
            return root;
        }

        private TreeNode GenerateMeasureHeadersTree(Song.MeasureHeader[] hs)
        {
            TreeNode node = new TreeNode($"MeasureHeaders [{hs.Length}]");
            node.Tag = hs;

            TreeNode[] children = new TreeNode[hs.Length];
            for (int i = 0; i < hs.Length; i++)
            {
                Song.MeasureHeader header = hs[i];
                TreeNode headerNode = new TreeNode();

                string name = $"MeasureHeader {header.Number}";
                if (header.Marker != null)
                {
                    name += $": ({header.Marker.Value.Title})";
                    headerNode.Nodes.Add(GenerateMarkerTree(header.Marker.Value));
                }
                headerNode.Text = name;
                headerNode.Tag = header;
                children[i] = headerNode;
            }
            node.Nodes.AddRange(children);
            return node;
        }

        private TreeNode GenerateMarkerTree(Song.Marker marker)
        {
            TreeNode root = new TreeNode("Marker");
            root.Tag = marker;
            return root;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
            {
                Console.WriteLine("AfterSelect: Node was null!");
                return;
            }
            OnTreeNodeSelected(e.Node);
        }
        private void OnTreeNodeSelected(TreeNode n)
        {
            inspectorPanel.SuspendLayout();
            inspectorPanel.Controls.Clear();

            if (n.Tag == null)
            {
                Console.WriteLine("TreeNode Tag is null!");
            }
            else
            {
                List<Control> controls = new List<Control>();
                switch (n.Tag)
                {
                    case Song s:
                        ShowObjectInspector(controls, s);
                        break;
                    case Song.MeasureHeader h:
                        ShowObjectInspector(controls, h);
                        break;
                    case Song.Marker m:
                        ShowObjectInspector(controls, m);
                        break;
                    case Song.SongMetadata m:
                        ShowObjectInspector(controls, m);
                        break;
                    case Track t:
                        ShowObjectInspector(controls, t);
                        break;
                    case Measure m:
                        ShowObjectInspector(controls, m);
                        break;
                    case MidiChannel c:
                        ShowObjectInspector(controls, c);
                        break;
                    case Voice v:
                        ShowObjectInspector(controls, v);
                        break;
                    case Beat b:
                        ShowObjectInspector(controls, b);
                        break;
                    case MixTableChange m:
                        ShowObjectInspector(controls, m);
                        break;
                    case Note note:
                        ShowObjectInspector(controls, note);
                        break;
                    default:
                        Console.WriteLine($"Invalid type: '{n.Tag.GetType().Name}'!");
                        break;
                }
                foreach (Control c in controls)
                {
                    inspectorPanel.Controls.Add(c);
                    SetInspectorControlProperties(c);
                }
            }
            inspectorPanel.ResumeLayout();
        }

        private void ShowObjectInspector(List<Control> controls, Song s)
        {
            controls.AddRange(CreateInspectorElement("Version", s.Version ?? ""));
            controls.AddRange(CreateInspectorElement("Triplet Feel", s.TripletFeel.ToString()));
            controls.AddRange(CreateInspectorElement("Tempo", s.Tempo.ToString()));

            controls.AddRange(CreateInspectorElement("Key", s.Key.ToString()));
        }
        private void ShowObjectInspector(List<Control> controls, Song.SongMetadata s)
        {
            controls.AddRange(CreateInspectorElement("Title", s.title));
            controls.AddRange(CreateInspectorElement("Subtitle", s.subtitle));
            controls.AddRange(CreateInspectorElement("Artist", s.artist));
            controls.AddRange(CreateInspectorElement("Album", s.album));
            controls.AddRange(CreateInspectorElement("Words", s.words));
            controls.AddRange(CreateInspectorElement("Tabbed By", s.tabbedBy));
            controls.AddRange(CreateInspectorElement("Instructions", s.instructions));
            controls.AddRange(CreateInspectorElement("Copyright", s.copyright));
            controls.AddRange(CreateInspectorElement($"Notice [{s.notice.Length}]", s.notice));
        }

        private void ShowObjectInspector(List<Control> controls, Track t)
        {
            controls.AddRange(CreateInspectorElement("Name", t.Name));
            controls.AddRange(CreateInspectorElement("Number", t.Number.ToString()));
            controls.AddRange(CreateInspectorElement("Color", t.Color.ToString()));

            controls.AddRange(CreateInspectorElement("MidiPort", t.MidiPort.ToString()));
            //controls.AddRange(CreateInspectorElement("MidiChannel", t.MidiChannel.ToString()));

            controls.AddRange(CreateInspectorElement($"Strings [{t.Strings.Length}]", t.Strings));
            controls.AddRange(CreateInspectorElement("FretCount", t.FretCount.ToString()));
            controls.AddRange(CreateInspectorElement("Offset", t.Offset.ToString()));

            controls.AddRange(CreateInspectorElement("Flags", $"0x{t.Flags.ToString("X2")}"));

            controls.AddRange(CreateInspectorElement("indicateTuning", t.indicateTuning));
            controls.AddRange(CreateInspectorElement("isMute", t.isMute));
            controls.AddRange(CreateInspectorElement("isSolo", t.isSolo));
            controls.AddRange(CreateInspectorElement("isVisible", t.isVisible));
            controls.AddRange(CreateInspectorElement("is12StringedGuitarTrack", t.is12StringedGuitarTrack));
            controls.AddRange(CreateInspectorElement("isBanjoTrack", t.isBanjoTrack));
            controls.AddRange(CreateInspectorElement("isPercussionTrack", t.isPercussionTrack));
        }

        private void ShowObjectInspector(List<Control> controls, Measure m)
        {
            //controls.AddRange(CreateInspectorElement("Number", m.Number.ToString()));
            controls.AddRange(CreateInspectorElement("Start", m.Start.ToString()));

            controls.AddRange(CreateInspectorElement("Clef", m.Clef.ToString()));
        }

        private void ShowObjectInspector(List<Control> controls, MidiChannel channel)
        {
            controls.AddRange(CreateInspectorElement("Port", channel.port.ToString()));
            controls.AddRange(CreateInspectorElement("Channel", channel.channel.ToString()));

            controls.AddRange(CreateInspectorElement("Instrument", channel.Instrument.ToString()));

            controls.AddRange(CreateInspectorElement("IsPercussionChannel?", channel.isPercussionChannel));
            controls.AddRange(CreateInspectorElement("Has Effect Channel?", channel.hasEffectChannel));

            controls.AddRange(CreateInspectorElement("Effect Channel", channel.hasEffectChannel ? channel.EffectChannel.ToString() : "(no effect channel)"));

            controls.AddRange(CreateInspectorElement("Volume", channel.Volume.ToString()));
            controls.AddRange(CreateInspectorElement("Balance", channel.Balance.ToString()));
            controls.AddRange(CreateInspectorElement("Tremolo", channel.Tremolo.ToString()));
            controls.AddRange(CreateInspectorElement("Reverb", channel.Reverb.ToString()));
            controls.AddRange(CreateInspectorElement("Phaser", channel.Phaser.ToString()));
            controls.AddRange(CreateInspectorElement("Chorus", channel.Chorus.ToString()));
            controls.AddRange(CreateInspectorElement("Bank", channel.Bank.ToString()));
        }

        private void ShowObjectInspector(List<Control> controls, Song.MeasureHeader h)
        {
            controls.AddRange(CreateInspectorElement("Number", h.Number.ToString()));

            controls.AddRange(CreateInspectorElement("Start", h.Start.ToString()));
            controls.AddRange(CreateInspectorElement("End", h.GetEnd().ToString()));
            controls.AddRange(CreateInspectorElement("Length", h.GetLength().ToString()));

            controls.AddRange(CreateInspectorElement("Flags", $"0x{h.flags.ToString("X2")}"));

            controls.AddRange(CreateInspectorElement("isMarker", h.isMarker));
            controls.AddRange(CreateInspectorElement("isRepeatAlternative", h.isRepeatAlternative));
            controls.AddRange(CreateInspectorElement("isRepeatOpen", h.isRepeatOpen));
            controls.AddRange(CreateInspectorElement("isRepeatClose", h.isRepeatClose));
            controls.AddRange(CreateInspectorElement("hasTonality", h.hasTonality));
            controls.AddRange(CreateInspectorElement("hasDoubleBar", h.hasDoubleBar));
        }

        private void ShowObjectInspector(List<Control> controls, Song.Marker marker)
        {
            controls.AddRange(CreateInspectorElement("Title", marker.Title));
            controls.AddRange(CreateInspectorElement("Color", marker.Color.ToString()));
        }

        private void ShowObjectInspector(List<Control> controls, Voice voice)
        {
            controls.AddRange(CreateInspectorElement("Voice Direction", voice.VoiceDirection.ToString()));
        }

        private void ShowObjectInspector(List<Control> controls, Beat beat)
        {
            controls.AddRange(CreateInspectorElement("Text", beat.Text ?? ""));
            controls.AddRange(CreateInspectorElement("Start", beat.Start.ToString()));
            controls.AddRange(CreateInspectorElement("Duration", beat.Duration?.ToString() ?? "(null)"));
            controls.AddRange(CreateInspectorElement("Status", beat.Status.ToString()));

            controls.AddRange(CreateInspectorElement("Notes.Count", beat.Notes.Count.ToString()));

            controls.Add(CreateInspectorGroup("Beat Effect", CreateInspectorBeatEffect(beat.Effect)));

            //controls.Add(CreateInspectorGroup("Notes", CreateInspectorNotes(beat.Notes)));
        }

        private void ShowObjectInspector(List<Control> controls, MixTableChange mixTable)
        {
            controls.AddRange(CreateInspectorElement("Instrument", mixTable.Instrument.ToString()));
            controls.AddRange(CreateInspectorElement("Instrument Duration", mixTable.InstrumentDuration.ToString()));

            controls.AddRange(CreateInspectorElement("Volume", mixTable.Volume.ToString()));
            controls.AddRange(CreateInspectorElement("Volume Duration", mixTable.VolumeDuration.ToString()));

            controls.AddRange(CreateInspectorElement("Balance", mixTable.Balance.ToString()));
            controls.AddRange(CreateInspectorElement("Balance Duration", mixTable.BalanceDuration.ToString()));

            controls.AddRange(CreateInspectorElement("Chorus", mixTable.Chorus.ToString()));
            controls.AddRange(CreateInspectorElement("Chorus Duration", mixTable.ChorusDuration.ToString()));

            controls.AddRange(CreateInspectorElement("Reverb", mixTable.Reverb.ToString()));
            controls.AddRange(CreateInspectorElement("Reverb Duration", mixTable.ReverbDuration.ToString()));

            controls.AddRange(CreateInspectorElement("Phaser", mixTable.Phaser.ToString()));
            controls.AddRange(CreateInspectorElement("Phaser Duration", mixTable.PhaserDuration.ToString()));

            controls.AddRange(CreateInspectorElement("Tremolo", mixTable.Tremolo.ToString()));
            controls.AddRange(CreateInspectorElement("Tremolo Duration", mixTable.TremoloDuration.ToString()));

            controls.AddRange(CreateInspectorElement("Tempo", mixTable.Tempo.ToString()));
            controls.AddRange(CreateInspectorElement("Tempo Duration", mixTable.TempoDuration.ToString()));

            controls.AddRange(CreateInspectorElement("Hide Tempo", mixTable.HideTempo));
        }

        private void ShowObjectInspector(List<Control> controls, Note note)
        {
            controls.AddRange(CreateInspectorElement("Type", note.Type.ToString()));
            controls.AddRange(CreateInspectorElement("Value (TiedNoteValue or Fret)", note.Value.ToString()));
            controls.AddRange(CreateInspectorElement("Duration", note.Duration.ToString()));
            controls.AddRange(CreateInspectorElement("Tuplet", note.Tuplet.ToString()));
            //controls.AddRange(CreateInspectorElement("Duration Percent", note.DurationPercent.ToString()));
            controls.AddRange(CreateInspectorElement("String", note.String.ToString()));
            controls.AddRange(CreateInspectorElement("Velocity", note.Velocity.ToString()));
            controls.AddRange(CreateInspectorElement("Swap Accidentals", note.SwapAccidentals));

            controls.Add(CreateInspectorGroup("Note Effect", CreateInspectorNoteEffect(note.Effect)));
        }


        private List<Control> CreateInspectorBeatEffect(Beat.BeatEffect effect)
        {
            List<Control> controls = new List<Control>();

            controls.AddRange(CreateInspectorElement("Pick Stroke", effect.pickStroke.ToString()));
            controls.AddRange(CreateInspectorElement("Slap Effect", effect.SlapEffect.ToString()));
            controls.AddRange(CreateInspectorElement("Stroke", effect.Stroke.ToString()));

            controls.AddRange(CreateInspectorElement("Pick Stroke", effect.TremoloBar?.ToString() ?? "(null)"));
            controls.AddRange(CreateInspectorElement("Fade In", effect.FadeIn));
            controls.AddRange(CreateInspectorElement("Vibrato", effect.Vibrato));
            controls.AddRange(CreateInspectorElement("Has Resgueado", effect.hasResgueado));

            if (effect.Chord != null)
            {
                controls.Add(CreateInspectorGroup("Chord", CreateInspectorChord(effect.Chord)));
            }
            return controls;
        }

        private List<Control> CreateInspectorNoteEffect(NoteEffect effect)
        {
            List<Control> controls = new List<Control>();
            controls.AddRange(CreateInspectorElement("Left Hand Finger", effect.LeftHandFinger.ToString()));
            controls.AddRange(CreateInspectorElement("Right Hand Finger", effect.RightHandFinger.ToString()));

            controls.AddRange(CreateInspectorElement("Bend", effect.Bend?.ToString() ?? "(none)"));
            controls.AddRange(CreateInspectorElement("Harmonic", effect.Harmonic?.ToString() ?? "(none)"));
            controls.AddRange(CreateInspectorElement("Trill", effect.Trill?.ToString() ?? "(none)"));
            controls.AddRange(CreateInspectorElement("Grace", effect.Grace?.ToString() ?? "(none)"));
            controls.AddRange(CreateInspectorElement("Tremolo Picking", effect.TremoloPicking?.ToString() ?? "(none)"));

            controls.AddRange(CreateInspectorElement("Slides", effect.Slides != null ? String.Join(", ", effect.Slides) : "(none)"));

            controls.AddRange(CreateInspectorElement("Ghost Note", effect.GhostNote));
            controls.AddRange(CreateInspectorElement("Palm Mute", effect.PalmMute));
            controls.AddRange(CreateInspectorElement("Hammer", effect.Hammer));
            controls.AddRange(CreateInspectorElement("Accentuated Note", effect.AccentuatedNote));
            controls.AddRange(CreateInspectorElement("Heavy Accentuated Note", effect.HeavyAccentuatedNote));
            controls.AddRange(CreateInspectorElement("Let Ring", effect.LetRing));
            controls.AddRange(CreateInspectorElement("Staccato", effect.Staccato));
            controls.AddRange(CreateInspectorElement("Vibrato", effect.Vibrato));

            return controls;
        }

        private List<Control> CreateInspectorChord(Chord chord)
        {
            List<Control> controls = new List<Control>();

            controls.AddRange(CreateInspectorElement("Name", chord.Name));
            controls.AddRange(CreateInspectorElement("Chord", chord.Type.ToString()));
            controls.AddRange(CreateInspectorElement("Root", chord.Root.ToString()));
            controls.AddRange(CreateInspectorElement("First Fret", chord.FirstFret.ToString()));
            controls.AddRange(CreateInspectorElement("Strings", String.Join(", ", chord.Strings)));

            controls.AddRange(CreateInspectorElement("Fifth", chord.Fifth.ToString()));
            controls.AddRange(CreateInspectorElement("Ninth", chord.Ninth.ToString()));
            controls.AddRange(CreateInspectorElement("Eleventh", chord.Eleventh.ToString()));

            controls.AddRange(CreateInspectorElement("Extension", chord.Extension.ToString()));

            controls.AddRange(CreateInspectorElement("Bass", chord.Bass.ToString()));
            controls.AddRange(CreateInspectorElement("Add", chord.Add));
            controls.AddRange(CreateInspectorElement("Tonality", chord.Tonality.ToString()));

            return controls;
        }

        /*
        private List<Control> CreateInspectorNotes(List<Note> notes)
        {
            List<Control> controls = new List<Control>();

            for (int i = 0; i < notes.Count; i++) {
                Note n = notes[i];
                List<Control> noteControls = new List<Control>();
                noteControls.AddRange(CreateInspectorElement("Type", n.Type.ToString()));
                noteControls.AddRange(CreateInspectorElement("Value (TiedNoteValue or Fret)", n.Value.ToString()));
                noteControls.AddRange(CreateInspectorElement("Duration", n.Duration.ToString()));
                noteControls.AddRange(CreateInspectorElement("Tuplet", n.Tuplet.ToString()));
                //noteControls.AddRange(CreateInspectorElement("Duration Percent", n.DurationPercent.ToString()));
                noteControls.AddRange(CreateInspectorElement("String", n.String.ToString()));
                noteControls.AddRange(CreateInspectorElement("Velocity", n.Velocity.ToString()));
                noteControls.AddRange(CreateInspectorElement("Swap Accidentals", n.SwapAccidentals));

                noteControls.AddRange(CreateInspectorElement("Effect", n.Effect.ToString() ?? "(null)"));

                controls.Add(CreateInspectorGroup($"Note {i}", noteControls));
            }
            return controls;
        }
        */

        private GroupBox CreateInspectorGroup(string name, List<Control> children)
        {
            GroupBox groupBox = new GroupBox();
            groupBox.Padding = new Padding(8);
            groupBox.AutoSize = true;
            groupBox.Text = name;
            foreach (Control child in children)
            {
                groupBox.Controls.Add(child);
                SetInspectorControlProperties(child);
            }
            return groupBox;
        }

        private Label CreateInspectorLabel(string label)
        {
            Label l = new Label();
            l.AutoSize = true;
            l.Text = label;
            return l;
        }

        private TextBox CreateInspectorTextbox(string value, bool multiline = false)
        {
            TextBox v = new TextBox();
            Color clr = v.BackColor;
            v.ReadOnly = true;
            v.BackColor = clr;
            if (multiline)
            {
                int singleLineHeight = v.Height;
                v.Multiline = true;
                v.Height = singleLineHeight * 6;
                v.ScrollBars = ScrollBars.Vertical;
            }
            v.Text = value;
            return v;
        }

        private Control[] CreateInspectorElement(string label, bool value)
        {
            CheckBox v = new CheckBox();
            v.Checked = value;
            v.Text = label;
            Color clr = v.ForeColor;
            //v.Enabled = false;
            v.AutoCheck = false;
            v.ForeColor = clr;
            return new Control[] {
                v
            };
        }

        private Control[] CreateInspectorElement(string label, string value)
        {
            return new Control[] { CreateInspectorLabel(label), CreateInspectorTextbox(value) };
        }
        private Control[] CreateInspectorElement(string label, string[] value)
        {
            return new Control[] { CreateInspectorLabel(label), CreateInspectorTextbox(string.Join(Environment.NewLine, value), true) };
        }

        private Control[] CreateInspectorElement(string label, Track.GuitarString?[] value)
        {
            string s = string.Join(Environment.NewLine, value);
            return new Control[] { CreateInspectorLabel(label), CreateInspectorTextbox(s, true) };
        }

        private void generateMIDIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedMidiOutDevice == null)
            {
                MessageBox.Show("No MIDI output device selected!");
                return;
            }
            if (!selectedMidiOutDevice.Value.Item2.SupportsMidiStreamOut)
            {
                Console.WriteLine("Warning: Selected MIDI device does not support streaming!");
            }
            Console.WriteLine("Generating MIDI file...");
            MidiOut mOut = new MidiOut(selectedMidiOutDevice.Value.Item1);

            //MidiEventCollection


            var collection = new MidiEventCollection(1, 120);
            var track = collection.AddTrack();
            collection.AddEvent(new NoteOnEvent(0, 1, 64, 127, 15),1);
            collection.AddEvent(new NoteOnEvent(15, 1, 65, 127, 15),1);
            collection.AddEvent(new NoteOnEvent(30, 1, 66, 127, 15),1);
            collection.AddEvent(new NoteOnEvent(45, 1, 67, 127, 15),1);
            collection.AddEvent(new NoteOnEvent(60, 1, 68, 127, 15), 1);

            collection.PrepareForExport();

            foreach(MidiEvent midiEvent in collection.GetTrackEvents(0))
            {
                MidiMessage m = new MidiMessage(midiEvent.GetAsShortMessage());
                
                mOut.Send(midiEvent.GetAsShortMessage());
            }
Thread.Sleep(2000);


            Console.WriteLine("Done!");
            mOut.Close();
            mOut.Dispose();
        }

        private List<(int,MidiOutCapabilities)> EnumerateMidiOutDevices()
        {
            List<(int,MidiOutCapabilities)> list = new List<(int,MidiOutCapabilities)>();
            Console.WriteLine("Enumerating MIDI Output devices...");
            Console.WriteLine("=====================");
            for (int device = 0; device < MidiOut.NumberOfDevices; device++)
            {
                MidiOutCapabilities caps = MidiOut.DeviceInfo(device);
                Console.WriteLine($"[{device}] {caps.ProductName}");
                list.Add((device,caps));
            }
            Console.WriteLine("=====================");
            return list;
        }

        private void showSongSequencerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SongSequencer sequencer = new SongSequencer();
            sequencer.Show();
            sequencer.LoadSong(this.loadedSong);
        }

        /*
        private void ShowObjectInspectorReflection(object o)
        {
            Type t = o.GetType();

            propNameLabel.Text = t.Name;

            inspectorPanel.SuspendLayout();
            inspectorPanel.Controls.Clear();

            AddInspectorLabel(inspectorPanel, $"Value (ToString()): {o.ToString()}");

            System.Reflection.PropertyInfo[] props = t.GetProperties();

            foreach (System.Reflection.PropertyInfo prop in props)
            {
                //Console.WriteLine($"\tProperty: {prop.Name}");

                string text = $"(Property) {prop.Name}: {prop.PropertyType.Name}";

                System.Reflection.ParameterInfo[] indices = prop.GetIndexParameters();
                if (indices.Length == 0)
                {
                    if (prop.PropertyType.IsArray)
                    {
                        Array array = prop.GetValue(o) as Array;
                        text += $" Length={array.Length}: {FlattenArray(array)}";
                    }
                    else
                    {
                        text += $": {prop.GetValue(o)}";
                    }
                    //text +=$": {prop.GetValue(o, null)}";
                }
                AddInspectorLabel(inspectorPanel, text);
            }

            System.Reflection.FieldInfo[] fields = t.GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                if (field.IsStatic) continue;

                //Console.WriteLine($"\tField: {field.Name}");

                string text = $"(Field) {field.Name}: {field.FieldType.Name}";

                if (field.FieldType.IsArray)
                {
                    Array array = field.GetValue(o) as Array;
                    text += $" Length={array.Length}: {FlattenArray(array)}";
                }
                else
                {
                    text += $": {field.GetValue(o)}";
                }

                AddInspectorLabel(inspectorPanel, text);

            }
            inspectorPanel.ResumeLayout();
            //inspectorPanel.Invalidate();
        }

        private void AddInspectorLabel(Panel p, string text)
        {
            Label l = new Label();
            l.AutoSize = true;
            //l.BorderStyle = BorderStyle.Fixed3D;
            //flowLayoutPanel1.SetFlowBreak(l, true);
            l.Text = text;
            p.Controls.Add(l);
        }

        private static string FlattenArray(Array a)
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            sb.Append("[");
            foreach (object p in a)
            {
                if (!first)
                {
                    sb.Append(", ");
                }
                sb.Append(p);
                first = false;
            }
            sb.Append("]");
            return sb.ToString();
        }
        */
    }

}