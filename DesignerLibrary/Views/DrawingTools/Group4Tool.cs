﻿using DesignerLibrary.Attributes;
using DesignerLibrary.Consts;
using DesignerLibrary.Helpers;
using DesignerLibrary.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace DesignerLibrary.DrawingTools
{
    class Group4Tool : TextUpDownTool
    {
        public Group4Tool()
        {
        }

        protected override void DrawUpArea(Graphics graph)
        {
            Format.Alignment = StringAlignment.Near;
            Format.LineAlignment = StringAlignment.Near;
            graph.DrawString(Text, Font, TextBrush, Bounds, Format);

            Format.Alignment = StringAlignment.Far;
            Format.LineAlignment = StringAlignment.Near;
            graph.DrawString(TopRight, Font, TextBrush, Bounds, Format);
        }

        protected override void DrawDownArea(Graphics graph)
        {
            base.DrawDownArea(graph);

            Format.Alignment = StringAlignment.Near;
            Format.LineAlignment = StringAlignment.Far;
            graph.DrawString(BottomLeft, ItalicFont, TextBrush, Bounds, Format);
        }

        protected override ToolPersistence NewPersistence()
        {
            return new Group4ToolPersistence();
        }

        public string TopRight
        {
            get { return Persistence.TopRight; }
            set
            {
                Persistence.TopRight = value;
                IsDirty = true;
                Invalidate();
            }
        }

        public string BottomLeft
        {
            get { return Persistence.BottomLeft; }
            set
            {
                Persistence.BottomLeft = value;
                IsDirty = true;
                Invalidate();
            }
        }

        private new Group4ToolPersistence Persistence
        {
            get { return base.Persistence as Group4ToolPersistence; }
        }

        protected override IList<PropertyDescriptor> GetPropertyDescriptors()
        {
            IList<PropertyDescriptor> descriptors = base.GetPropertyDescriptors();

            descriptors.Add(new MyPropertyDescriptor(this, PropertyNames.BottomLeft,
                new Attribute[]
                {
                    CustomVisibleAttribute.Yes,
                    new LocalizedCategoryAttribute( "Appearance" ),
                    new LocalizedDisplayNameAttribute( "BottomLeft" ),
                    new PropertyOrderAttribute((int)PropertyOrder.BottomLeft),
                }));

            descriptors.Add(new MyPropertyDescriptor(this, PropertyNames.TopRight,
                new Attribute[]
                {
                    CustomVisibleAttribute.Yes,
                    new LocalizedCategoryAttribute( "Appearance" ),
                    new LocalizedDisplayNameAttribute( "TopRight" ),
                    new PropertyOrderAttribute((int)PropertyOrder.TopRight),
                }));

            return descriptors;
        }
    }
}
