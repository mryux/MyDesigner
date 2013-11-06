using System.Drawing;
using System.Windows.Forms;
using DesignerLibrary.Trackers;

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
    }
}
