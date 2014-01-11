using DesignerLibrary.Persistence;
using DesignerLibrary.Trackers;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DesignerLibrary.DrawingTools
{
    class EllipseTool : RectangleTool
    {
        public EllipseTool()
        {
            base.Tracker = new EllipseTracker( this );
        }

        protected override ToolPersistence NewPersistence()
        {
            return new EllipseToolPersistence();
        }

        protected override void OnPaint(PaintEventArgs pArgs)
        {
            pArgs.Graphics.FillEllipse( Brush, Bounds );
            pArgs.Graphics.DrawEllipse( Pen, Bounds );
        }

        protected override bool OnHitTest(Point pPoint)
        {
            GraphicsPath lPath = new GraphicsPath();

            lPath.AddEllipse( Bounds );
            lPath.CloseFigure();
            
            return new Region( lPath ).IsVisible( pPoint );
        }
    }
}
