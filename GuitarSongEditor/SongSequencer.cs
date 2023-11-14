using GuitarProSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GuitarSongEditor
{
    public partial class SongSequencer : Form
    {
        private Song? loadedSong = null;
        public SongSequencer()
        {
            InitializeComponent();
        }


        public void LoadSong(Song s)
        {
            loadedSong = s;

            if (s.Tracks != null)
            {
                trackListBox1.Items.AddRange(s.Tracks);
                trackListBox1.SelectedIndex = 0;
            }

            Console.WriteLine($"Sequencer Loaded Song: {s.Metadata.title}");
        }

        private void trackListBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            Track selected = (Track)trackListBox1.SelectedItem;
            sequencerControl1.Track = selected;
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            sequencerControl1.TrackX = hScrollBar1.Value*3f;
        }
    }
}
