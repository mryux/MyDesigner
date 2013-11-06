using System.Windows.Forms;
using DesignerLibrary.DrawingTools;

namespace DesignerLibrary.Trackers
{
    class EllipseTracker : RectangleTracker
    {
        public EllipseTracker(DrawingTool pTool)
            : base( pTool )
        {
        }

        protected override void OnResizePaint(PaintEventArgs pArgs)
        {
            pArgs.Graphics.DrawEllipse( Pen, ResizingRect );
        }
    }
}
