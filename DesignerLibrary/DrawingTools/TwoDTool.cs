using DesignerLibrary.Attributes;
using DesignerLibrary.Consts;
using DesignerLibrary.Helpers;
using DesignerLibrary.Persistence;
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
        }

        protected override void OnSetPersistence()
        {
            base.OnSetPersistence();

            Brush = new SolidBrush( FillColor );
        }

        public Color FillColor
        {
            get { return (Persistence as TwoDToolPersistence).FillColor; }
            set
            {
                (Persistence as TwoDToolPersistence).FillColor = value;
                Brush = new SolidBrush( value );
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
                    new PropertyOrderAttribute( (int)PropertyOrder.eFillColor )
                } ) );

            return lDescriptors;
        }
    }
}
