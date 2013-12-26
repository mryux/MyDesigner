﻿using DesignerLibrary.Trackers;
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
                pArgs.Graphics.FillRegion( Brush, GetRegion() );
                pArgs.Graphics.DrawArc( Pen, Bounds, StartAngle, SweepAngle );
            }
        }

        private Region GetRegion()
        {
            GraphicsPath lPath = new GraphicsPath();

            lPath.AddArc( Bounds, StartAngle, SweepAngle );
            lPath.CloseFigure();
            return new Region( lPath );
        }

        protected override bool OnHitTest(Point pPoint)
        {
            GraphicsPath lPath = new GraphicsPath();

            lPath.AddArc( Bounds, StartAngle, SweepAngle );
            lPath.CloseFigure();

            return new Region( lPath ).IsVisible( pPoint );
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
