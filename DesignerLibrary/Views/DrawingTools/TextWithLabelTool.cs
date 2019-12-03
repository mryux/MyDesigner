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
    class TextWithLabelTool : TextTool
    {
        public TextWithLabelTool()
        {
            LabelFormat = new StringFormat()
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
            };
        }

        protected override ToolPersistence NewPersistence()
        {
            return new TextWithLabelToolPersistence();
        }

        protected override void OnPaint(PaintEventArgs args)
        {
            base.OnPaint(args);

            Graphics graph = args.Graphics;

            graph.DrawString(Label, ItalicFont, new SolidBrush(TextColor), Bounds, LabelFormat);
        }

        protected override void OnSetPersistence()
        {
            base.OnSetPersistence();

            ItalicFont = new Font(Font.FontFamily, Font.SizeInPoints, FontStyle.Italic, GraphicsUnit.Point);
        }

        private Font ItalicFont { get; set; }
        private StringFormat LabelFormat { get; set; }

        public string Label
        {
            get { return Persistence.Label; }
            set
            {
                Persistence.Label = value;
                IsDirty = true;
                Invalidate();
            }
        }

        private new TextWithLabelToolPersistence Persistence
        {
            get { return base.Persistence as TextWithLabelToolPersistence; }
        }

        protected override IList<PropertyDescriptor> GetPropertyDescriptors()
        {
            IList<PropertyDescriptor> descriptors = base.GetPropertyDescriptors();

            descriptors.Add(new MyPropertyDescriptor(this, PropertyNames.Label,
                new Attribute[]
                {
                    CustomVisibleAttribute.Yes,
                    new LocalizedCategoryAttribute( "Appearance" ),
                    new LocalizedDisplayNameAttribute( "Label" ),
                    new PropertyOrderAttribute((int)Consts.PropertyOrder.Label),
                }));

            return descriptors;
        }
    }
}
