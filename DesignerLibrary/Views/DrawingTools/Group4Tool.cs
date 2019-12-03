using DesignerLibrary.Attributes;
using DesignerLibrary.Consts;
using DesignerLibrary.Helpers;
using DesignerLibrary.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DesignerLibrary.DrawingTools
{
    class Group4Tool : TextTool
    {
        public Group4Tool()
        {
        }

        protected override void OnPaint(PaintEventArgs args)
        {
            Graphics graph = args.Graphics;

            Clear(graph);

            Format.Alignment = StringAlignment.Near;
            Format.LineAlignment = StringAlignment.Near;
            graph.DrawString(Text, Font, TextBrush, Bounds, Format);

            Format.Alignment = StringAlignment.Far;
            Format.LineAlignment = StringAlignment.Near;
            graph.DrawString(TopRight, Font, TextBrush, Bounds, Format);

            int y = (Bounds.Top + Bounds.Bottom) / 2;
            graph.DrawLine(Pen, Bounds.Left, y, Bounds.Right, y);

            Format.Alignment = StringAlignment.Near;
            Format.LineAlignment = StringAlignment.Far;
            graph.DrawString(BottomLeft, ItalicFont, TextBrush, Bounds, Format);

            Format.Alignment = StringAlignment.Far;
            Format.LineAlignment = StringAlignment.Far;
            graph.DrawString(BottomRight, ItalicFont, TextBrush, Bounds, Format);
        }

        protected override ToolPersistence NewPersistence()
        {
            return new Group4Persistence();
        }

        protected override void OnSetPersistence()
        {
            base.OnSetPersistence();

            ItalicFont = new Font(Font.FontFamily, Font.SizeInPoints, FontStyle.Italic, GraphicsUnit.Point);
        }

        private Font ItalicFont { get; set; }

        public string TopRight
        {
            get { return Persistence.TopRight; }
            set
            {
                Persistence.TopRight = value;
                IsDirty = true;
                Invalidate();
            }
        }

        public string BottomLeft
        {
            get { return Persistence.BottomLeft; }
            set
            {
                Persistence.BottomLeft = value;
                IsDirty = true;
                Invalidate();
            }
        }

        public string BottomRight
        {
            get { return Persistence.BottomRight; }
            set
            {
                Persistence.BottomRight = value;
                IsDirty = true;
                Invalidate();
            }
        }

        private new Group4Persistence Persistence
        {
            get { return base.Persistence as Group4Persistence; }
        }

        protected override IList<PropertyDescriptor> GetPropertyDescriptors()
        {
            IList<PropertyDescriptor> descriptors = base.GetPropertyDescriptors();

            descriptors.Add(new MyPropertyDescriptor(this, PropertyNames.BottomLeft,
                new Attribute[]
                {
                    CustomVisibleAttribute.Yes,
                    new LocalizedCategoryAttribute( "Appearance" ),
                    new LocalizedDisplayNameAttribute( "BottomLeft" ),
                    new PropertyOrderAttribute((int)PropertyOrder.BottomLeft),
                }));

            descriptors.Add(new MyPropertyDescriptor(this, PropertyNames.BottomRight,
                new Attribute[]
                {
                    CustomVisibleAttribute.Yes,
                    new LocalizedCategoryAttribute( "Appearance" ),
                    new LocalizedDisplayNameAttribute( "BottomRight" ),
                    new PropertyOrderAttribute((int)PropertyOrder.BottomRight),
                }));

            descriptors.Add(new MyPropertyDescriptor(this, PropertyNames.TopRight,
                new Attribute[]
                {
                    CustomVisibleAttribute.Yes,
                    new LocalizedCategoryAttribute( "Appearance" ),
                    new LocalizedDisplayNameAttribute( "TopRight" ),
                    new PropertyOrderAttribute((int)PropertyOrder.TopRight),
                }));

            return descriptors;
        }
    }
}
