using DesignerLibrary.Attributes;
using DesignerLibrary.Consts;
using DesignerLibrary.Helpers;
using DesignerLibrary.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DesignerLibrary.DrawingTools
{
    class BarcodeTool : RectangleTool
    {
        public BarcodeTool()
        {
            Format = new StringFormat();
            Format.Alignment = StringAlignment.Center;
            Format.LineAlignment = StringAlignment.Far;

            //base.Pen.Color = SystemColors.
        }

        protected override ToolPersistence NewPersistence()
        {
            return new BarcodePersistence();
        }

        private BarcodeLib.Barcode BarcodeInstance = new BarcodeLib.Barcode();
        private Image BarcodeImg;

        protected override void OnPaint(PaintEventArgs args)
        {
            base.OnPaint(args);

            Graphics graph = args.Graphics;

            if (BarcodeImg != null)
            {
                graph.DrawImage(BarcodeImg, Bounds.Left + 5, Bounds.Top + 2);
            }

            graph.DrawString(Barcode, SystemFonts.DefaultFont, Brushes.Black, Bounds, Format);
        }

        private new BarcodePersistence Persistence
        {
            get { return base.Persistence as BarcodePersistence; }
        }

        private StringFormat Format { get; set; }

        public string Barcode
        {
            get { return Persistence.Barcode; }
            set
            {
                Persistence.Barcode = value;
                MakeBarcodeImage();

                IsDirty = true;
                Invalidate();
            }
        }

        protected override void OnBoundsChanged()
        {
            base.OnBoundsChanged();

            MakeBarcodeImage();
        }

        protected override void OnRuntimeInitialize(Control parent)
        {
            base.OnRuntimeInitialize(parent);

            MakeBarcodeImage();
        }

        protected override void OnDrawingAdded()
        {
            base.OnDrawingAdded();

            MakeBarcodeImage();
        }

        private void MakeBarcodeImage()
        {
            if (string.IsNullOrEmpty(Barcode))
                return;

            Size size = new Size(Bounds.Width - 20, Bounds.Height - 40);
            size = GraphicsMapper.Instance.TransformSize(size, CoordinateSpace.Device, CoordinateSpace.Page);

            if (size.Width > 100 && size.Height > 100)
                BarcodeImg = BarcodeInstance.Encode(BarcodeLib.TYPE.CODE128, Barcode, size.Width, size.Height);
        }

        protected override IList<PropertyDescriptor> GetPropertyDescriptors()
        {
            IList<PropertyDescriptor> descriptors = base.GetPropertyDescriptors();

            descriptors.Add(new SiPropertyDescriptor(this, PropertyNames.Barcode,
                new Attribute[]
                {
                    CustomVisibleAttribute.Yes,
                    new LocalizedCategoryAttribute( "Appearance" ),
                    new LocalizedDisplayNameAttribute( "Barcode" ),
                    new PropertyOrderAttribute( (int)PropertyOrder.Barcode ),
                }));

            return descriptors;
        }
    }
}
