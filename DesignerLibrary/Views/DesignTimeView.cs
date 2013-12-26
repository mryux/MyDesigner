﻿using DesignerLibrary.Attributes;
using DesignerLibrary.Constants;
using DesignerLibrary.DrawingTools;
using DesignerLibrary.Helpers;
using DesignerLibrary.Trackers;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace DesignerLibrary.Views
{
    [TypeConverter( typeof( BrowsablePropertiesConverter ) )]
    class DesignTimeView : BaseView
    {
        private IServiceProvider ServiceProvider { get; set; }
        private ISelectionService SelectionService { get; set; }

        public DesignTimeView(RootDesigner pDesigner)
        {
            AllowDrop = true;

            Site = pDesigner.Component.Site;
            ServiceProvider = Site.Container as IServiceProvider;
            SelectionService = GetService<ISelectionService>();

            CaptionHeight = 30;
            LayerDescription = Properties.Resources.LayerDescription;
        }

        private DrawingTool HitTest(Point pPoint)
        {
            return DrawingTools.FirstOrDefault( e =>
            {
                bool lRet = e.HitTest( pPoint );

                if (!lRet)
                    lRet = e.Tracker.HitTest( pPoint ) > 0;

                return lRet;
            } );
        }

        private static readonly Color DesignTitleBaseColor = Color.FromArgb( 50, 50, 50 );
        private static readonly Color DesignTitleLightingColor = Color.FromArgb( 200, 200, 200 );
        private int CaptionHeight { get; set; }

        protected override void OnPaint(PaintEventArgs pArgs)
        {
            base.OnPaint( pArgs );

            Graphics lGraph = pArgs.Graphics;

            // paint title
            Rectangle lRect = new Rectangle( Bounds.Left, Bounds.Top, AutoScrollMinSize.Width, CaptionHeight );

            if (pArgs.ClipRectangle.IntersectsWith( lRect ))
            {
                Brush lBrush = new LinearGradientBrush( lRect, DesignTitleBaseColor, DesignTitleLightingColor, LinearGradientMode.Horizontal );

                lGraph.FillRectangle( lBrush, lRect );
                lRect.Inflate( -4, 0 );
                lGraph.DrawString( LayerDescription, Font, Brushes.White, lRect );
            }

            // paint selected tool.
            if (_SelectedTool != null)
                _SelectedTool.Tracker.Paint( pArgs );

            // paint picking tool.
            if (PickingTool != null)
                PickingTool.Paint( pArgs );
        }

        private DrawingTool PickingTool { get; set; }

        private DrawingTool _SelectedTool = null;
        private DrawingTool SelectedTool
        {
            get { return _SelectedTool; }
            set
            {
                if (_SelectedTool != null)
                {
                    _SelectedTool.Selected = false;
                    InvalidateRect( _SelectedTool.Tracker.SurroundingRect );

                    SelectionService.SetSelectedComponents( new object[] { _SelectedTool }, SelectionTypes.Remove );
                }

                _SelectedTool = value;
                if (_SelectedTool != null)
                {
                    _SelectedTool.Selected = true;
                    InvalidateRect( _SelectedTool.Tracker.SurroundingRect );

                    SelectionService.SetSelectedComponents( new object[] { _SelectedTool }, SelectionTypes.Add );
                }
            }
        }

        private Point DraggingPoint { get; set; }

        protected override void OnDragEnter(DragEventArgs pArgs)
        {
            base.OnDragEnter( pArgs );

            pArgs.Effect = DragDropEffects.Move;

            Point lPoint = PointToClient( new Point( pArgs.X, pArgs.Y ) );
            //if (lPoint.Y < CaptionHeight)
            //    pArgs.Effect = DragDropEffects.None;

            //if (pArgs.Data.GetDataPresent( typeof( LineTool ) ))
            //    pArgs.Effect = DragDropEffects.Move;
        }

        private AutoScrollTimer AutoScrollTimer = DummyTimer.Dummy;

        protected override void OnDragOver(DragEventArgs pArgs)
        {
            base.OnDragOver( pArgs );

            AutoScrollTimer lNewTimer = null;
            Point lLocation = PointToClient( new Point( pArgs.X, pArgs.Y ) );
            int lScrollBarWidth = VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0;
            int lScrollBarHeight = HorizontalScroll.Visible ? SystemInformation.HorizontalScrollBarHeight : 0;
            int lAutoScrollMargin = 20;

            if (lLocation.X < lAutoScrollMargin)
                lNewTimer = new AutoScrollLeftTimer( lLocation, this );
            else if (Width - lScrollBarWidth - lLocation.X < lAutoScrollMargin)
                lNewTimer = new AutoScrollRightTimer( lLocation, this );
            else if (lLocation.Y < lAutoScrollMargin)
                lNewTimer = new AutoScrollUpTimer( lLocation, this );
            else if (Height - lScrollBarHeight - lLocation.Y < lAutoScrollMargin)
                lNewTimer = new AutoScrollDownTimer( lLocation, this );

            AutoScrollTimer.AutoScrollRedir lRedir = AutoScrollTimer.AutoScrollRedir.eNone;

            if (lNewTimer != null)
                lRedir = AutoScrollTimer.Redir( lNewTimer );
            else
                lRedir = AutoScrollTimer.Redir( lLocation );

            if (lRedir.HasFlag( AutoScrollTimer.AutoScrollRedir.eStop ))
            {
                AutoScrollTimer.Stop();
                AutoScrollTimer = DummyTimer.Dummy;
            }

            if (lRedir.HasFlag( AutoScrollTimer.AutoScrollRedir.eStartNew ))
            {
                lNewTimer.Start();
                AutoScrollTimer = lNewTimer;
            }
        }

        private Rectangle InflateRect(Rectangle pRect, int lWidth, int lHeight)
        {
            Rectangle lRect = pRect;

            lRect.Inflate( lWidth, lHeight );
            return lRect;
        }

        protected override void OnDragDrop(DragEventArgs pArgs)
        {
            base.OnDragDrop( pArgs );

            // drag DrawingTool & drop.
            if (DraggingPoint != Point.Empty)
            {
                string lObj = pArgs.Data.GetData( typeof( string ) ) as string;

                lObj.Split( ',' ).All( e =>
                {
                    int lIndex = Convert.ToInt32( e );
                    DrawingTool lDrawingTool = DrawingTools[lIndex];
                    Point lPoint = GetScrollablePoint( PointToClient( new Point( pArgs.X, pArgs.Y ) ) );

                    InvalidateRect( lDrawingTool.Tracker.SurroundingRect );
                    lDrawingTool.DoDrop( new Point( lPoint.X - DraggingPoint.X, lPoint.Y - DraggingPoint.Y ) );
                    InvalidateRect( lDrawingTool.Tracker.SurroundingRect );

                    return true;
                } );
            }
            else
            {
                // drag ToolboxItem & drop.
                ToolboxItem lItem = GetService<IToolboxService>().DeserializeToolboxItem( pArgs.Data );
                DrawingTool lTool = lItem.CreateComponents().FirstOrDefault() as DrawingTool;

                if (lTool != null)
                {
                    Point lLocation = GetScrollablePoint( PointToClient( new Point( pArgs.X, pArgs.Y ) ) );
                    Rectangle lRect = lTool.SurroundingRect;

                    lLocation.Offset( -lRect.Width / 2, -lRect.Height / 2 );
                    lTool.Location = lLocation;
                    AddTool( lTool );
                    InvalidateRect( lTool.SurroundingRect );
                }
            }
        }

        private Rectangle DragBoxFromMouseDown { get; set; }

        protected override void OnMouseDown(MouseEventArgs pArgs)
        {
            Point lLocation = GetScrollablePoint( pArgs.Location );
            DrawingTool lTool = HitTest( lLocation );

            if (lTool != null)
                lTool.Tracker.StartResize( lLocation );

            if (lTool != null)
                SelectionService.SetSelectedComponents( new object[] { this }, SelectionTypes.Remove );
            SelectedTool = lTool;
            if (lTool == null)
                SelectionService.SetSelectedComponents( new object[] { this }, SelectionTypes.Add );

            base.FullDragMode = (lTool == null && SelectedToolboxItem == null);
            Size lDragSize = SystemInformation.DragSize;

            lLocation.Offset( lDragSize.Width / 2, lDragSize.Height / 2 );
            DragBoxFromMouseDown = new Rectangle( lLocation, lDragSize );
            DraggingPoint = lLocation;

            base.OnMouseDown( pArgs );
        }

        ToolboxItem SelectedToolboxItem
        {
            get
            {
                var lItem = GetService<IToolboxService>().GetSelectedToolboxItem();
                ToolboxItem lRet = null;

                if (lItem != null
                    && lItem.TypeName != NameConsts.Pointer)
                {
                    lRet = lItem;
                }

                return lRet;
            }
        }

        protected override void OnMouseMove(MouseEventArgs pArgs)
        {
            base.OnMouseMove( pArgs );

            if (FullDragMode)
                return;

            bool lRunDefault = true;
            Point lLocation = GetScrollablePoint( pArgs.Location );

            if (SelectedTool != null)
            {
                DrawingTracker lTracker = SelectedTool.Tracker;

                lRunDefault = false;
                // stretching on tracker.
                if (lTracker.IsResizing)
                {
                    InvalidateRect( lTracker.SurroundingRect );
                    lTracker.Resize( lLocation );
                    InvalidateRect( lTracker.SurroundingRect );
                }
                else if (DragBoxFromMouseDown != Rectangle.Empty)
                {
                    // start DragDrop on selected tool object.
                    if (!DragBoxFromMouseDown.Contains( lLocation ))
                    {
                        int lIndex = DrawingTools.IndexOf( SelectedTool );

                        DoDragDrop( lIndex.ToString(), DragDropEffects.All );
                        DraggingPoint = Point.Empty;
                        DragBoxFromMouseDown = Rectangle.Empty;
                    }
                }
                else
                    lRunDefault = true;
            }

            // update current Cursor.
            if (lRunDefault)
            {
                if (PickingTool != null)
                {
                    InvalidateRect( PickingTool.SurroundingRect );
                    PickingTool.Resize( lLocation );
                    InvalidateRect( PickingTool.SurroundingRect );
                }
                else
                {
                    ToolboxItem lItem = SelectedToolboxItem;

                    // update cursor by hitTest on each drawing tool.
                    if (lItem == null)
                    {
                        var lHittedTool = DrawingTools.FirstOrDefault( e =>
                        {
                            Cursor lCursor = e.Tracker.GetCursor( lLocation );
                            bool lRet = false;

                            if (lCursor != Cursors.Default)
                            {
                                Cursor.Current = lCursor;
                                lRet = true;
                            }

                            return lRet;
                        } );

                        if (lHittedTool == null)
                            Cursor.Current = Cursors.Hand;
                    }
                    else
                    {
                        if (DragBoxFromMouseDown != Rectangle.Empty)
                        {
                            // create toolbox item
                            if (!DragBoxFromMouseDown.Contains( lLocation ))
                            {
                                PickingTool = lItem.CreateComponents().FirstOrDefault() as DrawingTool;
                                PickingTool.StartResize( lLocation );
                            }
                        }
                        else
                            Cursor.Current = Cursors.Cross;
                    }
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs pArgs)
        {
            base.OnMouseUp( pArgs );

            if (FullDragMode)
                FullDragMode = false;

            Point lLocation = GetScrollablePoint( pArgs.Location );

            // End tracker resize operation.
            if (SelectedTool != null
                && SelectedTool.Tracker.IsResizing)
            {
                Rectangle lRect = SelectedTool.SurroundingRect;
                int lWidth = SelectedTool.Tracker.Margin + 1;     // +1 for SmoothingMode.AntiAlias

                InvalidateRect( InflateRect( SelectedTool.SurroundingRect, lWidth, lWidth ) );
                SelectedTool.Tracker.EndResize( lLocation );
                InvalidateRect( InflateRect( SelectedTool.SurroundingRect, lWidth, lWidth ) );
            }
            else if (PickingTool != null)
            {
                if (PickingTool.EndResize( lLocation ))
                {
                    AddTool( PickingTool );
                    InvalidateRect( PickingTool.SurroundingRect );
                    PickingTool = null;
                }
            }

            DragBoxFromMouseDown = Rectangle.Empty;
            DraggingPoint = Point.Empty;
        }
        
        protected override void OnAddTool(DrawingTool pTool)
        {
            (pTool as IComponent).Site = Site;
            pTool.RedrawEvent += (object pSender, EventArgs pArgs)=>
            {
                InvalidateRect( (pSender as DrawingTool).Tracker.SurroundingRect );
            };
        }

        [CustomVisible( true )]
        [Category( "Appearance" )]
        [DisplayName( "Name" )]
        [PropertyOrder(1)]
        public string LayerName
        {
            get;
            set;
        }

        private string _LayerDescription = string.Empty;
        [CustomVisible( true )]
        [Category( "Appearance" )]
        [DisplayName( "Description" )]
        [PropertyOrder(2)]
        public string LayerDescription
        {
            get { return _LayerDescription; }
            set
            {
                _LayerDescription = value;

                Rectangle lRect = new Rectangle( Bounds.Left, Bounds.Top, Bounds.Width, CaptionHeight );
                InvalidateRect( lRect );
            }
        }

        [CustomVisible( true )]
        [Category( "Appearance" )]
        [DisplayName( "Width" )]
        [PropertyOrder(5)]
        public int LayerWidth
        {
            get { return AutoScrollMinSize.Width; }
            set
            {
                AutoScrollMinSize = new Size( value, AutoScrollMinSize.Height );
            }
        }

        [CustomVisible( true )]
        [Category( "Appearance" )]
        [DisplayName( "Height" )]
        [PropertyOrder(6)]
        public int LayerHeight
        {
            get { return AutoScrollMinSize.Height; }
            set
            {
                AutoScrollMinSize = new Size( AutoScrollMinSize.Width, value );
            }
        }

        /// <summary>
        /// map pPoint to point in scroll control.
        /// </summary>
        /// <param name="pPoint"></param>
        /// <returns></returns>
        private Point GetScrollablePoint(Point pPoint)
        {
            Point lPoint = pPoint;

            lPoint.Offset( -AutoScrollPosition.X, -AutoScrollPosition.Y );
            return lPoint;
        }
        
        /// <summary>
        /// map pRect to rectangle in current view
        /// </summary>
        /// <param name="pRect">rect in scroll control</param>
        private void InvalidateRect(Rectangle pRect)
        {
            Rectangle lRect = pRect;

            lRect.Offset( AutoScrollPosition.X, AutoScrollPosition.Y );
            Invalidate( lRect );
        }
        
        protected T GetService<T>()
            where T : class
        {
            return ServiceProvider.GetService( typeof( T ) ) as T;
        }
    }
}
