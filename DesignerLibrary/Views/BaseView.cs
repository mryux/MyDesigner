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
            BackColor = Color.LightGreen;

            DoubleBuffered = true;
            ResizeRedraw = true;
            AutoScrollMinSize = new Size( 1200, 800 );
            CaptionHeight = 30;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            GraphicsMapper.Instance.Initialize( this );
            _LayerSize = GraphicsMapper.Instance.TransformSize( AutoScrollMinSize );
        }

        protected virtual void OnAddTool(DrawingTool pTool)
        {
            pTool.RedrawEvent += (pSender, pArgs) =>
            {
                InvalidateRect( (pSender as DrawingTool).Tracker.SurroundingRect );
            };
        }

        protected virtual void OnLoadModel(SitePlanModel pModel)
        {
            PersistenceFactory.Instance.Import( pModel ).All( p =>
            {
                DrawingTool lTool = p.CreateDrawingTool( DesignerHost );

                p.LoadFromSitePlanModel( pModel );
                AddTool( lTool );
                return true;
            } );

            LayerDescription = pModel.Description;

            if (pModel.Width > 0)
                LayerWidth = pModel.Width;

            if (pModel.Height > 0)
                LayerHeight = pModel.Height;
        }

        protected List<DrawingTool> DrawingTools = new List<DrawingTool>();

        protected void AddTool(DrawingTool pTool)
        {
            OnAddTool( pTool );

            DrawingTools.Add( pTool );
        }

        public void Load(SitePlanModel pModel)
        {
            OnLoadModel( pModel );

            Invalidate();
        }

        public void OnPrint(PrintPageEventArgs pArgs)
        {
            PaintEventArgs lPaintArgs = new PaintEventArgs( pArgs.Graphics, pArgs.PageBounds );

            OnPaint( lPaintArgs );
        }

        private static readonly Color DesignTitleBaseColor = Color.FromArgb( 50, 50, 50 );
        private static readonly Color DesignTitleLightingColor = Color.FromArgb( 200, 200, 200 );
        protected int CaptionHeight { get; set; }

        protected virtual void PrePaint(Graphics pGraph)
        {
        }

        protected override void OnPaint(PaintEventArgs pArgs)
        {
            base.OnPaint( pArgs );

            Graphics lGraph = pArgs.Graphics;

            // in vc, there is PrepareDC in which I can set MapMode for CDC object, we don't have to Setup CDC object in OnPaint()
            // todo: find an alternative PrepareDC in C#.
            GraphicsMapper.InitGraphics( lGraph );

            Point lPt = GraphicsMapper.Instance.TransformPoint( AutoScrollPosition );
            lGraph.TranslateTransform( lPt.X, lPt.Y );

            // paint title
            Rectangle lRect = new Rectangle( Bounds.Left, Bounds.Top, AutoScrollMinSize.Width, CaptionHeight );

            lRect = GraphicsMapper.Instance.TransformRectangle( lRect );
            if (pArgs.Graphics.ClipBounds.IntersectsWith( lRect ))
            {
                Brush lBrush = new LinearGradientBrush( lRect, DesignTitleBaseColor, DesignTitleLightingColor, LinearGradientMode.Horizontal );

                lGraph.FillRectangle( lBrush, lRect );
                lRect.Inflate( -4, 0 );
                lGraph.DrawString( LayerDescription, Font, Brushes.White, lRect );
            }
            PrePaint( lGraph );

            // paint each drawingTool.
            DrawingTools.All( e =>
            {
                e.Paint( pArgs );
                return true;
            } );
        }

        protected bool FullDragMode = false;
        private Point FullDragPoint = Point.Empty;

        protected override void OnMouseDown(MouseEventArgs pArgs)
        {
            base.OnMouseDown( pArgs );

            if (FullDragMode)
            {
                FullDragPoint = pArgs.Location;
                FullDragPoint.Offset( -AutoScrollPosition.X, -AutoScrollPosition.Y );
            }
        }

        protected override void OnMouseMove(MouseEventArgs pArgs)
        {
            base.OnMouseMove( pArgs );

            if (FullDragMode)
            {
                AutoScrollPosition = new Point( FullDragPoint.X - pArgs.X, FullDragPoint.Y - pArgs.Y );
                Cursor.Current = Cursors.Hand;
            }
        }

        protected override void OnMouseUp(MouseEventArgs pArgs)
        {
            base.OnMouseUp( pArgs );

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

                Rectangle lRect = new Rectangle( Bounds.Left, Bounds.Top, Bounds.Width, CaptionHeight );

                lRect = GraphicsMapper.Instance.TransformRectangle( lRect );
                InvalidateRect( lRect );
            }
        }

        private Size _LayerSize = Size.Empty;
        protected int LayerWidth
        {
            get { return _LayerSize.Width; }
            set
            {
                _LayerSize.Width = value;
                AutoScrollMinSize = GraphicsMapper.Instance.TransformSize( _LayerSize, CoordinateSpace.Device, CoordinateSpace.Page );
            }
        }

        protected int LayerHeight
        {
            get { return _LayerSize.Height; }
            set
            {
                _LayerSize.Height = value;
                AutoScrollMinSize = GraphicsMapper.Instance.TransformSize( _LayerSize, CoordinateSpace.Device, CoordinateSpace.Page );
            }
        }

        protected DrawingTool HitTest(Point pPoint)
        {
            return DrawingTools.Reverse<DrawingTool>().FirstOrDefault( e =>
            {
                bool lRet = e.HitTest( pPoint );

                if (!lRet)
                    lRet = e.Tracker.HitTest( pPoint ) > 0;

                return lRet;
            } );
        }

        /// <summary>
        /// map pPoint to point in scroll control.
        /// </summary>
        /// <param name="pPoint"></param>
        /// <returns></returns>
        protected Point GetScrollablePoint(Point pPoint)
        {
            pPoint.Offset( -AutoScrollPosition.X, -AutoScrollPosition.Y );

            return GraphicsMapper.Instance.TransformPoint( pPoint );
        }

        /// <summary>
        /// map pRect to rectangle in current view
        /// </summary>
        /// <param name="pRect">rect in scroll control</param>
        protected void InvalidateRect(Rectangle pRect)
        {
            Rectangle lRect = GraphicsMapper.Instance.TransformRectangle( pRect, CoordinateSpace.Device, CoordinateSpace.Page );

            lRect.Offset( AutoScrollPosition.X, AutoScrollPosition.Y );
            Invalidate( lRect );
        }
    }
}