using DesignerLibrary.Attributes;
using DesignerLibrary.Consts;
using DesignerLibrary.DrawingTools;
using DesignerLibrary.Helpers;
using DesignerLibrary.Models;
using DesignerLibrary.Persistence;
using DesignerLibrary.Trackers;
using System;
using System.Collections.Generic;
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
    class DesignTimeView : BaseView, IDataErrorInfo, IMessageFilter
    {
        private IServiceProvider ServiceProvider { get; set; }
        private ISelectionService SelectionService { get; set; }

        public DesignTimeView(RootDesigner pDesigner)
        {
            InitializeComponent();

            AllowDrop = true;

            Site = pDesigner.Component.Site;
            DesignerHost = Site.Container as IDesignerHost;
            SelectionService = GetService<ISelectionService>();

            _deleteToolStripMenuItem.Click += OnDeleteTool;
            _bringToFrontToolStripMenuItem.Click += OnBringToFront;
            _bringToBackToolStripMenuItem.Click += OnBringToBack;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            _Ruler = new JointRuler();
        }

        public event EventHandler<EventArgs<bool>> DirtyEvent;

        private bool _IsDirty = false;
        public bool IsDirty
        {
            get { return _IsDirty; }
            set
            {
                _IsDirty = value;
                if (DirtyEvent != null)
                    DirtyEvent( this, new EventArgs<bool>( _IsDirty ) );
            }
        }

        protected override void PrePaint(Graphics pGraph)
        {
            int lHeight = GraphicsMapper.Instance.TransformInt( CaptionHeight );

            pGraph.TranslateTransform( 0, lHeight );
            _Ruler.Paint( pGraph );
            PaintGrid( pGraph );
            pGraph.TranslateTransform( 0, -lHeight );
        }

        protected override void OnPaint(PaintEventArgs pArgs)
        {
            base.OnPaint( pArgs );

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
                }
                SelectionService.SetSelectedComponents( new object[] { _SelectedTool ?? (object)this }, SelectionTypes.Remove );

                _SelectedTool = value;
                if (_SelectedTool != null)
                {
                    _SelectedTool.Selected = true;
                    InvalidateRect( _SelectedTool.Tracker.SurroundingRect );
                }
                SelectionService.SetSelectedComponents( new object[] { _SelectedTool ?? (object)this }, SelectionTypes.Add );
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
            pRect.Inflate( lWidth, lHeight );
            return pRect;
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
                    SelectedTool = lDrawingTool;

                    return true;
                } );

                IsDirty = true;
            }
            else
            {
                // drag ToolboxItem & drop.
                ToolboxItem lItem = GetService<IToolboxService>().DeserializeToolboxItem( pArgs.Data );
                DrawingTool lTool = lItem.CreateComponents( DesignerHost ).FirstOrDefault() as DrawingTool;

                if (lTool != null)
                {
                    lTool.CreatePersistence();
                    Point lLocation = GetScrollablePoint( PointToClient( new Point( pArgs.X, pArgs.Y ) ) );
                    Rectangle lRect = lTool.SurroundingRect;

                    lLocation.Offset( -lRect.Width / 2, -lRect.Height / 2 );
                    lTool.Location = lLocation;
                    AddTool( lTool );
                    SelectedTool = lTool;
                    InvalidateRect( lTool.SurroundingRect );
                }
            }
        }

        private Rectangle DragBoxFromMouseDown { get; set; }

        protected override void OnMouseDown(MouseEventArgs pArgs)
        {
            if (pArgs.Button == MouseButtons.Left)
            {
                Point lLocation = GetScrollablePoint( pArgs.Location );
                DrawingTool lTool = HitTest( lLocation );

                if (lTool == null)
                    base.FullDragMode = SelectedToolboxItem == null;
                else
                    lTool.Tracker.StartResize( lLocation );

                SelectedTool = lTool;

                Size lDragSize = GraphicsMapper.Instance.TransformSize( SystemInformation.DragSize );
                DragBoxFromMouseDown = new Rectangle( lLocation, lDragSize );
                DraggingPoint = lLocation;
            }

            base.OnMouseDown( pArgs );
        }

        protected override void OnMouseMove(MouseEventArgs pArgs)
        {
            base.OnMouseMove( pArgs );

            Point lLocation = GetScrollablePoint( pArgs.Location );

            switch (pArgs.Button)
            {
                case MouseButtons.Right:
                    return;

                case MouseButtons.None:
                    Cursor lCurrentCursor = Cursors.Hand;

                    if (SelectedToolboxItem == null
                        && PickingTool == null)
                    {
                        DrawingTools.Any( e =>
                        {
                            Cursor lCursor = e.Tracker.GetCursor( lLocation );
                            bool lRet = false;

                            if (lCursor != Cursors.Default)
                            {
                                lCurrentCursor = lCursor;
                                lRet = true;
                            }

                            return lRet;
                        } );
                    }
                    else
                        lCurrentCursor = Cursors.Cross;

                    Cursor.Current = lCurrentCursor;
                    break;

                case MouseButtons.Left:
                    if (FullDragMode)
                        return;

                    bool lRunDefault = true;

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
                                int dx = GraphicsMapper.Instance.TransformInt( DraggingPoint.X - SelectedTool.SurroundingRect.Left, CoordinateSpace.Device, CoordinateSpace.Page );
                                int dy = GraphicsMapper.Instance.TransformInt( DraggingPoint.Y - SelectedTool.SurroundingRect.Top, CoordinateSpace.Device, CoordinateSpace.Page );

                                using (new DragImage( SelectedTool.GetImage( LayerWidth, LayerHeight ), dx, dy ))
                                {
                                    int lIndex = DrawingTools.IndexOf( SelectedTool );

                                    DoDragDrop( lIndex.ToString(), DragDropEffects.All );
                                    DraggingPoint = Point.Empty;
                                    DragBoxFromMouseDown = Rectangle.Empty;
                                }
                            }
                        }
                        else
                            lRunDefault = true;
                    }

                    // update current Cursor.
                    if (lRunDefault)
                    {
                        if (PickingTool == null)
                        {
                            ToolboxItem lItem = SelectedToolboxItem;

                            // update cursor by hitTest on each drawing tool.
                            if (lItem != null
                                && DragBoxFromMouseDown != Rectangle.Empty)
                            {
                                // create toolbox item
                                if (!DragBoxFromMouseDown.Contains( lLocation ))
                                {
                                    PickingTool = lItem.CreateComponents( DesignerHost ).FirstOrDefault() as DrawingTool;
                                    PickingTool.CreatePersistence();
                                    PickingTool.StartResize( lLocation );
                                }
                            }
                        }
                    }
                    break;
            }

            if (PickingTool != null)
            {
                InvalidateRect( PickingTool.SurroundingRect );
                PickingTool.Resize( lLocation );
                InvalidateRect( PickingTool.SurroundingRect );
            }
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs pArgs)
        {
            base.OnGiveFeedback( pArgs );

            pArgs.UseDefaultCursors = false;
            Cursor.Current = Cursors.SizeAll;
        }

        protected override void OnMouseUp(MouseEventArgs pArgs)
        {
            base.OnMouseUp( pArgs );

            if (FullDragMode)
                FullDragMode = false;

            Point lLocation = GetScrollablePoint( pArgs.Location );

            switch (pArgs.Button)
            {
                case MouseButtons.Right:
                    SelectedTool = HitTest( lLocation );
                    if (SelectedTool != null)
                        _contextMenuStrip.Show( this, pArgs.Location );
                    break;

                case MouseButtons.Left:
                    // End tracker resize operation.
                    if (SelectedTool != null
                        && SelectedTool.Tracker.IsResizing)
                    {
                        Rectangle lRect = SelectedTool.SurroundingRect;
                        int lWidth = SelectedTool.Tracker.Margin + 1;     // +1 for SmoothingMode.AntiAlias

                        InvalidateRect( InflateRect( SelectedTool.SurroundingRect, lWidth, lWidth ) );
                        SelectedTool.Tracker.EndResize( lLocation );
                        InvalidateRect( InflateRect( SelectedTool.SurroundingRect, lWidth, lWidth ) );
                        SelectedTool = SelectedTool;    // refresh property grid
                        IsDirty = true;
                    }
                    else if (PickingTool != null)
                    {
                        if (PickingTool.EndResize( lLocation ))
                        {
                            AddTool( PickingTool );
                            SelectedTool = PickingTool;
                            InvalidateRect( PickingTool.SurroundingRect );
                            PickingTool = null;
                        }
                    }
                    break;
            }

            DragBoxFromMouseDown = Rectangle.Empty;
            DraggingPoint = Point.Empty;
        }

        protected override void OnMouseDoubleClick(MouseEventArgs pArgs)
        {
            base.OnMouseDoubleClick( pArgs );

            if (PickingTool != null)
            {
                PickingTool.Escape();
                AddTool( PickingTool );
                SelectedTool = PickingTool;
                InvalidateRect( PickingTool.SurroundingRect );
                PickingTool = null;
            }
        }

        public void Cleanup()
        {
            var lTools = DrawingTools.ToList();

            lTools.All( t =>
            {
                RemoveTool( t );
                return true;
            } );
        }

        private void RemoveTool(DrawingTool pTool)
        {
            pTool.OnRemove();
            DrawingTools.Remove( pTool );

            Rectangle lRect = pTool.Tracker.SurroundingRect;

            InvalidateRect( lRect );
            IsDirty = true;
        }
        
        protected override void OnAddTool(DrawingTool pTool)
        {
            base.OnAddTool( pTool );

            // setup pTool events
            pTool.IsDirtyEvent += (pSender, pArgs) =>
            {
                bool lIsDirty = pArgs.Data;

                if (lIsDirty)
                    IsDirty = true;
            };
            
            IsDirty = true;
        }

        protected override void OnLoadModel(SitePlanModel pModel)
        {
            base.OnLoadModel( pModel );

            LayerName = pModel.Name;
            IsDirty = false;
            SelectedTool = null;
        }

        public string Validate()
        {
            string lRet = (this as IDataErrorInfo)[PropertyNames.LayerName];

            if (string.IsNullOrEmpty( lRet ))
            {
                var lTools = from tool in DrawingTools
                            let errorMsg = tool.Validate()
                            where !string.IsNullOrEmpty( errorMsg )
                            select new Tuple<DrawingTool, string>( tool, errorMsg );

                if (lTools.Count() > 0)
                {
                    SelectedTool = lTools.First().Item1;

                    var lErrorMsgs = from t in lTools
                                     select t.Item2;

                    lRet = "-- " + string.Join( "\n-- ", lErrorMsgs );
                }
            }

            return lRet;
        }

        public void Save(SitePlanModel pModel)
        {
            var lPersistences = from tool in DrawingTools
                                select tool.Persistence;
            // DrawingTools
            pModel.Name = LayerName;
            pModel.Description = LayerDescription;
            pModel.Width = LayerWidth;
            pModel.Height = LayerHeight;
            pModel.Layout = PersistenceFactory.Instance.GetLayout( lPersistences );

            IsDirty = false;
        }

        void OnDeleteTool(object sender, EventArgs pArgs)
        {
            if (MessageBoxHelper.OKCancelMessage( Properties.Resources.Warning_Delete ) == DialogResult.OK)
            {
                RemoveTool( SelectedTool );
                SelectedTool = null;
            }
        }

        void OnBringToFront(object sender, EventArgs pArgs)
        {
            OnBringTo( tools =>
            {
                // move SelectedTool next to last overlapped tool
                int lIndex = tools.Max( t => DrawingTools.IndexOf( t ) );

                return lIndex + 1;
            } );
        }

        void OnBringToBack(object sender, EventArgs pArgs)
        {
            OnBringTo( tools =>
            {
                // move SelectedTool prior to first overlapped tool
                int lIndex = tools.Min( t => DrawingTools.IndexOf( t ) );

                return lIndex;
            } );
        }

        private void OnBringTo(Func<IEnumerable<DrawingTool>, int> pGetPosition)
        {
            var lOverlappedTools = GetOverlappedTools( SelectedTool );

            if (lOverlappedTools.Count() > 0)
            {
                DrawingTools.Remove( SelectedTool );
                DrawingTools.Insert( pGetPosition( lOverlappedTools ), SelectedTool );
                Invalidate();
                IsDirty = true;
            }
        }

        private IEnumerable<DrawingTool> GetOverlappedTools(DrawingTool pTool)
        {
            Graphics lGraph = Graphics.FromHwnd( Handle );

            return from t in DrawingTools
                   where t != pTool
                        && t.IsOverlapped( pTool, lGraph )
                   select t;
        }

        private ToolboxItem SelectedToolboxItem
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

        private JointRuler _Ruler = null;

        private bool _ShowGrid = false;
        public bool ShowGrid
        {
            get { return _ShowGrid; }
            set
            {
                _ShowGrid = value;
                Invalidate();
            }
        }

        private void PaintGrid(Graphics pGraph)
        {
            if (ShowGrid)
            {
                Pen lPen = new Pen( Color.DarkGray, 1.0f ) { DashStyle = DashStyle.Dot };
                int lOrigin = GraphicsMapper.Instance.TransformInt( _Ruler.RulerSize.Height );

                pGraph.TranslateTransform( lOrigin, lOrigin );
                for (int i = 0; i < LayerHeight; i += 50)
                    Ruler.DrawHorzLine( pGraph, lPen, 0, LayerWidth, i );

                for (int i = 0; i < LayerWidth; i += 50)
                    Ruler.DrawVertLine( pGraph, lPen, i, 0, LayerHeight );
                pGraph.TranslateTransform( -lOrigin, -lOrigin );
            }
        }

        #region IDataErrorInfo Members
        string IDataErrorInfo.Error
        {
            get { return null; }
        }

        string IDataErrorInfo.this[string pFieldName]
        {
            get
            {
                string lRet = string.Empty;

                if (pFieldName == PropertyNames.LayerName)
                {
                    if (string.IsNullOrEmpty( LayerName ))
                        lRet = Properties.Resources.Error_InvalidLayerName;
                }

                return lRet;
            }
        }
        #endregion

        private string _LayerName = string.Empty;

        [CustomVisible( true )]
        [LocalizedCategory( "Appearance" )]
        [LocalizedDisplayName( "Name" )]
        public string LayerName
        {
            get { return _LayerName; }
            set
            {
                _LayerName = value;
                IsDirty = true;
            }
        }

        [CustomVisible( true )]
        [LocalizedCategory( "Appearance" )]
        [LocalizedDisplayName( "Description" )]
        public new string LayerDescription
        {
            get { return base.LayerDescription; }
            set
            {
                base.LayerDescription = value;
                IsDirty = true;
            }
        }

        [CustomVisible( true )]
        [LocalizedCategory( "Appearance" )]
        [LocalizedDisplayName( "Width" )]
        public new int LayerWidth
        {
            get { return base.LayerWidth; }
            set
            {
                base.LayerWidth = value;
                IsDirty = true;
            }
        }

        [CustomVisible( true )]
        [LocalizedCategory( "Appearance" )]
        [LocalizedDisplayName( "Height" )]
        public new int LayerHeight
        {
            get { return base.LayerHeight; }
            set
            {
                base.LayerHeight = value;
                IsDirty = true;
            }
        }

        protected T GetService<T>()
            where T : class
        {
            return DesignerHost.GetService( typeof( T ) ) as T;
        }

        private void InitializeComponent()
        {
            _deleteToolStripMenuItem = new ToolStripMenuItem()
            {
                Size = new Size( 145, 22 ),
                Text = Properties.Resources.ContextMenu_Delete,
            };
            _bringToBackToolStripMenuItem = new ToolStripMenuItem()
            {
                Size = new Size( 145, 22 ),
                Text = Properties.Resources.ContextMenu_BringToBack,
            };
            _bringToFrontToolStripMenuItem = new ToolStripMenuItem()
            {
                Size = new Size( 145, 22 ),
                Text = Properties.Resources.ContextMenu_BringToFront,
            };

            _contextMenuStrip = new ContextMenuStrip();
            _contextMenuStrip.Items.AddRange( new ToolStripItem[]
            {
                _deleteToolStripMenuItem,
                _bringToBackToolStripMenuItem,
                _bringToFrontToolStripMenuItem
            } );
            _contextMenuStrip.Size = new Size( 146, 70 );
            Size = new Size( 623, 341 );
        }

        private ContextMenuStrip _contextMenuStrip;
        private ToolStripMenuItem _deleteToolStripMenuItem;
        private ToolStripMenuItem _bringToBackToolStripMenuItem;
        private ToolStripMenuItem _bringToFrontToolStripMenuItem;

        bool IMessageFilter.PreFilterMessage(ref Message pMsg)
        {
            bool lRet = false;

            switch (pMsg.Msg)
            {
                case WinMessageHelper.WM_KEYDOWN:
                    Keys lKeyDown = (Keys)pMsg.WParam.ToInt32();

                    switch (lKeyDown)
                    {
                        case Keys.ControlKey:
                            KeyboardHelper.Instance.IsCtrlPressing = true;
                            WinMessageHelper.Instance.PostMessage_MouseMove( this );
                            break;
                    }
                    break;

                case WinMessageHelper.WM_KEYUP:
                    Keys lKeyUp = (Keys)pMsg.WParam.ToInt32() | Control.ModifierKeys;

                    switch (lKeyUp)
                    {
                        case Keys.Delete:
                            if (SelectedTool != null)
                            {
                                _deleteToolStripMenuItem.PerformClick();
                                lRet = true;
                            }
                            break;

                        case Keys.ControlKey:
                            KeyboardHelper.Instance.IsCtrlPressing = false;
                            WinMessageHelper.Instance.PostMessage_MouseMove( this );
                            break;
                    }
                    break;
            }

            return lRet;
        }
    }
}
