using DesignerLibrary.Persistence;
using DesignerLibrary.Trackers;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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
            get { return (Persistence as LineToolPersistence).StartPos; }
            set { (Persistence as LineToolPersistence).StartPos = value; }
        }

        public Point EndPos
        {
            get { return (Persistence as LineToolPersistence).EndPos; }
            set { (Persistence as LineToolPersistence).EndPos = value; }
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

        protected override void FillPath(System.Drawing.Drawing2D.GraphicsPath pPath)
        {
            Point lPoint1 = EndPos;
            Point lPoint2 = StartPos;
            int lMargin = (int)Pen.Width;

            lPoint1.Offset( lMargin, lMargin );
            lPoint2.Offset( lMargin, lMargin );
            pPath.AddLines( new Point[]{ StartPos, EndPos, lPoint1, lPoint2 } );
        }

        protected override void OnPaint(PaintEventArgs pArgs)
        {
            Graphics lGraph = pArgs.Graphics;

            lGraph.DrawLine( Pen, StartPos, EndPos );
        }

        protected override Rectangle GetSurroundingRect()
        {
            return DrawingTool.GetClipRect( new Point[]{ StartPos, EndPos } );
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
