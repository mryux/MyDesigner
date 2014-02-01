using DesignerLibrary.Attributes;
using DesignerLibrary.Consts;
using DesignerLibrary.DrawingTools;
using DesignerLibrary.Helpers;
using DesignerLibrary.Persistence;
using DesignerLibrary.Trackers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

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
            ServiceProvider = Site.Container as IServiceProvider;
            SelectionService = GetService<ISelectionService>();

            LayerDescription = Properties.Resources.LayerDescription;

            _deleteToolStripMenuItem.Click += OnDeleteTool;
            _bringToFrontToolStripMenuItem.Click += OnBringToFront;
            _bringToBackToolStripMenuItem.Click += OnBringToBack;
        }

        public event EventHandler<EventArgs<bool>> DirtyEvent;

        private bool _IsDirty = false;
        private bool IsDirty
        {
            get { return _IsDirty; }
            set
            {
                _IsDirty = value;
                if (DirtyEvent != null)
                    DirtyEvent( this, new EventArgs<bool>( _IsDirty ) );
            }
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

                IsDirty = true;
            }
            else
            {
                // drag ToolboxItem & drop.
                ToolboxItem lItem = GetService<IToolboxService>().DeserializeToolboxItem( pArgs.Data );
                DrawingTool lTool = lItem.CreateComponents().FirstOrDefault() as DrawingTool;

                if (lTool != null)
                {
                    lTool.Persistence = lTool.CreatePersistence();
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

                if (lTool != null)
                    lTool.Tracker.StartResize( lLocation );

                SelectedTool = lTool;

                base.FullDragMode = (lTool == null && SelectedToolboxItem == null);

                DragBoxFromMouseDown = new Rectangle( lLocation, SystemInformation.DragSize );
                DraggingPoint = lLocation;
            }

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
                                int lIndex = DrawingTools.IndexOf( SelectedTool );
                                int dx = DraggingPoint.X - SelectedTool.SurroundingRect.Left;
                                int dy = DraggingPoint.Y - SelectedTool.SurroundingRect.Top;

                                using (DragImage image = new DragImage(SelectedTool.GetImage(LayerWidth, LayerHeight), dx, dy))
                                {
                                    Cursor.Hide();
                                    DoDragDrop(lIndex.ToString(), DragDropEffects.All);
                                    DraggingPoint = Point.Empty;
                                    DragBoxFromMouseDown = Rectangle.Empty;
                                    Cursor.Show();
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
                                    PickingTool = lItem.CreateComponents().FirstOrDefault() as DrawingTool;
                                    PickingTool.Persistence = PickingTool.CreatePersistence();
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

        private void RemoveTool(DrawingTool pTool)
        {
            Rectangle lRect = pTool.Tracker.SurroundingRect;

            DrawingTools.Remove( pTool );
            InvalidateRect( lRect );
            IsDirty = true;
        }
        
        protected override void OnAddTool(DrawingTool pTool)
        {
            (pTool as IComponent).Site = Site;
            pTool.RedrawEvent += (object pSender, EventArgs pArgs)=>
            {
                InvalidateRect( (pSender as DrawingTool).Tracker.SurroundingRect );
            };
            
            IsDirty = true;
        }

        protected override void OnLoadModel(string pPath)
        {
            base.OnLoadModel( pPath );

            IsDirty = false;
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

        public void Save(string pPath)
        {
            SitePlanTools lTools = new SitePlanTools();

            // DrawingTools
            lTools.Name = LayerName;
            lTools.Description = LayerDescription;
            lTools.Width = Width;
            lTools.Height = Height; 

            XmlSerializer lSerializer = new XmlSerializer( typeof( SitePlanTools ), PersistenceTypes );

            lTools.Persistences = (from tool in DrawingTools
                            select tool.Persistence).ToArray();
            using (TextWriter writer = new StreamWriter(pPath))
            {
                lSerializer.Serialize( writer, lTools );
                writer.Flush();
            }
        }

        void OnDeleteTool(object sender, EventArgs pArgs)
        {
            if (MessageBox.Show( Properties.Resources.Warning_Delete, Properties.Resources.Caption_Warning, MessageBoxButtons.OKCancel ) == DialogResult.OK)
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
        [Category( "Appearance" )]
        [DisplayName( "Name" )]
        public string DesignerName
        {
            get { return base.LayerName; }
            set
            {
                base.LayerName = value;
                IsDirty = true;
            }
        }

        [CustomVisible( true )]
        [Category( "Appearance" )]
        [DisplayName( "Description" )]
        public string DesignerDescription
        {
            get { return base.LayerDescription; }
            set
            {
                base.LayerDescription = value;
                IsDirty = true;
            }
        }

        [CustomVisible( true )]
        [Category( "Appearance" )]
        [DisplayName( "Width" )]
        public int DesignerWidth
        {
            get { return base.LayerWidth; }
            set
            {
                base.LayerWidth = value;
                IsDirty = true;
            }
        }

        [CustomVisible( true )]
        [Category( "Appearance" )]
        [DisplayName( "Height" )]
        public int DesignerHeight
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
            return ServiceProvider.GetService( typeof( T ) ) as T;
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

            if (pMsg.Msg == WinMessages.WM_KEYUP)
            {
                Keys lKeyPressed = (Keys)pMsg.WParam.ToInt32() | Control.ModifierKeys;

                switch (lKeyPressed)
                {
                    case Keys.Delete:
                        _deleteToolStripMenuItem.PerformClick();
                        lRet = true;
                        break;
                }
            }

            return lRet;
        }
    }
}
