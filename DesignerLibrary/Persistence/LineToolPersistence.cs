using System.Drawing;

namespace DesignerLibrary.Persistence
{
    public class LineToolPersistence : ToolPersistence
    {
        public LineToolPersistence()
            : base( typeof( DrawingTools.LineTool ) )
        {
            StartPos = new Point( 0, 0 );
            EndPos = new Point( 100, 100 );
        }

        public Point StartPos { get; set; }
        public Point EndPos { get; set; }
    }
}
