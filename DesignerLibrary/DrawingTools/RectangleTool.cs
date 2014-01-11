using DesignerLibrary.Attributes;
using DesignerLibrary.Constants;
using DesignerLibrary.Helpers;
using DesignerLibrary.Persistence;
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
        public Rectangle Bounds
        {
            get { return (Persistence as RectangleToolPersistence).Bounds; }
            set
            {
                Invalidate();
                (Persistence as RectangleToolPersistence).Bounds = value;
                Invalidate();
            }
        }

        public RectangleTool()
        {
            base.Tracker = new RectangleTracker( this );
        }

        protected override ToolPersistence NewPersistence()
        {
            return new RectangleToolPersistence();
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
