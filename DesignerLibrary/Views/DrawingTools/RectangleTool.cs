using DesignerLibrary.Attributes;
using DesignerLibrary.Consts;
using DesignerLibrary.Helpers;
using DesignerLibrary.Persistence;
using DesignerLibrary.Trackers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
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
                OnBoundsChanged();
                (Persistence as RectangleToolPersistence).Bounds = value;
                IsDirty = true;
                Invalidate();
            }
        }

        protected virtual void OnBoundsChanged()
        {
        }

        public RectangleTool()
        {
            base.Tracker = new RectangleTracker(this);
        }

        protected override ToolPersistence NewPersistence()
        {
            return new RectangleToolPersistence();
        }

        protected override void OnLocationChanged(Point offset)
        {
            Rectangle rect = Bounds;

            rect.Offset(offset);
            Bounds = rect;
        }

        protected override void OnPaint(PaintEventArgs args)
        {
            Graphics graph = args.Graphics;

            graph.FillRectangle(Brush, Bounds);
            graph.DrawRectangle(Pen, Bounds);
        }

        protected override void FillPath(GraphicsPath path)
        {
            path.AddRectangle(Bounds);
        }

        protected override bool OnHitTest(Point point)
        {
            return Bounds.Contains(point);
        }

        protected override Rectangle GetSurroundingRect()
        {
            return Bounds;
        }

        protected override void OnStartResize(Point point)
        {
            Location = point;
            Adjust.MovingPointIndex = (int)RectTrackerAdjust.RectPointIndex.eBottomRight;
        }

        protected override void OnResize(Point point)
        {
            Rectangle lRect = Bounds;

            Adjust.Resize(point, ref lRect);
            Bounds = lRect;
        }

        protected override bool OnEndResize(Point point)
        {
            OnResize(point);
            return true;
        }

        protected override IList<PropertyDescriptor> GetPropertyDescriptors()
        {
            IList<PropertyDescriptor> descriptors = base.GetPropertyDescriptors();

            descriptors.Add(new SiPropertyDescriptor(this, PropertyNames.Bounds,
                new Attribute[]
                {
                    CustomVisibleAttribute.Yes,
                    new LocalizedCategoryAttribute( "Appearance" ),
                    new LocalizedDisplayNameAttribute( "Bounds" ),
                    new PropertyOrderAttribute( (int)Consts.PropertyOrder.eBounds )
                }));

            return descriptors;
        }
    }
}
