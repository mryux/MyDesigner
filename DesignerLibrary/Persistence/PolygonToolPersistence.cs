using System.Collections.Generic;
using System.Drawing;

namespace DesignerLibrary.Persistence
{
    public class PolygonToolPersistence : TwoDToolPersistence
    {
        public PolygonToolPersistence()
            : base( typeof( DrawingTools.PolygonTool ) )
        {
            Points = new List<Point>();
        }

        public List<Point> Points { get; set; }
    }
}
