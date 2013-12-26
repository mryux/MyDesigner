using DesignerLibrary.Attributes;
using DesignerLibrary.Constants;
using DesignerLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace DesignerLibrary.DrawingTools
{
    abstract class TwoDTool : DrawingTool
    {
        protected TwoDTool()
        {
            Brush = new SolidBrush( FillColor );
        }

        private Color _FillColor = Color.Transparent;

        public Color FillColor
        {
            get { return _FillColor; }
            set
            {
                _FillColor = value;
                Brush = new SolidBrush( FillColor );
                Invalidate();
            }
        }

        protected Brush Brush { get; set; }

        protected override IList<PropertyDescriptor> GetPropertyDescriptors()
        {
            IList<PropertyDescriptor> lDescriptors = base.GetPropertyDescriptors();

            lDescriptors.Add( new SiPropertyDescriptor( this, PropertyNames.FillColor,
                new Attribute[] 
                { 
                    CustomVisibleAttribute.Yes,
                    new CategoryAttribute( "Appearance" ),
                    new DisplayNameAttribute( "FillColor" ),
                    new PropertyOrderAttribute( 6 )
                } ) );

            return lDescriptors;
        }
    }
}
