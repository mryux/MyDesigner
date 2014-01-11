using DesignerLibrary.Persistence;
using DesignerLibrary.Trackers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DesignerLibrary.DrawingTools
{
    class LineTool : DrawingTool
    {
        public LineTool()
        {
            base.Tracker = new LineTracker( this );
        }

        public Point StartPos
        {
            get
            {
                return (Persistence as LineToolPersistence).StartPos;
            }

            set
            {
                (Persistence as LineToolPersistence).StartPos = value;
            }
        }

        public Point EndPos
        {
            get
            {
                return (Persistence as LineToolPersistence).EndPos;
            }

            set
            {
                (Persistence as LineToolPersistence).EndPos = value;
            }
        }

        protected override ToolPersistence NewPersistence()
        {
            return new LineToolPersistence();
        }

        protected override void OnLocationChanged(Point pOffset)
        {
            // update StartPos/EndPos, so PointChanged event could be fired properly
            Point lStartPos = StartPos;
            lStartPos.Offset( pOffset );
            StartPos = lStartPos;

            Point lEndPos = EndPos;
            lEndPos.Offset( pOffset );
            EndPos = lEndPos;
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
