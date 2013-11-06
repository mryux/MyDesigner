using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using DesignerLibrary.DrawingTools;

namespace DesignerLibrary.Trackers
{
    class RectangleTracker : DrawingTracker
    {
        public RectangleTracker(DrawingTool pTool)
            : base( pTool )
        {
        }

        private byte GetIndex(VIndex pVIndex, HIndex pHIndex)
        {
            return (byte)((byte)pHIndex | (byte)pVIndex << 4);
        }

        protected void GetIndex(byte pIndex, out VIndex pVIndex, out HIndex pHIndex)
        {
            pHIndex = (HIndex)(pIndex & 0x0f);
            pVIndex = (VIndex)(pIndex >> 4 & 0x0f);
        }

        protected enum HIndex { eLeft = 1, eCenter, eRight }
        protected enum VIndex { eTop = 1, eCenter, eBottom }

        protected enum RectPointIndex 
        { 
            eNone,
            eTopLeft = VIndex.eTop << 4 | HIndex.eLeft,
            eTopCenter = VIndex.eTop << 4 | HIndex.eCenter,
            eTopRight = VIndex.eTop << 4 | HIndex.eRight,
            eMidLeft = VIndex.eCenter << 4 | HIndex.eLeft,
            eMidRight = VIndex.eCenter << 4 | HIndex.eRight,
            eBottomLeft = VIndex.eBottom << 4 | HIndex.eLeft,
            eBottomCenter = VIndex.eBottom << 4 | HIndex.eCenter,
            eBottomRight = VIndex.eBottom << 4 | HIndex.eRight,
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
        }

        private void Rectify(Rectangle pRect, Point pPoint, ref Point pLocation, ref Size pSize, ref RectPointIndex pIndex)
        {
            VIndex lVIndex;
            HIndex lHIndex;

            GetIndex( (byte)pIndex, out lVIndex, out lHIndex );

            if (pSize.Height < 0)
            {
                pLocation.Y = lVIndex == VIndex.eTop ? pRect.Bottom : pPoint.Y;

                lVIndex = (lVIndex == VIndex.eTop) ? VIndex.eBottom : VIndex.eTop;
                pSize.Height = -pSize.Height;
            }

            if (pSize.Width < 0)
            {
                pLocation.X = lHIndex == HIndex.eLeft ? pRect.Right : pPoint.X;

                lHIndex = (lHIndex == HIndex.eLeft) ? HIndex.eRight : HIndex.eLeft;
                pSize.Width = -pSize.Width;
            }

            pIndex = (RectPointIndex)GetIndex( lVIndex, lHIndex );
        }

        protected override void OnResize(Point pPoint)
        {
            Rectangle lRect = ResizingRect;
            Point lLocation = lRect.Location;
            Size lSize = lRect.Size;
            RectPointIndex lMovingPointIndex = (RectPointIndex)MovingPointIndex;

            switch (lMovingPointIndex)
            {
                case RectPointIndex.eTopLeft:
                    lLocation = pPoint;
                    lSize = new Size( lRect.Right - pPoint.X, lRect.Bottom - pPoint.Y );
                    break;

                case RectPointIndex.eTopCenter:
                    lLocation.Y = pPoint.Y;
                    lSize.Height = lRect.Bottom - pPoint.Y;
                    break;

                case RectPointIndex.eTopRight:
                    lLocation.Y = pPoint.Y;
                    lSize = new Size( pPoint.X - lRect.Left, lRect.Bottom - pPoint.Y );
                    break;

                case RectPointIndex.eMidLeft:
                    lLocation.X = pPoint.X;
                    lSize.Width = lRect.Right - pPoint.X;
                    break;

                case RectPointIndex.eMidRight:
                    lSize.Width = pPoint.X - lRect.Left;
                    break;

                case RectPointIndex.eBottomLeft:
                    lLocation.X = pPoint.X;
                    lSize = new Size( lRect.Right - pPoint.X, pPoint.Y - lRect.Top );
                    break;

                case RectPointIndex.eBottomCenter:
                    lSize.Height = pPoint.Y - lRect.Top;
                    break;

                case RectPointIndex.eBottomRight:
                    lSize = new Size( pPoint.X - lRect.Left, pPoint.Y - lRect.Top );
                    break;
            }

            Rectify( lRect, pPoint, ref lLocation, ref lSize, ref lMovingPointIndex );
            MovingPointIndex = (int)lMovingPointIndex;
            ResizingRect = new Rectangle( lLocation, lSize );
        }

        protected override void OnEndResize()
        {
            RectangleTool lTool = DrawingTool as RectangleTool;

            lTool.Rect = ResizingRect;
        }

        protected override Rectangle GetSurroundingRect()
        {
            return IsResizing ? ResizingRect : DrawingTool.SurroundingRect;
        }
    }
}
