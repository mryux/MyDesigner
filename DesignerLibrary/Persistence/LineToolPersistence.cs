using System.Drawing;

namespace DesignerLibrary.Persistence
{
    public class LineToolPersistence : ToolPersistence
    {
        public LineToolPersistence()
        {
            StartPos = new Point( 0, 0 );
            EndPos = new Point( 100, 100 );
        }

        internal override DrawingTools.DrawingTool NewDrawingTool()
        {
            return new DrawingTools.LineTool();
        }

        public Point StartPos { get; set; }
        public Point EndPos { get; set; }
    }
}
