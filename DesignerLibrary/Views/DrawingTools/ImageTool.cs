using DesignerLibrary.Attributes;
using DesignerLibrary.Consts;
using DesignerLibrary.Helpers;
using DesignerLibrary.Persistence;
using DesignerLibrary.TypeEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace DesignerLibrary.DrawingTools
{
    class ImageTool : RectangleTool
    {
        private PictureBox _PictureBox = null;

        public ImageTool()
        {
        }

        protected override ToolPersistence NewPersistence()
        {
            return new ImageToolPersistence();
        }

        protected override void OnPaint(PaintEventArgs args)
        {
            args.Graphics.DrawRectangle(Pen, Bounds);

            if (!string.IsNullOrEmpty(FileLocation))
                args.Graphics.DrawImage(Image.FromFile(FileLocation), Bounds);
        }

        protected override void OnRuntimeInitialize(Control parent)
        {
            base.OnRuntimeInitialize(parent);

            _PictureBox = new PictureBox();
            _PictureBox.Bounds = GraphicsMapper.Instance.TransformRectangle(Bounds, CoordinateSpace.Device, CoordinateSpace.Page);
            _PictureBox.ImageLocation = FileLocation;
            _PictureBox.SizeMode = PictureBoxSizeMode.StretchImage;

            parent.Controls.Add(_PictureBox);
        }

        public string FileLocation
        {
            get { return Persistence.ImagePath; }
            set
            {
                Persistence.ImagePath = value;
                IsDirty = true;
                Invalidate();
            }
        }

        private new ImageToolPersistence Persistence
        {
            get { return base.Persistence as ImageToolPersistence; }
        }

        public override string GetFieldError(string pFieldName)
        {
            string lRet = string.Empty;

            if (pFieldName == PropertyNames.FileLocation)
            {
                if (string.IsNullOrEmpty(FileLocation))
                    lRet = Properties.Resources.Error_InvalidFileLocation;
            }

            return lRet;
        }

        protected override IList<PropertyDescriptor> GetPropertyDescriptors()
        {
            IList<PropertyDescriptor> lDescriptors = base.GetPropertyDescriptors();

            // remove FillColor property descriptor.
            var lFillColor = lDescriptors.FirstOrDefault(e => e.Name == PropertyNames.FillColor);
            if (lFillColor != null)
                lDescriptors.Remove(lFillColor);

            lDescriptors.Add(new SiPropertyDescriptor(this, PropertyNames.FileLocation,
                new Attribute[]
                {
                    CustomVisibleAttribute.Yes,
                    new LocalizedCategoryAttribute( "Appearance" ),
                    new LocalizedDisplayNameAttribute( "FileLocation" ),
                    new EditorAttribute( typeof( ImageFileTypeEditor ), typeof( UITypeEditor ) ),
                    new PropertyOrderAttribute( (int)Consts.PropertyOrder.eFileLocation )
                }));

            return lDescriptors;
        }
    }
}
