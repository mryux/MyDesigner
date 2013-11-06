using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DesignerLibrary.Trackers;
using System.Windows.Forms;
using System.Drawing;

namespace DesignerLibrary.DrawingTools
{
    class ArcTool : RectangleTool
    {
        public float StartAngle { get; set; }
        public float SweepAngle { get; set; }

        public ArcTool()
        {
            StartAngle = 180.0f;
            SweepAngle = 180.0f;

            base.Tracker = new ArcTracker( this );
        }

        protected override void OnPaint(PaintEventArgs pArgs)
        {
            if (Rect.Height > 0
                && Rect.Width > 0)
            {
                pArgs.Graphics.DrawArc( Pen, Rect, StartAngle, SweepAngle );
            }
        }
    }
}
