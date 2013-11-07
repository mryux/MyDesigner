using System.Drawing;
using System.Windows.Forms;
using DesignerLibrary.Trackers;
using System.Drawing.Drawing2D;

namespace DesignerLibrary.DrawingTools
{
    class EllipseTool : RectangleTool
    {
        public EllipseTool()
        {
            base.Tracker = new EllipseTracker( this );
        }

        protected override void OnPaint(PaintEventArgs pArgs)
        {
            pArgs.Graphics.DrawEllipse( Pen, Rect );
        }

        protected override bool OnHitTest(Point pPoint)
        {
            GraphicsPath lPath = new GraphicsPath();

            lPath.AddEllipse( Rect );
            lPath.CloseFigure();
            
            return new Region( lPath ).IsVisible( pPoint );
        }
    }
}
