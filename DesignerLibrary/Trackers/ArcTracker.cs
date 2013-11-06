using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DesignerLibrary.DrawingTools;
using System.Windows.Forms;
using System.Drawing;

namespace DesignerLibrary.Trackers
{
    class ArcTracker : RectangleTracker
    {
        public ArcTracker(DrawingTool pTool)
            : base( pTool )
        {
        }

        private enum ArcPointIndex { eNone, eStartAngle, eEndAngle }

        protected override Dictionary<int, Point> TrackerPoints
        {
            get
            {
                Dictionary<int, Point> lRet = new Dictionary<int,Point>();

                // take out center points to avoid tracker points overlapping Angle Points.
                base.TrackerPoints.Where( p =>
                {
                    HIndex lHIndex;
                    VIndex lVIndex;

                    GetIndex( (byte)p.Key, out lVIndex, out lHIndex );

                    return lVIndex != VIndex.eCenter
                        && lHIndex != HIndex.eCenter;
                } ).All( p =>
                {
                    lRet.Add( p.Key, p.Value );
                    return true;
                } );

                // add startAnglePoint/endAnglePoint
                ArcTool lTool = DrawingTool as ArcTool;

                lRet.Add( (int)ArcPointIndex.eStartAngle, GetAnglePoint( lTool.Rect, lTool.StartAngle ) );
                lRet.Add( (int)ArcPointIndex.eEndAngle, GetAnglePoint( lTool.Rect, lTool.StartAngle + lTool.SweepAngle ) );

                return lRet;
            }
        }

        private Point GetAnglePoint(Rectangle pRect, double dAngle)
        {
            int lHalfWidth = pRect.Width / 2;
            int lHalfHeight = pRect.Height / 2;
            Point lOffset = new Point( lHalfWidth, lHalfHeight );

            Point lOrigin = pRect.Location;
            lOrigin.Offset( lOffset );
            double lX = lOrigin.X + lHalfWidth * Math.Cos( dAngle * Math.PI / 180.0 );
            double lY = lOrigin.Y + lHalfHeight * Math.Sin( dAngle * Math.PI / 180.0 );

            return new Point( (int)lX, (int)lY );
        }

        protected override void OnResizePaint(PaintEventArgs pArgs)
        {
            ArcTool lTool = DrawingTool as ArcTool;

            if (ResizingRect.Height > 0
                && ResizingRect.Width > 0)
            {
                pArgs.Graphics.DrawArc( Pen, ResizingRect, lTool.StartAngle, lTool.SweepAngle );
            }
        }

        protected override Cursor GetHoverCursor(Point pPoint)
        {
            Cursor lRet = base.GetHoverCursor( pPoint );

            if (lRet == Cursors.Default)
            {
                ArcPointIndex lIndex = (ArcPointIndex)HitTest( pPoint );
                
                switch( lIndex )
                {
                    case ArcPointIndex.eStartAngle:
                    case ArcPointIndex.eEndAngle:
                        lRet = Cursors.PanNorth;
                        break;
                }
            }

            return lRet;
        }
    }
}
