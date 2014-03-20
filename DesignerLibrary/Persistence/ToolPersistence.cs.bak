using System.Drawing;
using System.Xml.Serialization;

namespace DesignerLibrary.Persistence
{
    public enum LineWidth { Thin, Medium, Thick }

    public abstract class ToolPersistence
    {
        protected ToolPersistence()
        {
            PenColor = Color.Black;
            PenWidth = LineWidth.Thin;
            Location = Point.Empty;
        }

        internal DrawingTools.DrawingTool CreateDrawingTool()
        {
            return NewDrawingTool();
        }

        internal abstract DrawingTools.DrawingTool NewDrawingTool();

        [XmlIgnore]
        public Color PenColor { get; set; }

        [XmlElement("PenColor")]
        public int PenColorAsArgb
        {
            get { return PenColor.ToArgb(); }
            set { PenColor = Color.FromArgb( value ); }
        }

        public LineWidth PenWidth { get; set; }
        public Point Location { get; set; }
    }
}
