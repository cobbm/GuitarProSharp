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
    public partial class TrackViewContainer : UserControl
    {
        public TrackViewContainer()
        {
            InitializeComponent();
        }
        private Track[]? _tracks;
        public Track[]? Tracks { 
            get {
                return _tracks; 
            }
            set
            {
                _tracks = value;
                Initialize();
            }
        }

        private void Initialize()
        {
            panel4.SuspendLayout();
            panel4.Controls.Clear();
            foreach(Track t in _tracks)
            {
                TrackView tv = new TrackView(t);
                panel4.Controls.Add(tv);
                tv.Dock = DockStyle.Top;
                tv.BringToFront();
            }
            panel4.ResumeLayout();
        }
    }
}
