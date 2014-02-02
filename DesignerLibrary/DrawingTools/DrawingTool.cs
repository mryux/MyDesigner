using DesignerLibrary.Attributes;
using DesignerLibrary.Consts;
using DesignerLibrary.Converters;
using DesignerLibrary.Helpers;
using DesignerLibrary.Persistence;
using DesignerLibrary.Trackers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace DesignerLibrary.DrawingTools
{
    abstract class DrawingTool : IComponent, ICustomTypeDescriptor, IDataErrorInfo
    {
        protected DrawingTool()
        {
        }

        private ToolPersistence _Persistence = null;
        public ToolPersistence Persistence
        {
            get 
            {
                return _Persistence;
            }

            set
            {
                _Persistence = value;
                OnSetPersistence();
                Invalidate();
            }
        }

        protected virtual void OnSetPersistence()
        {
            Pen = new Pen( _Persistence.PenColor, GetLineWidth( _Persistence.PenWidth ) );
        }

        public Color PenColor
        {
            get { return _Persistence.PenColor; }
            set
            {
                _Persistence.PenColor = value;

                Pen.Color = value;
                Invalidate();
            }
        }

        public LineWidth PenWidth
        {
            get { return _Persistence.PenWidth; }
            set
            {
                _Persistence.PenWidth = value;

                Pen.Width = GetLineWidth( value );
                Invalidate();
            }
        }

        private float GetLineWidth(LineWidth pWidth)
        {
            float lRet = 1.0f;

            switch (pWidth)
            {
                case LineWidth.Thin:
                    lRet = 1.0f;
                    break;

                case LineWidth.Medium:
                    lRet = 2.0f;
                    break;

                case LineWidth.Thick:
                    lRet = 3.0f;
                    break;
            }

            return lRet;
        }

        protected Region Region 
        {
            get
            {
                GraphicsPath lPath = new GraphicsPath();

                FillPath( lPath );
                lPath.CloseFigure();

                return new Region( lPath );
            }
        }

        public bool IsOverlapped(DrawingTool pTool, Graphics pGraph)
        {
            Region lRegion = Region;
            
            lRegion.Intersect( pTool.Region );
            return !lRegion.IsEmpty( pGraph );
        }

        public Pen Pen { get; private set; }
        public DrawingTracker Tracker { get; protected set; }
        protected TrackerAdjust Adjust { get { return Tracker.Adjust; } }

        public Point Location
        {
            get { return _Persistence.Location; }
            set
            {
                Point lOffset = new Point( value.X - Location.X, value.Y - Location.Y );

                Invalidate();   // invalidate old area
                //_Persistence.Location = value;
                OnLocationChanged( lOffset );
                Invalidate();   // invalidate new area
            }
        }

        public ToolPersistence CreatePersistence()
        {
            return NewPersistence();
        }

        protected abstract ToolPersistence NewPersistence();
        protected abstract void OnLocationChanged(Point pOffset);
        protected abstract void OnPaint(PaintEventArgs pArgs);
        protected abstract Rectangle GetSurroundingRect();
        protected abstract void OnStartResize(Point pPoint);
        protected abstract void OnResize(Point pPoint);
        protected abstract bool OnEndResize(Point pPoint);
        protected abstract void FillPath(GraphicsPath pPath);

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
            if (pArgs.Graphics.ClipBounds.IntersectsWith( SurroundingRect ))
                OnPaint( pArgs );
        }

        public bool HitTest(Point pPoint)
        {
            bool lRet = false;

            if (SurroundingRect.Contains( pPoint ))
                lRet = OnHitTest( pPoint );

            return lRet;
        }

        protected virtual bool OnHitTest(Point pPoint)
        {
            return Region.IsVisible( pPoint );
        }

        private bool _Selected = false;
        public bool Selected
        {
            get { return _Selected; }
            set
            {
                _Selected = value;
                Pen.Color = _Selected ? Color.Red : _Persistence.PenColor;
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

        protected virtual void OnEscape()
        {
        }

        public void Escape()
        {
            OnEscape();
            IsResizing = false;
        }

        public Bitmap DefaultImage
        {
            get
            {
                Size lSize = new Size(SurroundingRect.Size.Width + 1, SurroundingRect.Size.Height + 1);
                Bitmap lRet = new Bitmap(lSize.Width, lSize.Height);

                using (Graphics lGraph = Graphics.FromImage(lRet))
                {
                    OnPaint(new PaintEventArgs(lGraph, Rectangle.Empty));
                }

                return lRet;
            }
        }

        public Bitmap GetImage(int pWidth, int pHeight)
        {
            Size lSize = new Size(SurroundingRect.Size.Width + 1, SurroundingRect.Size.Height + 1);
            Bitmap lRet = new Bitmap(lSize.Width, lSize.Height);

            using(Bitmap tmpImage = new Bitmap(pWidth, pHeight))
            {
                using (Graphics lGraph = Graphics.FromImage(tmpImage))
                {
                    OnPaint(new PaintEventArgs(lGraph, Rectangle.Empty));
                }

                using(Graphics lGraph = Graphics.FromImage(lRet))
                {
                    lGraph.DrawImage(tmpImage, 0, 0, SurroundingRect, GraphicsUnit.Pixel);
                }
            }

            return lRet;
        }

        protected virtual void OnRun(Control pControl)
        {
        }

        public void Run(Control pControl)
        {
            OnRun( pControl );
        }

        public event EventHandler RedrawEvent;

        protected void Invalidate()
        {
            if (RedrawEvent != null)
                RedrawEvent( this, EventArgs.Empty );
        }

        protected ICollection<string> Fields = new List<string>();
        public string Validate()
        {
            string lErrorMsg = string.Empty;

            Fields.Any( field =>
            {
                lErrorMsg = (this as IDataErrorInfo)[field];
                return !string.IsNullOrEmpty( lErrorMsg );
            } );

            return lErrorMsg;
        }

        private IList<PropertyDescriptor> GetProperties()
        {
            IList<PropertyDescriptor> lRet = GetPropertyDescriptors();

            Fields = (from p in lRet
                      select p.Name).ToList();

            return lRet;
        }

        protected virtual IList<PropertyDescriptor> GetPropertyDescriptors()
        {
            IList<PropertyDescriptor> lRet = new List<PropertyDescriptor>();

            lRet.Add( new SiPropertyDescriptor( this, PropertyNames.Location,
                new Attribute[] 
                { 
                    CustomVisibleAttribute.Yes,
                    new CategoryAttribute( "Appearance" ),
                    new DisplayNameAttribute( "Location" ),
                    new PropertyOrderAttribute( (int)PropertyOrder.eLocation )
                } ) );

            lRet.Add( new SiPropertyDescriptor( this, PropertyNames.PenColor,
                new Attribute[] 
                { 
                    CustomVisibleAttribute.Yes,
                    new CategoryAttribute( "Appearance" ),
                    new DisplayNameAttribute( "LineColor" ),
                    new PropertyOrderAttribute( (int)PropertyOrder.eLineColor ),
                } ) );

            lRet.Add( new SiPropertyDescriptor( this, PropertyNames.PenWidth,
                new Attribute[]
                {
                    CustomVisibleAttribute.Yes,
                    new CategoryAttribute( "Appearance" ),
                    new DisplayNameAttribute( "LineWidth" ),
                    new PropertyOrderAttribute( (int)PropertyOrder.eLineWidth ),
                    new TypeConverterAttribute( typeof( LineWidthConverter ) ),
                } ) );

            return lRet;
        }

        #region IComponent implementation
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
            return new PropertyDescriptorCollection( GetProperties().ToArray() );
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
