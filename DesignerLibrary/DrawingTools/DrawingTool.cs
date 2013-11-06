using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DesignerLibrary.Trackers;

namespace DesignerLibrary.DrawingTools
{
    abstract class DrawingTool : IComponent
    {
        protected DrawingTool()
        {
            Pen = new Pen( Color.Black, 2.0f );
        }

        public Pen Pen { get; private set; }
        public DrawingTracker Tracker { get; protected set; }

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
    }
}
