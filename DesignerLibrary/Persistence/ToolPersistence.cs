using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Xml.Serialization;

namespace DesignerLibrary.Persistence
{
    public enum LineWidth { Thin, Medium, Thick }

    public abstract class ToolPersistence
    {
        protected ToolPersistence(Type toolType)
        {
            ToolType = toolType;

            Visible = true;
            PenColor = Color.Black;
            PenWidth = LineWidth.Thin;
            Location = Point.Empty;
        }

        private Type ToolType { get; set; }

        internal DrawingTools.DrawingTool CreateDrawingTool(IDesignerHost designerHost)
        {
            DrawingTools.DrawingTool ret = null;

            // construct tool via DesignerHost at DesignTime.
            if (designerHost != null)
                ret = designerHost.CreateComponent(ToolType) as DrawingTools.DrawingTool;
            else
            {
                // construct tool via reflection at Runtime.
                ConstructorInfo ci = ToolType.GetConstructor(new Type[] { });

                ret = ci.Invoke(null) as DrawingTools.DrawingTool;
            }

            ret.Persistence = this;
            return ret;
        }

        protected virtual void OnRectDeserialized(Rectangle rect) { }
        protected virtual void OnFillColorDeserialized(Color color) { }

        [XmlIgnore]
        public Color PenColor { get; set; }

        [XmlElement("PenColor")]
        public int PenColorAsArgb
        {
            get { return PenColor.ToArgb(); }
            set { PenColor = Color.FromArgb(value); }
        }

        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        public bool Visible { get; set; }

        public LineWidth PenWidth { get; set; }
        public Point Location { get; set; }
    }
}
