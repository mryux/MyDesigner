using DesignerLibrary.Persistence;
using DesignerLibrary.Trackers;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DesignerLibrary.DrawingTools
{
    class ArcTool : RectangleTool
    {
        public ArcTool()
        {
            base.Tracker = new ArcTracker( this );
        }

        protected override ToolPersistence NewPersistence()
        {
            return new ArcToolPersistence();
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

        protected override bool OnHitTest(Point pPoint)
        {
            return Region.IsVisible( pPoint );
        }

        protected override void FillPath(GraphicsPath pPath)
        {
            pPath.AddArc( Bounds, StartAngle, SweepAngle );
        }

        public float StartAngle
        {
            get { return Persistence.StartAngle; }
            set { Persistence.StartAngle = value; }
        }

        public float SweepAngle
        {
            get { return Persistence.SweepAngle; }
            set { Persistence.SweepAngle = value; }
        }

        private new ArcToolPersistence Persistence
        {
            get { return base.Persistence as ArcToolPersistence; }
        }

        private ArcTrackerAdjust ArcAdjust
        {
            get { return Adjust as ArcTrackerAdjust; }
        }

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
