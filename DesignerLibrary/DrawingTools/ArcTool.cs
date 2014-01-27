using DesignerLibrary.Trackers;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

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
            if (Bounds.Height > 0
                && Bounds.Width > 0)
            {
                pArgs.Graphics.FillRegion( Brush, Region );
                pArgs.Graphics.DrawArc( Pen, Bounds, StartAngle, SweepAngle );
            }
        }

        protected override void FillPath(GraphicsPath pPath)
        {
            pPath.AddArc( Bounds, StartAngle, SweepAngle );
        }

        private ArcTrackerAdjust ArcAdjust { get { return Adjust as ArcTrackerAdjust; } }

        protected override void OnStartResize(Point pPoint)
        {
            base.OnStartResize( pPoint );

            ArcAdjust.StartAngle = StartAngle;
            ArcAdjust.SweepAngle = SweepAngle;
        }

        protected override void OnResize(Point pPoint)
        {
            base.OnResize( pPoint );

            StartAngle = ArcAdjust.StartAngle;
            SweepAngle = ArcAdjust.SweepAngle;
        }
    }
}
