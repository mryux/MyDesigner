using DesignerLibrary.Attributes;
using DesignerLibrary.Consts;
using DesignerLibrary.Converters;
using DesignerLibrary.Helpers;
using DesignerLibrary.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DesignerLibrary.DrawingTools
{
    class TextTool : RectangleTool
    {
        public TextTool()
        {
            Format = new StringFormat();
        }

        protected override ToolPersistence NewPersistence()
        {
            return new TextToolPersistence();
        }

        protected override void OnPaint(PaintEventArgs pArgs)
        {
            base.OnPaint( pArgs );

            Graphics lGraph = pArgs.Graphics;

            lGraph.DrawString( Text, Font, new SolidBrush( TextColor ), Bounds, Format );
        }

        protected StringFormat Format { get; set; }

        protected override void OnSetPersistence()
        {
            base.OnSetPersistence();

            Format.Alignment = Alignment;

            Font lFont = Font.FromLogFont( Persistence.LogFont );
            _Font = new Font( lFont.FontFamily, lFont.SizeInPoints, lFont.Style, GraphicsUnit.Point );
        }

        private TextBox _TextBox = null;

        protected override void OnDoubleClick(Control pSender, MouseEventArgs pArgs)
        {
            base.OnDoubleClick( pSender, pArgs );

            if(_TextBox == null)
            {
                _TextBox = new TextBox();
                _TextBox.Multiline = true;
                pSender.Controls.Add( _TextBox );
            }
            else
                _TextBox.Visible = true;

            _TextBox.Font = Font;
            _TextBox.ForeColor = TextColor;
            if (FillColor != Color.Transparent)
                _TextBox.BackColor = FillColor;
            _TextBox.Bounds = GraphicsMapper.Instance.TransformRectangle( Bounds, CoordinateSpace.Device, CoordinateSpace.Page );
            _TextBox.Focus();
        }

        protected override void OnLostSelection()
        {
            base.OnLostSelection();

            if( _TextBox != null)
            {
                _TextBox.Visible = false;
                Text = _TextBox.Text;
            }
        }

        protected override bool OnKey(Keys pKey)
        {
            bool lRet = base.OnKey( pKey );

            if (pKey == Keys.Delete)
                lRet = !_TextBox.Visible;

            return lRet;
        }

        public Color TextColor
        {
            get { return Persistence.TextColor; }
            set
            {
                Persistence.TextColor = value;
                IsDirty = true;
                Invalidate();
            }
        }

        public string Text
        {
            get { return Persistence.Text; }
            set
            {
                Persistence.Text = value;
                IsDirty = true;
                Invalidate();
            }
        }

        private Font _Font;
        public Font Font
        {
            get { return _Font; }
            set
            {
                _Font = value;
                Persistence.SetLogFont( _Font );

                IsDirty = true;
                Invalidate();
            }
        }

        public StringAlignment Alignment
        {
            get { return Persistence.Alignment; }
            set
            {
                Persistence.Alignment = value;
                Format.Alignment = value;
                IsDirty = true;
                Invalidate();
            }
        }

        private new TextToolPersistence Persistence
        {
            get { return base.Persistence as TextToolPersistence; }
        }

        protected override IList<PropertyDescriptor> GetPropertyDescriptors()
        {
            IList<PropertyDescriptor> lDescriptors = base.GetPropertyDescriptors();

            lDescriptors.Add( new SiPropertyDescriptor( this, PropertyNames.TextColor,
                new Attribute[] 
                { 
                    CustomVisibleAttribute.Yes,
                    new LocalizedCategoryAttribute( "Appearance" ),
                    new LocalizedDisplayNameAttribute( "TextColor" ),
                    new PropertyOrderAttribute( (int)Consts.PropertyOrder.TextColor ),
                } ) );

            lDescriptors.Add( new SiPropertyDescriptor( this, PropertyNames.Text,
                new Attribute[] 
                { 
                    CustomVisibleAttribute.Yes,
                    new LocalizedCategoryAttribute( "Appearance" ),
                    new LocalizedDisplayNameAttribute( "Text" ),
                    new PropertyOrderAttribute( (int)Consts.PropertyOrder.Text ),
                    new EditorAttribute( typeof( MultilineStringEditor ), typeof( UITypeEditor ) ),
                } ) );

            lDescriptors.Add( new SiPropertyDescriptor( this, PropertyNames.Font,
                new Attribute[] 
                { 
                    CustomVisibleAttribute.Yes,
                    new LocalizedCategoryAttribute( "Appearance" ),
                    new LocalizedDisplayNameAttribute( "Font" ),
                    new PropertyOrderAttribute( (int)Consts.PropertyOrder.Font ),
                } ) );

            lDescriptors.Add( new SiPropertyDescriptor( this, PropertyNames.Alignment,
                new Attribute[] 
                { 
                    CustomVisibleAttribute.Yes,
                    new LocalizedCategoryAttribute( "Appearance" ),
                    new LocalizedDisplayNameAttribute( "Alignment" ),
                    new TypeConverterAttribute( typeof( AlignmentConverter ) ),
                    new PropertyOrderAttribute( (int)Consts.PropertyOrder.Alignment ),
                } ) );

            return lDescriptors;
        }
    }
}
