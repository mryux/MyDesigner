using DesignerLibrary.Trackers;
using System.Drawing;
using System.Windows.Forms;

namespace DesignerLibrary.DrawingTools
{
    abstract class BaseTool
    {
        protected BaseTool()
        {
        }

        private bool _Selected = false;
        public bool Selected
        {
            get { return _Selected; }
            set
            {
                _Selected = value;
            }
        }

        public DrawingTracker Tracker { get; protected set; }

        protected abstract void DoPaint(PaintEventArgs pArgs);
        protected abstract Rectangle GetSurroundingRect();
        protected abstract void OnLocationChanged(Point pOffset);
        protected abstract bool OnHitTest(Point pPoint);
        protected abstract void OnStartResize(Point pPoint);
        protected abstract void OnResize(Point pPoint);
        protected abstract bool OnEndResize(Point pPoint);

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

        public void Paint(PaintEventArgs pArgs)
        {
            DoPaint( pArgs );
        }

        public bool HitTest(Point pPoint)
        {
            bool lRet = false;

            if (SurroundingRect.Contains( pPoint ))
                lRet = OnHitTest( pPoint );

            return lRet;
        }

        public Rectangle SurroundingRect
        {
            get { return GetSurroundingRect(); }
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
    }
}
