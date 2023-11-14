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
    public partial class SequencerControl : UserControl
    {
        private Font font;

        private Track? _track = null;
        public Track Track
        {
            get
            {
                return _track;
            }
            set
            {
                _track = value;
                Invalidate();
            }
        }

        private float _trackX = 0;
        public float TrackX
        {
            get
            {
                return _trackX;
            }
            set
            {
                _trackX = value;
                Invalidate();
            }
        }

        private readonly float BEAT_WIDTH = 40;
        private readonly float NOTE_HEIGHT = 15;
        private readonly float RENDER_OFFSET_Y = 18;

        private readonly Brush NOTE_COLOR = Brushes.DarkBlue;
        private readonly Brush NOTE_TEXT_COLOR = Brushes.White;

        private readonly Brush MEASURE_HEADER_TEXT_COLOR = Brushes.Black;
        private readonly Pen MEASURE_HEADER_COLOR = Pens.Red;
        public SequencerControl()
        {
            InitializeComponent();
            font = new Font(FontFamily.GenericSansSerif, 9);
        }

        private void DrawNote(Graphics g, string value, float x, float y, float w, float h, Brush color)
        {
            g.FillRectangle(NOTE_COLOR, x, y, w, h);

            g.DrawString(value, font, NOTE_TEXT_COLOR, new PointF(x, y));
        }

        private float RenderBeat(Graphics g, Beat b, float x, float w, float h)
        {
            if (x >= 0 && x <= Bounds.Width)
            {
                //
                foreach (Note n in b.Notes)
                {
                    float posY = RENDER_OFFSET_Y + (n.String * NOTE_HEIGHT);
                    float noteWidth = BEAT_WIDTH * (n.Duration + 2);
                    DrawNote(g, n.Value.ToString(), x, posY, noteWidth, NOTE_HEIGHT, NOTE_COLOR);
                }
            }
            return BEAT_WIDTH;
        }

        private float RenderMeasure(Graphics g, Measure m, float x, float h)
        {
            if (x >= 0 && x <= Bounds.Width)
            {
                string marker = string.Empty;
                if (m.Header.Marker != null) marker = $" ({m.Header.Marker.Value.Title})";

                g.DrawString($"{m.Header.Number}{marker}", font, MEASURE_HEADER_TEXT_COLOR, new PointF(x, 0));
                
                //measure header line
                g.DrawLine(MEASURE_HEADER_COLOR, x, 0, x, h);
            }

            float posX = 0;
            bool first = true;
            foreach (Beat b in m.Voices[0].Beats)
            {
                //draw Beat line
                if (first)
                {
                    first = false;
                } else { 
                    g.DrawLine(Pens.Black, x + posX, RENDER_OFFSET_Y, x + posX, h);
                }
                posX += RenderBeat(g, b, posX + x, BEAT_WIDTH, h);
            }
            return posX;
        }

        private void Render(Graphics g, Track t)
        {
            g.DrawLine(Pens.Black, 0, RENDER_OFFSET_Y, Bounds.Width, RENDER_OFFSET_Y);

            float posX = -_trackX;
            for (int i = 0; i < _track.Measures.Count; i++)
            {
                Measure m = _track.Measures[i];
                //calculate measure absolute X:
                
                //
                if (posX > Bounds.Width) break;
                posX += RenderMeasure(g, m, posX, Bounds.Height);
                if (posX > Bounds.Width) break;
            }
        }

        private void SequencerControl_Paint(object sender, PaintEventArgs e)
        {
            if (_track == null)
            {
                e.Graphics.DrawString("No Track Selected :(", font, Brushes.Black, new PointF(0, 0));
                return;
            }
            Render(e.Graphics, _track);
        }

        private void SequencerControl_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }
    }
}
