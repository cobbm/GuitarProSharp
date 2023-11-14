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
    public partial class TrackView : UserControl
    {
        private Track track;
        public TrackView(Track track)
        {
            this.track = track;

            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            trackIndexLabel.Text = this.track.Number.ToString();
            instrumentLabel.Text = this.track.Name;
            trackColorPanel.BackColor = this.track.Color;
            //etc
        }
    }
}
