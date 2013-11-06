using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DesignerLibrary.DrawingTools;

namespace DesignerLibrary.Trackers
{
    class PolygonTracker : DrawingTracker
    {
        public PolygonTracker(DrawingTool pTool)
            : base( pTool )
        {
        }

        private enum PolygonPointIndex { eNone, eStart }

        protected override Dictionary<int, Point> TrackerPoints
        {
            get
            {
                PolygonTool lTool = DrawingTool as PolygonTool;
                Dictionary<int, Point> lRet = new Dictionary<int, Point>();
                int lIndex = (int)PolygonPointIndex.eStart;

                lTool.Points.All( e =>
                {
                    lRet.Add( lIndex++, e );
                    return true;
                } );

                return lRet;
            }
        }

        private Point _MovingPoint;

        protected override void OnResizePaint(PaintEventArgs pArgs)
        {
            // MovingPointIndex is 1 based.
            int lMovingPointIndex = MovingPointIndex - 1;
            var lPoints = (DrawingTool as PolygonTool).Points;
            // because lPoints.First is equal to lPoints.Last
            int lCount = lPoints.Count();

            int lPrevIndex = (lMovingPointIndex + lCount - 1) % lCount;
            int lNextIndex = (lMovingPointIndex + 1) % lCount;

            pArgs.Graphics.DrawLines( Pen, new Point[] { lPoints[lPrevIndex], _MovingPoint, lPoints[lNextIndex] } );
        }

        protected override Cursor GetHoverCursor(Point pPoint)
        {
            return HitTest( pPoint ) > 0 ? Cursors.Cross : Cursors.Default;
        }

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
            PolygonTool lTool = DrawingTool as PolygonTool;

            lTool.Points[MovingPointIndex - 1] = _MovingPoint;
            _MovingPoint = Point.Empty;
        }

        protected override Rectangle GetSurroundingRect()
        {
            List<Point> lPoints = new List<Point>( (DrawingTool as PolygonTool).Points );

            if (MovingPointIndex > 0)
                lPoints[MovingPointIndex - 1] = _MovingPoint;

            return DrawingTool.GetClipRect( lPoints.ToArray() );
        }
    }
}
