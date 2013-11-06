using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DesignerLibrary.DrawingTools;

namespace DesignerLibrary.Trackers
{
    abstract class DrawingTracker
    {
        protected DrawingTracker(DrawingTool pTool)
        {
            DrawingTool = pTool;

            _Pen = new Pen( Brushes.Blue, 2.0f );
            _Pen.DashPattern = new float[] { 4.0F, 2.0F, 1.0F, 3.0F };
        }

        public bool IsResizing { get; protected set; }

        protected DrawingTool DrawingTool { get; set; }

        protected abstract Dictionary<int, Point> TrackerPoints { get; }
        protected Dictionary<int, Rectangle> TrackerRects
        {
            get
            {
                Dictionary<int, Rectangle> lRet = new Dictionary<int, Rectangle>();

                TrackerPoints.All( e =>
                {
                    lRet.Add( e.Key, GetTrackerRect( e.Value ) );
                    return true;
                } );

                return lRet;
            }
        }

        private Pen _Pen;
        public Pen Pen { get { return _Pen; } }
        
        private static readonly int sMargin = 2;
        public int Margin
        {
            get { return (int)( DrawingTool.Pen.Width / 2 + sMargin ); }
        }

        protected abstract void OnResizePaint(PaintEventArgs pArgs);
        protected abstract Cursor GetHoverCursor(Point pPoint);
        protected abstract void OnStartResize(Point pPoint);
        protected abstract void OnResize(Point pPoint);
        protected abstract void OnEndResize();
        protected abstract Rectangle GetSurroundingRect();

        public void Paint(PaintEventArgs pArgs)
        {
            if (IsResizing)
                OnResizePaint( pArgs );

            // paint involved Tracker rects.
            var lRects = from rect in TrackerRects.Values
                         where pArgs.ClipRectangle.Contains( rect )
                         select rect;

            if (lRects.Count() > 0)
                pArgs.Graphics.FillRectangles( Brushes.Black, lRects.ToArray() );
        }
                
        /// <summary>
        /// get specific Cursor with specified pPoint of mouse position.
        /// </summary>
        /// <param name="pPoint"></param>
        /// <returns></returns>
        public Cursor GetCursor(Point pPoint)
        {
            Cursor lRet = Cursors.Default;

            if (DrawingTool.HitTest( pPoint ))
                lRet = Cursors.SizeAll;
            else
                lRet = GetHoverCursor( pPoint );

            return lRet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pPoint"></param>
        /// <returns></returns>
        public int HitTest(Point pPoint)
        {
            var lPair = TrackerRects.FirstOrDefault( e => e.Value.Contains( pPoint ) );
            int lRet = 0;

            if (lPair.Value != Rectangle.Empty)
                lRet = lPair.Key;

            return lRet;
        }

        public Rectangle GetTrackerRect(Point pPoint)
        {
            Rectangle lRect = new Rectangle( pPoint.X, pPoint.Y, 0, 0 );

            lRect.Inflate( Margin, Margin );
            return lRect;
        }

        protected int MovingPointIndex = 0;

        public bool StartResize(Point pPoint)
        {
            MovingPointIndex = HitTest( pPoint );

            if (MovingPointIndex > 0)
            {
                IsResizing = true;
                OnStartResize( pPoint );
            }            

            return IsResizing;
        }

        public void Resize(Point pPoint)
        {
            OnResize( pPoint );
        }

        public void EndResize(Point pPoint)
        {
            OnEndResize();
            IsResizing = false;
            MovingPointIndex = 0;
        }

        public Rectangle SurroundingRect
        {
            get
            {
                Rectangle lRet = GetSurroundingRect();
                int lMargin = Margin + 1;   // +1 for SmoothingMode.AntiAlias

                lRet.Inflate( lMargin, lMargin );
                return lRet;
            }
        }
    }
}
