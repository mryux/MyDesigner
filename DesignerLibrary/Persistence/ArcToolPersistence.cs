using System;
using System.Drawing;

namespace DesignerLibrary.Persistence
{
    public class ArcToolPersistence : RectangleToolPersistence
    {
        public ArcToolPersistence()
            : base( typeof( DrawingTools.ArcTool ) )
        {
            StartAngle = 180.0f;
            SweepAngle = 180.0f;
        }

        const double Rad2Deg = 180.0 / Math.PI;
        const double Deg2Rad = Math.PI / 180.0;

        private int Angle360Round(Point pStart, Point pEnd)
        {
            double lAngle = Math.Atan2( pStart.Y - pEnd.Y, pStart.X - pEnd.X ) * Rad2Deg;

            return ((int)lAngle + 360) % 360;
        }

        public float StartAngle { get; set; }
        public float SweepAngle { get; set; }
    }
}
