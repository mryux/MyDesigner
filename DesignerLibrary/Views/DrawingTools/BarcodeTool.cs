using DesignerLibrary.Attributes;
using DesignerLibrary.Consts;
using DesignerLibrary.Helpers;
using DesignerLibrary.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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
            BarcodeInstance = new BarcodeLib.Barcode()
            {
                BackColor = Color.White,//图片背景颜色
                ForeColor = Color.Black,//条码颜色
                IncludeLabel = false,
                Alignment = BarcodeLib.AlignmentPositions.CENTER,
                LabelPosition = BarcodeLib.LabelPositions.BOTTOMCENTER, //code的显示位置
                ImageFormat = ImageFormat.Bmp, //图片格式
                LabelFont = BarcodeLabelFont,
                BarWidth = 2,
            };
        }

        protected override ToolPersistence NewPersistence()
        {
            return new BarcodePersistence();
        }

        private static readonly Font BarcodeLabelFont = new Font("verdana", 8f);
        private BarcodeLib.Barcode BarcodeInstance;
        private Image BarcodeImg;
        private int LabelHeight = 0;

        protected override void OnPaint(PaintEventArgs args)
        {
            base.OnPaint(args);

            Graphics graph = args.Graphics;

            if (BarcodeImg != null)
            {
                if (LabelHeight == 0)
                {
                    LabelHeight = (int)graph.MeasureString(Barcode, BarcodeLabelFont, Bounds.Width, Format).Height;
                }

                GraphicsUnit unit = GraphicsUnit.Display;
                RectangleF rect = BarcodeImg.GetBounds(ref unit);
                RectangleF targetRect = new RectangleF(Bounds.Left, Bounds.Top, Bounds.Width, Bounds.Height - LabelHeight);

                graph.DrawImage(BarcodeImg, targetRect, rect, unit);
            }

            graph.DrawString(Barcode, BarcodeLabelFont, Brushes.Black, Bounds, Format);
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

        protected override void OnSetRuntimeValue(string value)
        {
            base.OnSetRuntimeValue(value);

            Barcode = value;
        }

        private void MakeBarcodeImage()
        {
            if (string.IsNullOrEmpty(Barcode))
                return;

            Size size = new Size(Bounds.Width, Bounds.Height);
            size = GraphicsMapper.Instance.TransformSize(size, CoordinateSpace.Device, CoordinateSpace.Page);

            if (size.Width > 100 && size.Height > 50)
            {
                BarcodeInstance.Height = size.Height;
                BarcodeInstance.Width = size.Width;

                try
                {
                    BarcodeImg = BarcodeInstance.Encode(BarcodeLib.TYPE.CODE128, Barcode);
                }
                catch (Exception ex)
                {
                    MessageBoxHelper.OKCancelMessage(ex.Message);
                }
            }
        }

        protected override IList<PropertyDescriptor> GetPropertyDescriptors()
        {
            IList<PropertyDescriptor> descriptors = base.GetPropertyDescriptors();

            descriptors.Add(new MyPropertyDescriptor(this, PropertyNames.Barcode,
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
