using System;
using System.Drawing;
using System.Xml.Serialization;

namespace DesignerLibrary.Persistence
{
    public class RectangleToolPersistence : TwoDToolPersistence
    {
        public RectangleToolPersistence()
            : this( typeof( DrawingTools.RectangleTool ) )
        {

        }

        public RectangleToolPersistence(Type pToolType)
            : base( pToolType )
        {
            Size = new Size ( 100, 100 );
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
