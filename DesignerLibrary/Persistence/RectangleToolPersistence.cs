using System.Drawing;
using System.Xml.Serialization;

namespace DesignerLibrary.Persistence
{
    public class RectangleToolPersistence : TwoDToolPersistence
    {
        public RectangleToolPersistence()
        {
            Size = new Size ( 100, 100 );
        }

        internal override DrawingTools.DrawingTool NewDrawingTool()
        {
            return new DrawingTools.RectangleTool();
        }

        public Size Size { get; set; }

        [XmlIgnore]
        public Rectangle Bounds
        {
            get { return new Rectangle( Location, Size ); }
            set
            {
                Location = value.Location;
                Size = value.Size;
            }
        }
    }
}
