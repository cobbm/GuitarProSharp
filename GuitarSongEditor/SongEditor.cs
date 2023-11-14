using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using GuitarProSharp;

namespace GuitarSongEditor
{
    public partial class SongEditor : Form
    {
        const string fileName = "D:\\Users\\Michael\\Downloads\\Muse - Plug In Baby.gp3";

        public SongEditor()
        {
            InitializeComponent();
        }

        private void LoadSong(Song song)
        {
            if (song.Tracks != null)
                trackViewContainer1.Tracks = song.Tracks;   
        }

        private void SongEditor_Load(object sender, EventArgs e)
        {
        }
    }
}
