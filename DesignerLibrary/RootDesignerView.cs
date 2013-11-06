using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using DesignerLibrary.Consts;
using DesignerLibrary.DrawingTools;
using DesignerLibrary.Trackers;
using System.Drawing.Drawing2D;

namespace DesignerLibrary
{
    // RootDesignerView is a simple control that will be displayed 
    // in the designer window.
    class RootDesignerView : Control
    {
        private RootDesigner m_designer;
        private IServiceProvider ServiceProvider { get; set; }

        public RootDesignerView(RootDesigner pDesigner)
        {
            m_designer = pDesigner;
            BackColor = Color.LightBlue;
            AllowDrop = true;

            ServiceProvider = pDesigner.Component.Site.Container as IServiceProvider;
        }

        private List<DrawingTool> DrawingTools = new List<DrawingTool>();

        private void AddTool(DrawingTool pTool)
        {
            DrawingTools.Add( pTool );
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

        protected override void OnPaint(PaintEventArgs pArgs)
        {
            base.OnPaint( pArgs );

            pArgs.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            DrawingTools.All( e =>
            {
                e.Paint( pArgs );
                return true;
            } );

            if (_SelectedTool != null)
                _SelectedTool.Tracker.Paint( pArgs );

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
                    Invalidate( _SelectedTool.Tracker.SurroundingRect );
                }

                _SelectedTool = value;
                if (_SelectedTool != null)
                {
                    _SelectedTool.Selected = true;
                    Invalidate( _SelectedTool.Tracker.SurroundingRect );
                }
            }
        }

        private Point DraggingPoint { get; set; }

        protected override void OnDragEnter(DragEventArgs pArgs)
        {
            base.OnDragEnter( pArgs );

            pArgs.Effect = DragDropEffects.Move;
            //if (pArgs.Data.GetDataPresent( typeof( LineTool ) ))
            //    pArgs.Effect = DragDropEffects.Move;

            Point lPoint = PointToClient( new Point( pArgs.X, pArgs.Y ) );
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
                    Point lPoint = PointToClient( new Point( pArgs.X, pArgs.Y ) );

                    Invalidate( lDrawingTool.Tracker.SurroundingRect );
                    lDrawingTool.DoDrop( new Point( lPoint.X - DraggingPoint.X, lPoint.Y - DraggingPoint.Y ) );
                    Invalidate( lDrawingTool.Tracker.SurroundingRect );

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
                    Point lLocation = PointToClient( new Point( pArgs.X, pArgs.Y ) );
                    Rectangle lRect = lTool.SurroundingRect;

                    lLocation.Offset( -lRect.Width / 2, -lRect.Height / 2 );
                    lTool.Location = lLocation;
                    AddTool( lTool );
                    Invalidate( lTool.SurroundingRect );
                }
            }
        }

        private Rectangle DragBoxFromMouseDown { get; set; }

        protected override void OnMouseDown(MouseEventArgs pArgs)
        {
            base.OnMouseDown( pArgs );

            Point lLocation = pArgs.Location;
            DrawingTool lTool = HitTest( lLocation );

            if (lTool != null)
            {
                if (!lTool.Tracker.StartResize( lLocation ))
                {
                    // on tool object.
                }
            }
            SelectedTool = lTool;

            Size lDragSize = SystemInformation.DragSize;

            lLocation.Offset( lDragSize.Width / 2, lDragSize.Height / 2 );
            DragBoxFromMouseDown = new Rectangle( lLocation, lDragSize );
            DraggingPoint = pArgs.Location;
        }

        protected override void OnMouseMove(MouseEventArgs pArgs)
        {
            base.OnMouseMove( pArgs );

            bool lRunDefault = true;
            Point lLocation = pArgs.Location;

            if (SelectedTool != null)
            {
                DrawingTracker lTracker = SelectedTool.Tracker;

                lRunDefault = false;
                // stretching on tracker.
                if (lTracker.IsResizing)
                {
                    Invalidate( lTracker.SurroundingRect );
                    lTracker.Resize( lLocation );
                    Invalidate( lTracker.SurroundingRect );
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
                    Invalidate( PickingTool.SurroundingRect );
                    PickingTool.Resize( lLocation );
                    Invalidate( PickingTool.SurroundingRect );
                }
                else 
                {
                    var lItem = GetService<IToolboxService>().GetSelectedToolboxItem();

                    if (lItem == null
                        || lItem.DisplayName == NameConsts.Pointer)
                    {
                        DrawingTools.FirstOrDefault( e =>
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
                    }
                    else
                    {
                        if (DragBoxFromMouseDown != Rectangle.Empty)
                        {
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

            Point lLocation = pArgs.Location;

            // End tracker resize operation.
            if (SelectedTool != null
                && SelectedTool.Tracker.IsResizing)
            {
                Rectangle lRect = SelectedTool.SurroundingRect;
                int lWidth = SelectedTool.Tracker.Margin + 1;     // +1 for SmoothingMode.AntiAlias

                Invalidate( InflateRect( SelectedTool.SurroundingRect, lWidth, lWidth ) );
                SelectedTool.Tracker.EndResize( lLocation );
                Invalidate( InflateRect( SelectedTool.SurroundingRect, lWidth, lWidth ) );
            }
            else if (PickingTool != null)
            {
                if (PickingTool.EndResize( lLocation ))
                {
                    AddTool( PickingTool );
                    Invalidate( PickingTool.SurroundingRect );
                    PickingTool = null;
                }
            }

            DragBoxFromMouseDown = Rectangle.Empty;
        }

        private T GetService<T>()
            where T : class
        {
            return ServiceProvider.GetService( typeof( T ) ) as T;
        }
    }
}