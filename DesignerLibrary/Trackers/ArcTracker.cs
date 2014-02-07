using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DesignerLibrary.DrawingTools;

namespace DesignerLibrary.Trackers
{
    using ArcPointIndex = ArcTrackerAdjust.ArcPointIndex;
    using HIndex = RectTrackerAdjust.HIndex;
    using VIndex = RectTrackerAdjust.VIndex;

    class ArcTracker : RectangleTracker
    {
        public ArcTracker(DrawingTool pTool)
            : base( pTool )
        {
            Adjust = new ArcTrackerAdjust();
        }

        private ArcTrackerAdjust ArcAdjust { get { return Adjust as ArcTrackerAdjust; } }

        protected override Dictionary<int, Point> TrackerPoints
        {
            get
            {
                Dictionary<int, Point> lRet = new Dictionary<int, Point>();

                // take out center points to avoid tracker points overlapping Angle Points.
                base.TrackerPoints.Where( p =>
                {
                    HIndex lHIndex;
                    VIndex lVIndex;

                    RectTrackerAdjust.GetIndex( (byte)p.Key, out lVIndex, out lHIndex );

                    return lVIndex != VIndex.eCenter
                        && lHIndex != HIndex.eCenter;
                } ).All( p =>
                {
                    lRet.Add( p.Key, p.Value );
                    return true;
                } );

                // add startAnglePoint/endAnglePoint
                ArcTool lTool = DrawingTool as ArcTool;

                lRet.Add( (int)ArcPointIndex.eStartAngle, GetPoint( lTool.Bounds, lTool.StartAngle ) );
                lRet.Add( (int)ArcPointIndex.eEndAngle, GetPoint( lTool.Bounds, lTool.StartAngle + lTool.SweepAngle ) );

                return lRet;
            }
        }
        
        protected override void OnResizePaint(PaintEventArgs pArgs)
        {
            ArcTool lTool = DrawingTool as ArcTool;

            if (ResizingRect.Height > 0
                && ResizingRect.Width > 0)
            {
                pArgs.Graphics.DrawArc( Pen, ResizingRect, ArcAdjust.StartAngle, ArcAdjust.SweepAngle );
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
                        lRet = Cursors.SizeWE;
                        break;
                }
            }

            return lRet;
        }

        protected override void OnStartResize(Point pPoint)
        {
            base.OnStartResize( pPoint );

            ArcTool lTool = DrawingTool as ArcTool;

            ArcAdjust.StartAngle = lTool.StartAngle;
            ArcAdjust.SweepAngle = lTool.SweepAngle;
        }

        protected override void OnEndResize()
        {
            base.OnEndResize();

            if (ArcAdjust.SweepAngle < 0)
            {
                ArcAdjust.StartAngle += ArcAdjust.SweepAngle;
                ArcAdjust.SweepAngle = -ArcAdjust.SweepAngle;
            }

            ArcTool lTool = DrawingTool as ArcTool;

            lTool.StartAngle = ArcAdjust.StartAngle;
            lTool.SweepAngle = ArcAdjust.SweepAngle;
        }

        /// <summary>
        /// get point with specific angle on ellipse around pRect
        /// </summary>
        /// <param name="pRect"></param>
        /// <param name="dAngle"></param>
        /// <returns></returns>
        private Point GetPoint(Rectangle pRect, double dAngle)
        {
            Point lPoint = GetCenter( pRect );

            double lX = pRect.Width / 2 * Math.Cos( dAngle * Math.PI / 180.0 );
            double lY = pRect.Height / 2 * Math.Sin( dAngle * Math.PI / 180.0 );

            lPoint.Offset( (int)lX, (int)lY );
            return lPoint;
        }
    }
}
