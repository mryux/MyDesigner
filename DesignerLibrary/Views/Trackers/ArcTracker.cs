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
                Rectangle lRect = lTool.SurroundingRect;

                lRet.Add( (int)ArcPointIndex.eStartAngle, GetPoint( lRect, lTool.StartAngle ) );
                lRet.Add( (int)ArcPointIndex.eEndAngle, GetPoint( lRect, lTool.StartAngle + lTool.SweepAngle ) );

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
        /// get point on ellipse around pRect via specific angle.
        /// use x^2/a^2 + y^2/b^2 = 1 to calculate point.
        /// </summary>
        /// <param name="pRect"></param>
        /// <param name="pAngle"></param>
        /// <returns></returns>
        private Point GetPoint(Rectangle pRect, float pAngle)
        {
            pAngle = ArcTrackerAdjust.RectifyAngle( pAngle );

            Point lRet = GetCenter( pRect );
            double lRadians = pAngle * Math.PI / 180;
            int a = pRect.Width / 2;
            int b = pRect.Height / 2;
            double lTan = Math.Tan( lRadians );

            double lX = (a * b) / Math.Sqrt( Math.Pow( b, 2 ) + Math.Pow( lTan, 2 ) * Math.Pow( a, 2 ) );
            if (pAngle >= 90.0 && pAngle < 270.0)
                lX = -lX;

            double lY = lTan * lX;
            lRet.Offset( (int)lX, (int)lY );

            return lRet;
        }
    }
}
