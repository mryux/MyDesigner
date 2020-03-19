using DesignerLibrary.Attributes;
using DesignerLibrary.Consts;
using DesignerLibrary.Converters;
using DesignerLibrary.Helpers;
using DesignerLibrary.Persistence;
using DesignerLibrary.Trackers;
using log4net;
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
        protected readonly ILog cLog;

        protected DrawingTool()
        {
            cLog = LogManager.GetLogger(GetType());
        }

        private ToolPersistence _Persistence = null;
        public ToolPersistence Persistence
        {
            get { return _Persistence; }
            set
            {
                _Persistence = value;
                OnSetPersistence();
                Invalidate();
            }
        }

        public int Id
        {
            get { return _Persistence.Id; }
            set { _Persistence.Id = value; }
        }

        public string Name
        {
            get
            {
                ISite lSite = GetSite();

                if (string.IsNullOrEmpty(Persistence.Name))
                    Persistence.Name = lSite.Name;
                else
                    lSite.Name = Persistence.Name;

                return lSite.Name;
            }
        }

        protected virtual void OnSetPersistence()
        {
            Pen = new Pen(_Persistence.PenColor, GetLineWidth(_Persistence.PenWidth));
        }

        public Color PenColor
        {
            get { return _Persistence.PenColor; }
            set
            {
                _Persistence.PenColor = value;

                Pen.Color = value;
                IsDirty = true;
                Invalidate();
            }
        }

        public bool Visible
        {
            get { return _Persistence.Visible; }
            set
            {
                _Persistence.Visible = value;
                IsDirty = true;
                // it's a runtime value, no need to invalidate here.
            }
        }

        public LineWidth PenWidth
        {
            get { return _Persistence.PenWidth; }
            set
            {
                _Persistence.PenWidth = value;

                Pen.Width = GetLineWidth(value);
                IsDirty = true;
                Invalidate();
            }
        }

        public void OnAdded()
        {
            OnDrawingAdded();
        }

        protected virtual void OnDrawingAdded()
        {
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

            return GraphicsMapper.Instance.TransformInt((int)lRet);
        }

        protected Region Region
        {
            get
            {
                // todo: try to reuse Region object for performance concern
                // recreate Region object when Size changed
                GraphicsPath path = new GraphicsPath();

                FillPath(path);
                path.CloseFigure();

                return new Region(path);
            }
        }

        public bool IsOverlapped(DrawingTool tool, Graphics graph)
        {
            Region region = Region;

            region.Intersect(tool.Region);
            return !region.IsEmpty(graph);
        }

        public Pen Pen { get; private set; }
        public DrawingTracker Tracker { get; protected set; }
        protected TrackerAdjust Adjust { get { return Tracker.Adjust; } }

        public Point Location
        {
            get { return _Persistence.Location; }
            set
            {
                Point lOffset = new Point(value.X - Location.X, value.Y - Location.Y);

                Invalidate();   // invalidate old area
                OnLocationChanged(lOffset);
                Invalidate();   // invalidate new area
            }
        }

        public void CreatePersistence()
        {
            Persistence = NewPersistence();
        }

        public void OnRemove()
        {
            ISite lSite = GetSite();

            if (lSite != null)
                lSite.Container.Remove(this);
        }

        protected abstract ToolPersistence NewPersistence();
        protected abstract void OnLocationChanged(Point offset);
        protected abstract void OnPaint(PaintEventArgs args);
        protected abstract Rectangle GetSurroundingRect();
        protected abstract void OnStartResize(Point point);
        protected abstract void OnResize(Point point);
        protected abstract bool OnEndResize(Point point);
        protected abstract void FillPath(GraphicsPath path);

        public static Rectangle GetClipRect(Point[] points)
        {
            Rectangle lRet = Rectangle.Empty;

            if (points.Count() > 1)
            {
                int lMinX = points.Min(e => e.X);
                int lMinY = points.Min(e => e.Y);
                int lMaxX = points.Max(e => e.X);
                int lMaxY = points.Max(e => e.Y);

                lRet = new Rectangle(lMinX, lMinY, lMaxX - lMinX, lMaxY - lMinY);
            }

            return lRet;
        }

        public void Paint(PaintEventArgs args)
        {
            if (InRuntime)
                OnRuntimePaint(args);
            else
                OnDesignPaint(args);
        }

        protected void OnDesignPaint(PaintEventArgs args)
        {
            if (args.Graphics.ClipBounds.IntersectsWith(SurroundingRect))
                OnPaint(args);
        }

        protected virtual void OnRuntimePaint(PaintEventArgs args)
        {
            OnDesignPaint(args);
        }

        public bool HitTest(Point point)
        {
            bool lRet = false;

            if (SurroundingRect.Contains(point))
                lRet = OnHitTest(point);

            return lRet;
        }

        protected virtual bool OnHitTest(Point point)
        {
            return Region.IsVisible(point);
        }

        private bool _Selected = false;
        public bool Selected
        {
            get { return _Selected; }
            set
            {
                _Selected = value;
                if (_Selected)
                    OnSelected();
                else
                    OnLostSelection();
            }
        }

        protected virtual void OnSelected()
        {
        }

        protected virtual void OnLostSelection()
        {
        }

        public bool ProcessKeyMsg(Keys key)
        {
            return OnKey(key);
        }

        protected virtual bool OnKey(Keys key)
        {
            return true;
        }

        public Rectangle SurroundingRect
        {
            get { return GetSurroundingRect(); }
        }

        public void DoDrop(Point offset)
        {
            Point lLocation = Location;

            lLocation.Offset(offset);
            Location = lLocation;
        }

        public bool IsResizing { get; protected set; }

        public void StartResize(Point point)
        {
            IsResizing = true;
            OnStartResize(point);
        }

        public void Resize(Point point)
        {
            OnResize(point);
        }

        public bool EndResize(Point point)
        {
            IsResizing = OnEndResize(point);
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

        protected virtual void OnDoubleClick(Control sender, MouseEventArgs args)
        {
        }

        public void OnMouseDoubleClick(Control sender, MouseEventArgs args)
        {
            OnDoubleClick(sender, args);
        }

        public Image DefaultImage
        {
            get
            {
                Size size = new Size(SurroundingRect.Size.Width, SurroundingRect.Size.Height);
                size = GraphicsMapper.Instance.TransformSize(size, CoordinateSpace.Device, CoordinateSpace.Page);
                Bitmap img = new Bitmap(size.Width + 1, size.Height + 1);

                using (Graphics graph = Graphics.FromImage(img))
                {
                    GraphicsMapper.InitGraphics(graph);
                    OnPaint(new PaintEventArgs(graph, Rectangle.Empty));
                }

                return img;
            }
        }

        public Image GetDraggingImage(int width, int height)
        {
            Bitmap img = null;
            Size size = GraphicsMapper.Instance.TransformSize(new Size(width, height), CoordinateSpace.Device, CoordinateSpace.Page);

            using (Bitmap tmpImage = new Bitmap(size.Width, size.Height))
            {
                using (Graphics graph = Graphics.FromImage(tmpImage))
                {
                    GraphicsMapper.InitGraphics(graph);
                    OnPaint(new PaintEventArgs(graph, Rectangle.Empty));
                }

                size = GraphicsMapper.Instance.TransformSize(SurroundingRect.Size, CoordinateSpace.Device, CoordinateSpace.Page);
                img = new Bitmap(size.Width + 1, size.Height + 1);
                using (Graphics graph = Graphics.FromImage(img))
                {
                    Rectangle rect = GraphicsMapper.Instance.TransformRectangle(SurroundingRect, CoordinateSpace.Device, CoordinateSpace.Page);
                    graph.DrawImage(tmpImage, 0, 0, rect, GraphicsUnit.Pixel);
                }
            }

            return img;
        }

        protected virtual void OnRun(Control control)
        {
        }

        public void Run(Control control)
        {
            OnRun(control);
        }

        protected virtual void OnRuntimeInitialize(Control parent)
        {
        }

        protected virtual void OnDisposed()
        {
        }

        protected bool InRuntime { get; set; }

        public void RuntimeInitialize(Control parent)
        {
            InRuntime = true;
            OnRuntimeInitialize(parent);
        }

        public event EventHandler RedrawEvent;

        protected void Invalidate()
        {
            if (RedrawEvent != null)
                RedrawEvent(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs<bool>> IsDirtyEvent;

        private bool _IsDirty = false;
        protected bool IsDirty
        {
            get { return _IsDirty; }
            set
            {
                _IsDirty = value;
                if (IsDirtyEvent != null)
                    IsDirtyEvent(this, new EventArgs<bool>(value));
            }
        }

        protected ICollection<string> Fields = new List<string>();
        public string Validate()
        {
            string lErrorMsg = string.Empty;

            Fields.Any(field =>
           {
               lErrorMsg = (this as IDataErrorInfo)[field];
               return !string.IsNullOrEmpty(lErrorMsg);
           });

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
            IList<PropertyDescriptor> ret = new List<PropertyDescriptor>();

            ret.Add(new MyPropertyDescriptor(this, PropertyNames.Id,
                new Attribute[]
                {
                    CustomVisibleAttribute.Yes,
                    new LocalizedCategoryAttribute( "Appearance" ),
                    new LocalizedDisplayNameAttribute( "Id" ),
                    new PropertyOrderAttribute( (int)Consts.PropertyOrder.eId ),
                }));

            ret.Add(new MyPropertyDescriptor(this, PropertyNames.Name,
                new Attribute[]
                {
                    CustomVisibleAttribute.Yes,
                    new LocalizedCategoryAttribute( "Appearance" ),
                    new LocalizedDisplayNameAttribute( "Name" ),
                    new PropertyOrderAttribute( (int)Consts.PropertyOrder.eName ),
                }));

            ret.Add(new MyPropertyDescriptor(this, PropertyNames.Visible,
                new Attribute[]
                {
                    CustomVisibleAttribute.Yes,
                    new LocalizedCategoryAttribute( "Appearance" ),
                    new LocalizedDisplayNameAttribute( "Visible" ),
                    new PropertyOrderAttribute( (int)Consts.PropertyOrder.eVisible ),
                }));

            ret.Add(new MyPropertyDescriptor(this, PropertyNames.PenColor,
                new Attribute[]
                {
                    CustomVisibleAttribute.Yes,
                    new LocalizedCategoryAttribute( "Appearance" ),
                    new LocalizedDisplayNameAttribute( "LineColor" ),
                    new PropertyOrderAttribute( (int)Consts.PropertyOrder.eLineColor ),
                }));

            ret.Add(new MyPropertyDescriptor(this, PropertyNames.PenWidth,
                new Attribute[]
                {
                    CustomVisibleAttribute.Yes,
                    new LocalizedCategoryAttribute( "Appearance" ),
                    new LocalizedDisplayNameAttribute( "LineWidth" ),
                    new PropertyOrderAttribute( (int)Consts.PropertyOrder.eLineWidth ),
                    new TypeConverterAttribute( typeof( LineWidthConverter ) ),
                }));

            return ret;
        }

        public string RuntimeValue
        {
            set
            {
                cLog.Info("set runtime value:" + value);
                OnSetRuntimeValue(value);
            }
        }

        protected virtual void OnSetRuntimeValue(string value)
        {
        }

        protected IEnumerable<PropertyDescriptor> RemoveProperties(IEnumerable<PropertyDescriptor> pProperties, IEnumerable<string> pNames)
        {
            return from p in pProperties
                   where !pNames.Contains(p.Name)
                   select p;
        }

        private ISite GetSite()
        {
            IComponent lComponent = this as IComponent;

            return lComponent.Site;
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
            OnDisposed();
        }
        #endregion

        #region ICustomTypeDescriptor Members
        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        private static readonly BrowsablePropertiesConverter Converter = new BrowsablePropertiesConverter();
        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return Converter;
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        object ICustomTypeDescriptor.GetEditor(System.Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(System.Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return TypeDescriptor.GetProperties(this, true);
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            return new PropertyDescriptorCollection(GetProperties().ToArray());
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
            get { return GetFieldError(pFieldName); }
        }
        #endregion
    }
}
