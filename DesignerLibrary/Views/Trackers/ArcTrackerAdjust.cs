using System;
using System.Drawing;

namespace DesignerLibrary.Trackers
{
    class ArcTrackerAdjust : RectTrackerAdjust
    {
        public float StartAngle { get; set; }
        public float SweepAngle { get; set; }
        public enum ArcPointIndex { eNone, eStartAngle, eEndAngle }

        protected override void OnResize(Point pPoint, ref Rectangle pRect)
        {
            base.OnResize( pPoint, ref pRect );

            switch ((ArcPointIndex)MovingPointIndex)
            {
                case ArcPointIndex.eStartAngle:
                    float lAngle = (float)GetAngle( pRect, pPoint );

                    SweepAngle = RectifyAngle( StartAngle + SweepAngle - lAngle );
                    StartAngle = lAngle;
                    break;

                case ArcPointIndex.eEndAngle:
                    SweepAngle = RectifyAngle( (float)GetAngle( pRect, pPoint ) - StartAngle );
                    break;
            }
        }

        /// <summary>
        /// rectify startAngle/sweepAngle.
        /// </summary>
        /// <param name="pSize"></param>
        protected override void OnRectify(Size pSize)
        {
            if (pSize.Height < 0)
            {
                StartAngle = RectifyAngle( -StartAngle );
                SweepAngle = -SweepAngle;
            }

            if (pSize.Width < 0)
            {
                StartAngle = RectifyAngle( 180 - StartAngle );
                SweepAngle = -SweepAngle;
            }
        }

        private float GetAngle(Rectangle pRect, Point pPoint)
        {
            Point lCenter = DrawingTracker.GetCenter( pRect );
            float lAngle = (float)(Math.Atan2( pPoint.Y - lCenter.Y, pPoint.X - lCenter.X ) * (180.0 / Math.PI));

            return ArcTrackerAdjust.RectifyAngle( lAngle );
        }

        public static float RectifyAngle(float pAngle)
        {
            return (pAngle + 360) % 360;
        }
    }
}
