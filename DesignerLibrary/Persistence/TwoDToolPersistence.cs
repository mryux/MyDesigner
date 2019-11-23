using System;
using System.Drawing;
using System.Xml.Serialization;

namespace DesignerLibrary.Persistence
{
    public abstract class TwoDToolPersistence : ToolPersistence
    {
        protected TwoDToolPersistence(Type pToolType)
            : base( pToolType )
        {
            FillColor = Color.Transparent;
        }

        [XmlIgnore]
        public Color FillColor { get; set; }

        [XmlElement( "FillColor" )]
        public int FillColorAsArgb
        {
            get { return FillColor.ToArgb(); }
            set { FillColor = Color.FromArgb( value ); }
        }
    }
}
