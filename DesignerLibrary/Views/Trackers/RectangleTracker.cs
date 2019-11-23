using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DesignerLibrary.DrawingTools;

namespace DesignerLibrary.Trackers
{
    using RectPointIndex = RectTrackerAdjust.RectPointIndex;

    class RectangleTracker : DrawingTracker
    {
        public RectangleTracker(DrawingTool pTool)
            : base( pTool )
        {
            Adjust = new RectTrackerAdjust();
        }

        protected Rectangle ResizingRect = Rectangle.Empty;

        protected override Dictionary<int, Point> TrackerPoints
        {
            get
            {
                Rectangle lRect = DrawingTool.SurroundingRect;
                Dictionary<int, Point> lRet = new Dictionary<int, Point>();

                new Tuple<RectPointIndex, Point>[]
                {
                    new Tuple<RectPointIndex, Point>( RectPointIndex.eTopLeft, new Point(0, 0) ),
                    new Tuple<RectPointIndex, Point>( RectPointIndex.eTopCenter, new Point(lRect.Width/2, 0) ),
                    new Tuple<RectPointIndex, Point>( RectPointIndex.eTopRight, new Point(lRect.Width, 0) ),

                    new Tuple<RectPointIndex, Point>( RectPointIndex.eMidLeft, new Point(0, lRect.Height/2) ),
                    new Tuple<RectPointIndex, Point>( RectPointIndex.eMidRight, new Point(lRect.Width, lRect.Height/2) ),

                    new Tuple<RectPointIndex, Point>( RectPointIndex.eBottomLeft, new Point(0, lRect.Height) ),
                    new Tuple<RectPointIndex, Point>( RectPointIndex.eBottomCenter, new Point(lRect.Width/2, lRect.Height) ),
                    new Tuple<RectPointIndex, Point>( RectPointIndex.eBottomRight, new Point(lRect.Width, lRect.Height) ),
                }.All( e => 
                {
                    Point lLocation = lRect.Location;
                    lLocation.Offset( e.Item2 );

                    lRet.Add( (int)e.Item1, lLocation );
                    return true;
                });

                return lRet;
            }
        }

        protected override void OnResizePaint(PaintEventArgs pArgs)
        {
            pArgs.Graphics.DrawRectangle( Pen, ResizingRect );
        }

        protected override Cursor GetHoverCursor(Point pPoint)
        {
            Cursor lRet = Cursors.Default;
            RectPointIndex lIndex = (RectPointIndex)HitTest( pPoint );

            switch (lIndex)
            {
                case RectPointIndex.eTopLeft:
                case RectPointIndex.eBottomRight:
                    lRet = Cursors.SizeNWSE;
                    break;

                case RectPointIndex.eTopRight:
                case RectPointIndex.eBottomLeft:
                    lRet = Cursors.SizeNESW;
                    break;

                case RectPointIndex.eTopCenter:
                case RectPointIndex.eBottomCenter:
                    lRet = Cursors.SizeNS;
                    break;

                case RectPointIndex.eMidLeft:
                case RectPointIndex.eMidRight:
                    lRet = Cursors.SizeWE;
                    break;
            }

            return lRet;
        }

        protected override void OnStartResize(Point pPoint)
        {
            ResizingRect = DrawingTool.SurroundingRect;
            Adjust.MovingPointIndex = MovingPointIndex;
        }

        protected override void OnResize(Point pPoint)
        {
            Adjust.Resize( pPoint, ref ResizingRect );
        }

        protected override void OnEndResize()
        {
            RectangleTool lTool = DrawingTool as RectangleTool;

            lTool.Bounds = ResizingRect;
        }

        protected override Rectangle GetSurroundingRect()
        {
            return IsResizing ? ResizingRect : DrawingTool.SurroundingRect;
        }
    }
}
