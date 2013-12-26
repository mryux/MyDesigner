using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SCF.SiPass.Explorer.Common.Converters;
using SCF.SiPass.Explorer.Common.Helpers;
using SCF.SiPass.SitePlan.Module.Attributes;
using SCF.SiPass.SitePlan.Module.Constants;
using SCF.SiPass.SitePlan.Module.Trackers;

namespace SCF.SiPass.SitePlan.Module.DrawingTools
{
    abstract class DrawingTool : ICustomTypeDescriptor, IComponent, IDataErrorInfo
    {
        protected DrawingTool()
        {
            Pen = new Pen( Color.Black, 2.0f );
        }

        public Color ForeColor
        {
            get { return Pen.Color; }
            set { Pen.Color = value; }
        }

        public enum LineWidth { eThin, eMedium, eThick }

        private LineWidth _LineWidth = LineWidth.eThin;
        public LineWidth Width
        {
            get { return _LineWidth; }
            set
            {
                _LineWidth = value;
                switch(_LineWidth)
                {
                    case DrawingTool.LineWidth.eThin:
                        Pen.Width = 1.0f;
                        break;

                    case DrawingTool.LineWidth.eMedium:
                        Pen.Width = 2.0f;
                        break;

                    case DrawingTool.LineWidth.eThick:
                        Pen.Width = 3.0f;
                        break;
                }
            }
        }

        public Pen Pen { get; private set; }
        public DrawingTracker Tracker { get; protected set; }

        protected TrackerAdjust Adjust { get { return Tracker.Adjust; } }

        private Point _Location = Point.Empty;

        public Point Location
        {
            get { return _Location; }
            set
            {
                Point lOffset = new Point( value.X - _Location.X, value.Y - _Location.Y );

                _Location = value;
                OnLocationChanged( lOffset );
            }
        }

        protected abstract void OnLocationChanged(Point pOffset);
        protected abstract void OnPaint(PaintEventArgs pArgs);
        protected abstract bool OnHitTest(Point pPoint);
        protected abstract Rectangle GetSurroundingRect();
        protected abstract void OnStartResize(Point pPoint);
        protected abstract void OnResize(Point pPoint);
        protected abstract bool OnEndResize(Point pPoint);
        
        public static Rectangle GetClipRect(Point[] pPoints)
        {
            Rectangle lRet = Rectangle.Empty;

            if (pPoints.Count() > 1)
            {
                int lMinX = pPoints.Min( e => e.X );
                int lMinY = pPoints.Min( e => e.Y );
                int lMaxX = pPoints.Max( e => e.X );
                int lMaxY = pPoints.Max( e => e.Y );

                lRet = new Rectangle( lMinX, lMinY, lMaxX - lMinX, lMaxY - lMinY );
            }

            return lRet;
        }

        public void Paint(PaintEventArgs pArgs)
        {
            if (pArgs.ClipRectangle.IntersectsWith( SurroundingRect ))
                OnPaint( pArgs );
        }

        public bool HitTest(Point pPoint)
        {
            bool lRet = false;

            if (SurroundingRect.Contains( pPoint ))
                lRet = OnHitTest( pPoint );

            return lRet;
        }

        private bool _Selected = false;
        public bool Selected
        {
            get { return _Selected; }
            set
            {
                _Selected = value;
                Pen.Color = _Selected ? Color.Red : Color.Black;
            }
        }

        public Rectangle SurroundingRect
        {
            get 
            {
                Rectangle lRect = GetSurroundingRect();
                int lWidth = (int)(Pen.Width / 2) + 1;  // +1 for SmoothingMode.AntiAlias

                lRect.Inflate( lWidth, lWidth );
                return lRect;
            }
        }

        public void DoDrop(Point pOffset)
        {
            Point lLocation = Location;

            lLocation.Offset( pOffset );
            Location = lLocation;
        }

        public bool IsResizing { get; protected set; }
        public void StartResize(Point pPoint)
        {
            IsResizing = true;
            OnStartResize( pPoint );
        }

        public void Resize(Point pPoint)
        {
            OnResize( pPoint );
        }

        public bool EndResize(Point pPoint)
        {
            IsResizing = OnEndResize( pPoint );
            return IsResizing;
        }

        protected virtual IList<PropertyDescriptor> GetPropertyDescriptors()
        {
            IList<PropertyDescriptor> lRet = new List<PropertyDescriptor>();

            lRet.Add( new SiPropertyDescriptor( this, PropertyNames.ForeColor, 
                new Attribute[] 
                { 
                    CustomVisibleAttribute.Yes,
                    new LocalizedCategoryAttribute( "Category_Appearance" ),
                    new LocalizedDisplayNameAttribute( "DisplayName_ForeColor" ) 
                } ) );
            // add Point property
            //lRet.Add( GetPointPropertyDescriptor() );

            return lRet;
        }

        #region "IComponent implementation"
        private EventHandler _Disposed;
        event EventHandler IComponent.Disposed
        {
            add { _Disposed += value; }
            remove { _Disposed -= value; }
        }

        ISite IComponent.Site { get; set; }

        void IDisposable.Dispose()
        {
        }
        #endregion

        #region ICustomTypeDescriptor Members

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes( this, true );
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName( this, true );
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName( this, true );
        }

        private static readonly BrowsablePropertiesConverter Converter = new BrowsablePropertiesConverter();
        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return Converter;
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent( this, true );
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty( this, true );
        }

        object ICustomTypeDescriptor.GetEditor(System.Type editorBaseType)
        {
            return TypeDescriptor.GetEditor( this, editorBaseType, true );
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents( this, true );
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(System.Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents( this, attributes, true );
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return TypeDescriptor.GetProperties( this, true );
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            return new PropertyDescriptorCollection( GetPropertyDescriptors().ToArray() );
        }

        #endregion

        #region IDataErrorInfo Members
        string IDataErrorInfo.Error
        {
            get { return null; }
        }

        public virtual string GetFieldError(string pFieldName)
        {
            return string.Empty;
        }

        string IDataErrorInfo.this[string pFieldName]
        {
            get { return GetFieldError( pFieldName ); }
        }
        #endregion
    }
}
