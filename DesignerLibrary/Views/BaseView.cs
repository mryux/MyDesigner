using DesignerLibrary.Constants;
using DesignerLibrary.DrawingTools;
using DesignerLibrary.Helpers;
using DesignerLibrary.Models;
using DesignerLibrary.Persistence;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace DesignerLibrary.Views
{
    abstract class BaseView : ScrollableControl
    {
        protected IDesignerHost DesignerHost { get; set; }
        protected BaseView()
        {
            BackColor = Color.White;

            DoubleBuffered = true;
            ResizeRedraw = true;
            AutoScrollMinSize = new Size(ViewConsts.Width, ViewConsts.Height);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            GraphicsMapper.Instance.Initialize(this);
            _LayerSize = GraphicsMapper.Instance.TransformSize(AutoScrollMinSize);
        }

        protected virtual void OnAddTool(DrawingTool tool)
        {
            tool.RedrawEvent += (sender, args) =>
            {
                InvalidateRect((sender as DrawingTool).Tracker.SurroundingRect);
            };
        }

        protected virtual void OnLoadModel(DesignerModel model)
        {
            PersistenceFactory.Instance.Import(model).All(persist =>
            {
                DrawingTool tool = persist.CreateDrawingTool(DesignerHost);

                AddTool(tool);
                return true;
            });

            LayerDescription = model.Description;

            if (model.Width > 0)
                LayerWidth = model.Width;

            if (model.Height > 0)
                LayerHeight = model.Height;
        }

        protected List<DrawingTool> DrawingTools = new List<DrawingTool>();

        protected void AddTool(DrawingTool tool)
        {
            OnAddTool(tool);

            DrawingTools.Add(tool);
        }

        public void Load(DesignerModel model)
        {
            OnLoadModel(model);

            Invalidate();
        }

        public void OnPrint(PrintPageEventArgs args)
        {
            PaintEventArgs paintArgs = new PaintEventArgs(args.Graphics, args.PageBounds);

            OnPaint(paintArgs);
        }

        protected virtual void PrePaint(PaintEventArgs args)
        {
        }

        protected override void OnPaint(PaintEventArgs args)
        {
            base.OnPaint(args);

            Graphics graph = args.Graphics;

            // in vc, there is PrepareDC in which I can set MapMode for CDC object, we don't have to Setup CDC object in OnPaint()
            // todo: find an alternative PrepareDC in C#.
            GraphicsMapper.InitGraphics(graph);

            Point pt = GraphicsMapper.Instance.TransformPoint(AutoScrollPosition);
            graph.TranslateTransform(pt.X, pt.Y);

            PrePaint(args);

            // paint each drawingTool.
            DrawingTools.All(tool =>
            {
                tool.Paint(args);
                return true;
            });
        }

        protected bool FullDragMode = false;
        private Point FullDragPoint = Point.Empty;

        protected override void OnMouseDown(MouseEventArgs args)
        {
            base.OnMouseDown(args);

            if (FullDragMode)
            {
                FullDragPoint = args.Location;
                FullDragPoint.Offset(-AutoScrollPosition.X, -AutoScrollPosition.Y);
            }
        }

        protected override void OnMouseMove(MouseEventArgs args)
        {
            base.OnMouseMove(args);

            if (FullDragMode)
            {
                AutoScrollPosition = new Point(FullDragPoint.X - args.X, FullDragPoint.Y - args.Y);
                Cursor.Current = Cursors.Hand;
            }
        }

        protected override void OnMouseUp(MouseEventArgs args)
        {
            base.OnMouseUp(args);

            if (FullDragMode)
            {
                FullDragPoint = Point.Empty;
                Cursor.Current = Cursors.Default;
            }
        }

        private string _Description = string.Empty;
        protected string LayerDescription
        {
            get { return _Description; }
            set
            {
                _Description = value;

                //Rectangle rect = new Rectangle(Bounds.Left, Bounds.Top, Bounds.Width, CaptionHeight);

                //rect = GraphicsMapper.Instance.TransformRectangle(rect);
                //InvalidateRect(rect);
            }
        }

        private Size _LayerSize = Size.Empty;
        protected int LayerWidth
        {
            get { return _LayerSize.Width; }
            set
            {
                _LayerSize.Width = value;
                AutoScrollMinSize = GraphicsMapper.Instance.TransformSize(_LayerSize, CoordinateSpace.Device, CoordinateSpace.Page);
            }
        }

        protected int LayerHeight
        {
            get { return _LayerSize.Height; }
            set
            {
                _LayerSize.Height = value;
                AutoScrollMinSize = GraphicsMapper.Instance.TransformSize(_LayerSize, CoordinateSpace.Device, CoordinateSpace.Page);
            }
        }

        protected DrawingTool HitTest(Point point)
        {
            return DrawingTools.Reverse<DrawingTool>().FirstOrDefault(e =>
           {
               bool lRet = e.HitTest(point);

               if (!lRet)
                   lRet = e.Tracker.HitTest(point) > 0;

               return lRet;
           });
        }

        /// <summary>
        /// map pPoint to point in scroll control.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected Point GetScrollablePoint(Point point)
        {
            point.Offset(-AutoScrollPosition.X, -AutoScrollPosition.Y);

            return GraphicsMapper.Instance.TransformPoint(point);
        }

        /// <summary>
        /// map pRect to rectangle in current view
        /// </summary>
        /// <param name="rect">rect in scroll control</param>
        protected void InvalidateRect(Rectangle rect)
        {
            Rectangle lRect = GraphicsMapper.Instance.TransformRectangle(rect, CoordinateSpace.Device, CoordinateSpace.Page);

            lRect.Offset(AutoScrollPosition.X, AutoScrollPosition.Y);
            Invalidate(lRect);
        }
    }
}