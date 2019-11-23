using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DesignerLibrary.DrawingTools;
using DesignerLibrary.Helpers;

namespace DesignerLibrary.Trackers
{
    abstract class DrawingTracker
    {
        protected DrawingTracker(DrawingTool tool)
        {
            DrawingTool = tool;

            _Pen = new Pen(Brushes.Blue, 2.0f);
            _Pen.DashPattern = new float[] { 4.0F, 2.0F, 1.0F, 3.0F };

            _Margin = GraphicsMapper.Instance.TransformInt(2);
            _MarginAmendForAntiAlias = GraphicsMapper.Instance.TransformInt(1);
        }

        public bool IsResizing { get; protected set; }
        public TrackerAdjust Adjust { get; set; }

        protected DrawingTool DrawingTool { get; set; }

        protected abstract Dictionary<int, Point> TrackerPoints { get; }
        protected Dictionary<int, Rectangle> TrackerRects
        {
            get
            {
                Dictionary<int, Rectangle> lRet = new Dictionary<int, Rectangle>();

                TrackerPoints.All(e =>
               {
                   lRet.Add(e.Key, GetTrackerRect(e.Value));
                   return true;
               });

                return lRet;
            }
        }

        private Pen _Pen;
        public Pen Pen { get { return _Pen; } }

        private int _Margin = 0;
        public int Margin
        {
            get { return (int)(DrawingTool.Pen.Width / 2 + _Margin); }
        }

        protected abstract void OnResizePaint(PaintEventArgs args);
        protected abstract Cursor GetHoverCursor(Point point);
        protected abstract void OnStartResize(Point point);
        protected abstract void OnResize(Point point);
        protected abstract void OnEndResize();
        protected abstract Rectangle GetSurroundingRect();

        public void Paint(PaintEventArgs args)
        {
            if (IsResizing)
                OnResizePaint(args);
            else
            {
                // paint involved Tracker rects.
                var rects = from rect in TrackerRects.Values
                            where args.Graphics.ClipBounds.Contains(rect)
                            select rect;

                if (rects.Count() > 0)
                    args.Graphics.FillRectangles(Brushes.Black, rects.ToArray());
            }
        }

        /// <summary>
        /// get specific Cursor with specified pPoint of mouse position.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Cursor GetCursor(Point point)
        {
            Cursor lRet = Cursors.Default;

            if (DrawingTool.HitTest(point))
                lRet = Cursors.SizeAll;
            else
                lRet = GetHoverCursor(point);

            return lRet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int HitTest(Point point)
        {
            var lPair = TrackerRects.FirstOrDefault(e => e.Value.Contains(point));
            int lRet = 0;

            if (lPair.Value != Rectangle.Empty)
                lRet = lPair.Key;

            return lRet;
        }

        public Rectangle GetTrackerRect(Point point)
        {
            Rectangle lRect = new Rectangle(point.X, point.Y, 0, 0);

            lRect.Inflate(Margin, Margin);
            return lRect;
        }

        public static Point GetCenter(Rectangle rect)
        {
            Point lRet = rect.Location;

            lRet.Offset(rect.Width / 2, rect.Height / 2);
            return lRet;
        }

        protected int MovingPointIndex = 0;

        public bool StartResize(Point point)
        {
            MovingPointIndex = HitTest(point);

            if (MovingPointIndex > 0)
            {
                IsResizing = true;
                OnStartResize(point);
            }

            return IsResizing;
        }

        public void Resize(Point point)
        {
            OnResize(point);
        }

        public void EndResize(Point point)
        {
            OnEndResize();
            IsResizing = false;
            MovingPointIndex = 0;
        }

        private int _MarginAmendForAntiAlias = 0;

        public Rectangle SurroundingRect
        {
            get
            {
                Rectangle lRet = GetSurroundingRect();
                int lMargin = Margin + _MarginAmendForAntiAlias;

                lRet.Inflate(lMargin, lMargin);
                return lRet;
            }
        }
    }
}
