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

        protected override void OnPaint(PaintEventArgs pArgs)
        {
            pArgs.Graphics.DrawRectangle( Pen, Bounds );

            if (!string.IsNullOrEmpty( FileLocation ))
                pArgs.Graphics.DrawImage( Image.FromFile( FileLocation ), Bounds );
        }

        private string _FileLocation = string.Empty;
        public string FileLocation
        {
            get { return _FileLocation; }
            set
            {
                _FileLocation = value;
                IsDirty = true;
                Invalidate();
            }
        }

        public override string GetFieldError(string pFieldName)
        {
            string lRet = string.Empty;

            if (pFieldName == PropertyNames.FileLocation)
            {
                if (string.IsNullOrEmpty( FileLocation ))
                    lRet = Properties.Resources.Error_InvalidFileLocation;
            }

            return lRet;
        }

        protected override IList<PropertyDescriptor> GetPropertyDescriptors()
        {
            IList<PropertyDescriptor> lDescriptors = base.GetPropertyDescriptors();

            // remove FillColor property descriptor.
            var lFillColor = lDescriptors.FirstOrDefault( e => e.Name == PropertyNames.FillColor );
            if (lFillColor != null)
                lDescriptors.Remove( lFillColor );

            lDescriptors.Add( new SiPropertyDescriptor( this, PropertyNames.FileLocation,
                new Attribute[] 
                { 
                    CustomVisibleAttribute.Yes,
                    new LocalizedCategoryAttribute( "Appearance" ),
                    new LocalizedDisplayNameAttribute( "FileLocation" ),
                    new EditorAttribute( typeof( ImageFileTypeEditor ), typeof( UITypeEditor ) ),
                    new PropertyOrderAttribute( (int)Consts.PropertyOrder.eFileLocation )
                } ) );

            return lDescriptors;
        }

        //protected override void OnSetSite(ISite pSite)
        //{
        //    base.OnSetSite( pSite );

        //    IDesignerHost pHost = pSite.Container as IDesignerHost;

        //    Control lView = (pHost.GetDesigner( pHost.RootComponent ) as IRootDesigner).GetView( ViewTechnology.Default ) as Control;

        //    if (lView != null)
        //    {
        //        _PictureBox = new PictureBox();
        //        _PictureBox.Bounds = Rect;
        //        //_PictureBox.ImageLocation = @"C:\Downloads\fun1.gif";
        //        _PictureBox.SizeMode = PictureBoxSizeMode.StretchImage;

        //        lView.Controls.Add( _PictureBox );
        //    }
        //}
    }
}
