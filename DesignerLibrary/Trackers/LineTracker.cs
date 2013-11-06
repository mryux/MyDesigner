using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DesignerLibrary.DrawingTools;
using System.Collections.Generic;
using System;

namespace DesignerLibrary.Trackers
{
    class LineTracker : DrawingTracker
    {
        public LineTracker(LineTool pLineTool)
            : base( pLineTool )
        {
        }

        protected override Cursor GetHoverCursor(Point pPoint)
        {
            return HitTest( pPoint ) > 0 ? Cursors.Cross : Cursors.Default;
        }

        protected override void OnResizePaint(PaintEventArgs pArgs)
        {
            var lPoints = GetStartEndPoints();

            pArgs.Graphics.DrawLine( Pen, lPoints.Item1, lPoints.Item2 );
        }

        protected override Rectangle GetSurroundingRect()
        {
            var lPoints = GetStartEndPoints();
            return DrawingTool.GetClipRect( new Point[] { lPoints.Item1, lPoints.Item2 } );
        }

        private enum LinePointIndex { eNone, eStart, eEnd }

        protected override Dictionary<int, Point> TrackerPoints
        {
            get
            {
                var lPoints = GetStartEndPoints();

                return new Dictionary<int, Point>()
                {
                    { (int)LinePointIndex.eStart, lPoints.Item1 },
                    { (int)LinePointIndex.eEnd, lPoints.Item2 },
                };
            }
        }

        private Tuple<Point, Point> GetStartEndPoints()
        {
            LineTool lLineTool = DrawingTool as LineTool;
            Point lStartPos = lLineTool.StartPos;
            Point lEndPos = lLineTool.EndPos;

            if (IsResizing)
            {
                if ((LinePointIndex)MovingPointIndex == LinePointIndex.eStart)
                    lStartPos = _MovingPoint;
                else
                    lEndPos = _MovingPoint;
            }

            return new Tuple<Point, Point>( lStartPos, lEndPos );
        }

        private Point _MovingPoint = Point.Empty;

        protected override void OnStartResize(Point pPoint)
        {
            _MovingPoint = pPoint;
        }

        protected override void OnResize(Point pPoint)
        {
            _MovingPoint = pPoint;
        }
        
        protected override void OnEndResize()
        {
            LineTool lLineTool = DrawingTool as LineTool;

            if ((LinePointIndex)MovingPointIndex == LinePointIndex.eStart)
                lLineTool.StartPos = _MovingPoint;
            else
                lLineTool.EndPos = _MovingPoint;

            _MovingPoint = Point.Empty;
        }
    }
}
