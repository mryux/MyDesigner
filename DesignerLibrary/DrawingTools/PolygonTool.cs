using DesignerLibrary.Persistence;
using DesignerLibrary.Trackers;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace DesignerLibrary.DrawingTools
{
    class PolygonTool : TwoDTool
    {
        public List<Point> Points
        {
            get { return (Persistence as PolygonToolPersistence).Points; }
            set
            {
                (Persistence as PolygonToolPersistence).Points = value;
            }
        }
        private Point _MovingPoint = Point.Empty;

        public PolygonTool()
        {
            base.Tracker = new PolygonTracker( this );
        }

        protected override ToolPersistence NewPersistence()
        {
            return new PolygonToolPersistence();
        }

        protected override void OnLocationChanged(Point pOffset)
        {
            List<Point> lPoints = new List<Point>();

            Points.All( p =>
            {
                p.Offset( pOffset );
                lPoints.Add( p );
                return true;
            } );

            Points = lPoints;
        }

        protected override void OnPaint(PaintEventArgs pArgs)
        {
            int lCount = Points.Count();

            if (lCount > 1)
                pArgs.Graphics.DrawLines( Pen, Points.ToArray() );

            if (_MovingPoint != Point.Empty)
            {
                pArgs.Graphics.DrawLine( Tracker.Pen, _MovingPoint, Points.Last() );
                pArgs.Graphics.DrawLine( Tracker.Pen, _MovingPoint, Points.First() );
            }
            else if (lCount > 1)
            {
                pArgs.Graphics.FillRegion( Brush, GetRegion() );
                pArgs.Graphics.DrawLine( Pen, Points.Last(), Points.First() );
            }
        }

        protected override bool OnHitTest(Point pPoint)
        {
            return GetRegion().IsVisible( pPoint );
        }

        protected override Rectangle GetSurroundingRect()
        {
            Rectangle lRet = Rectangle.Empty;
            List<Point> lPoints = new List<Point>( Points );

            if (_MovingPoint != Point.Empty)
                lPoints.Add( _MovingPoint );

            return DrawingTool.GetClipRect( lPoints.ToArray() );
        }

        private Region GetRegion()
        {
            GraphicsPath lPath = new GraphicsPath();

            lPath.AddLines( Points.ToArray() );
            lPath.CloseFigure();
            return new Region( lPath );
        }

        protected override void OnStartResize(Point pPoint)
        {
            Points.Add( pPoint );
            _MovingPoint = pPoint;
        }

        protected override void OnResize(Point pPoint)
        {
            _MovingPoint = pPoint;
        }

        protected override bool OnEndResize(Point pPoint)
        {
            bool lRet = false;
            Point lStartPoint = Points.First();
            Rectangle lRect = Tracker.GetTrackerRect( lStartPoint );

            // mouse up on start point
            if (lRect.Contains( pPoint ))
            {
                _MovingPoint = Point.Empty;
                lRet = true;
            }
            else
            {
                Points.Add( pPoint );
                _MovingPoint = pPoint;
            }

            return lRet;
        }

        protected override void OnEscape()
        {
            Point lStartPoint = Points.First();

            OnEndResize( lStartPoint );
        }
    }
}
