using System.Collections.Generic;
using System.Drawing;

namespace DesignerLibrary.Persistence
{
    public class PolygonToolPersistence : TwoDToolPersistence
    {
        public PolygonToolPersistence()
        {
            Points = new List<Point>();
        }

        internal override DrawingTools.DrawingTool NewDrawingTool()
        {
            return new DrawingTools.PolygonTool();
        }

        public List<Point> Points { get; set; }
    }
}
