using DesignerLibrary.Attributes;
using DesignerLibrary.Constants;
using DesignerLibrary.Helpers;
using DesignerLibrary.Trackers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DesignerLibrary.DrawingTools
{
    class RectangleTool : TwoDTool
    {
        private Rectangle _Bounds = Rectangle.Empty;
        public Rectangle Bounds
        {
            get { return _Bounds; }
            set
            {
                Invalidate();
                _Bounds = value;
                Invalidate();
            }
        }

        public RectangleTool()
        {
            Bounds = new Rectangle( 0, 0, 100, 100 );

            base.Tracker = new RectangleTracker( this );
        }

        protected override void OnLocationChanged(Point pOffset)
        {
            Rectangle lRect = Bounds;

            lRect.Offset( pOffset );
            Bounds = lRect;
        }

        protected override void OnPaint(PaintEventArgs pArgs)
        {
            pArgs.Graphics.FillRectangle( Brush, Bounds );
            pArgs.Graphics.DrawRectangle( Pen, Bounds );
        }

        protected override bool OnHitTest(Point pPoint)
        {
            return Bounds.Contains( pPoint );
        }

        protected override Rectangle GetSurroundingRect()
        {
            return Bounds;
        }

        protected override void OnStartResize(Point pPoint)
        {
            Location = pPoint;
            Adjust.MovingPointIndex = (int)RectTrackerAdjust.RectPointIndex.eBottomRight;
        }

        protected override void OnResize(Point pPoint)
        {
            Rectangle lRect = Bounds;

            Adjust.Resize( pPoint, ref lRect );
            Bounds = lRect;
        }

        protected override bool OnEndResize(Point pPoint)
        {
            OnResize( pPoint );
            return true;
        }

        protected override IList<PropertyDescriptor> GetPropertyDescriptors()
        {
            IList<PropertyDescriptor> lDescriptors = base.GetPropertyDescriptors();

            lDescriptors.Add( new SiPropertyDescriptor( this, PropertyNames.Bounds,
                new Attribute[] 
                { 
                    CustomVisibleAttribute.Yes,
                    new CategoryAttribute( "Appearance" ),
                    new DisplayNameAttribute( "Bounds" ),
                    new PropertyOrderAttribute( 2 )
                } ) );

            return lDescriptors;
        }
    }
}
