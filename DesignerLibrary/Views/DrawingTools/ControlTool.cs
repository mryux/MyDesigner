using System.Drawing;
using System.Windows.Forms;
using SCF.SiPass.SitePlan.Module.Trackers;

namespace SCF.SiPass.SitePlan.Module.DrawingTools
{
    class ControlTool : BaseTool
    {
        public ControlTool()
        {
            Rect = new Rectangle( 0, 0, 100, 100 );

            base.Tracker = new RectangleTracker( this );
        }

        protected override void DoPaint(PaintEventArgs pArgs)
        {
        }
    }
}
