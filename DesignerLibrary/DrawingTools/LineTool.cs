using System;
using System.Drawing;
using System.Windows.Forms;
using DesignerLibrary.Trackers;

namespace DesignerLibrary.DrawingTools
{
    class LineTool : DrawingTool
    {
        public LineTool()
        {
            StartPos = new Point( 0, 0 );
            EndPos = new Point( 100, 100 );

            base.Tracker = new LineTracker( this );
        }

        public Point StartPos;
        public Point EndPos;

        protected override void OnLocationChanged(Point pOffset)
        {
            // update StartPos/EndPos, so PointChanged event could be fired properly
            StartPos.Offset( pOffset );
            EndPos.Offset( pOffset );
        }

        protected override void OnPaint(PaintEventArgs pArgs)
        {
            pArgs.Graphics.DrawLine( Pen, StartPos, EndPos );
        }

        /// <summary>
        /// test if specified pPoint is on this line tool.
        /// </summary>
        /// <param name="pPoint"></param>
        /// <returns></returns>
        protected override bool OnHitTest(Point pPoint)
        {
            double lDistanceStartEnd = GetDistance( StartPos, EndPos );
            double lDistance1 = GetDistance( StartPos, pPoint );
            double lDistance2 = GetDistance( pPoint, EndPos );

            return Math.Abs( lDistance1 + lDistance2 - lDistanceStartEnd ) < Pen.Width;
        }

        protected override Rectangle GetSurroundingRect()
        {
            return DrawingTool.GetClipRect( new Point[]{ StartPos, EndPos } );
        }

        private double GetDistance(Point pPoint1, Point pPoint2)
        {
            int lX = pPoint1.X - pPoint2.X;
            int lY = pPoint1.Y - pPoint2.Y;

            return Math.Sqrt( lX * lX + lY * lY );
        }

        protected override void OnStartResize(Point pPoint)
        {
            Location = pPoint;
            EndPos = pPoint;
        }

        protected override void OnResize(Point pPoint)
        {
            EndPos = pPoint;
        }

        protected override bool OnEndResize(Point pPoint)
        {
            OnResize( pPoint );
            return true;
        }
    }
}
