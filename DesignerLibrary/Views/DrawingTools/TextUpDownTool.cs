using DesignerLibrary.Attributes;
using DesignerLibrary.Consts;
using DesignerLibrary.Helpers;
using DesignerLibrary.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DesignerLibrary.DrawingTools
{
    class TextUpDownTool : TextTool
    {
        protected override void OnPaint(PaintEventArgs args)
        {
            Graphics graph = args.Graphics;

            Clear(graph);

            DrawUpArea(graph);
            DrawSeparator(graph);
            DrawDownArea(graph);
        }

        protected virtual void DrawUpArea(Graphics graph)
        {
            Format.Alignment = AlignRight ? StringAlignment.Far : StringAlignment.Near;
            Format.LineAlignment = StringAlignment.Near;
            graph.DrawString(Text, Font, TextBrush, Bounds, Format);
        }

        protected virtual void DrawDownArea(Graphics graph)
        {
            Format.Alignment = AlignRight ? StringAlignment.Far : StringAlignment.Near;
            Format.LineAlignment = StringAlignment.Far;
            graph.DrawString(BottomRight, ItalicFont, TextBrush, Bounds, Format);
        }

        private void DrawSeparator(Graphics graph)
        {
            int y = (Bounds.Top + Bounds.Bottom) / 2;
            graph.DrawLine(Pens.Black, Bounds.Left, y, Bounds.Right, y);
        }


        protected override ToolPersistence NewPersistence()
        {
            return new TextUpDownToolPersistence();
        }

        protected override void OnSetPersistence()
        {
            base.OnSetPersistence();

            ItalicFont = new Font(Font.FontFamily, Font.SizeInPoints, FontStyle.Italic, GraphicsUnit.Point);
        }

        protected Font ItalicFont { get; set; }
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

        public bool AlignRight
        {
            get { return Persistence.AlignRight; }
            set
            {
                Persistence.AlignRight = value;
                IsDirty = true;
                Invalidate();
            }
        }

        private new TextUpDownToolPersistence Persistence
        {
            get { return base.Persistence as TextUpDownToolPersistence; }
        }

        protected override IList<PropertyDescriptor> GetPropertyDescriptors()
        {
            IList<PropertyDescriptor> descriptors = base.GetPropertyDescriptors();

            descriptors.Add(new MyPropertyDescriptor(this, PropertyNames.BottomRight,
                new Attribute[]
                {
                    CustomVisibleAttribute.Yes,
                    new LocalizedCategoryAttribute( "Appearance" ),
                    new LocalizedDisplayNameAttribute( "BottomRight" ),
                    new PropertyOrderAttribute((int)PropertyOrder.BottomRight),
                }));

            descriptors.Add(new MyPropertyDescriptor(this, PropertyNames.AlignRight,
                new Attribute[]
                {
                    CustomVisibleAttribute.Yes,
                    new LocalizedCategoryAttribute( "Appearance" ),
                    new LocalizedDisplayNameAttribute( "AlignRight" ),
                    new PropertyOrderAttribute((int)PropertyOrder.AlignRight),
                }));

            return descriptors;
        }
    }
}
