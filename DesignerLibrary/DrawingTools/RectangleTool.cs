using System.Drawing;
using System.Windows.Forms;
using DesignerLibrary.Trackers;

namespace DesignerLibrary.DrawingTools
{
    class RectangleTool : DrawingTool
    {
        public Rectangle Rect { get; set; }

        public RectangleTool()
        {
            Rect = new Rectangle( 0, 0, 100, 100 );
            base.Tracker = new RectangleTracker( this );
        }

        protected override void OnLocationChanged(Point pOffset)
        {
            Rectangle lRect = Rect;

            lRect.Offset( pOffset );
            Rect = lRect;
        }

        protected override void OnPaint(PaintEventArgs pArgs)
        {
            pArgs.Graphics.DrawRectangle( Pen, Rect );
        }

        protected override bool OnHitTest(Point pPoint)
        {
            return Rect.Contains( pPoint );
        }

        protected override Rectangle GetSurroundingRect()
        {
            return Rect;
        }

        protected override void OnStartResize(Point pPoint)
        {
            Location = pPoint;
        }

        protected override void OnResize(Point pPoint)
        {
            Rectangle lRect = Rect;

            lRect.Width = pPoint.X - Location.X;
            lRect.Height = pPoint.Y - Location.Y;

            Rect = lRect;
        }

        protected override bool OnEndResize(Point pPoint)
        {
            OnResize( pPoint );
            return true;
        }
    }
}
