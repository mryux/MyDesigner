using DesignerLibrary.Attributes;
using DesignerLibrary.Constants;
using DesignerLibrary.Consts;
using DesignerLibrary.DrawingTools;
using DesignerLibrary.Helpers;
using DesignerLibrary.Models;
using DesignerLibrary.Persistence;
using DesignerLibrary.Trackers;
using DesignerLibrary.Views.Rulers;
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
    [TypeConverter(typeof(BrowsablePropertiesConverter))]
    class DesignTimeView : BaseView, IDataErrorInfo, IMessageFilter
    {
        private ISelectionService SelectionService { get; set; }

        public DesignTimeView(RootDesigner designer)
        {
            InitializeComponent();

            AllowDrop = true;

            Site = designer.Component.Site;
            DesignerHost = Site.Container as IDesignerHost;
            SelectionService = GetService<ISelectionService>();

            _deleteToolStripMenuItem.Click += OnDeleteTool;
            _bringToFrontToolStripMenuItem.Click += OnBringToFront;
            _bringToBackToolStripMenuItem.Click += OnBringToBack;
            //CaptionHeight = 30;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            Ruler = new JointRuler();
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
                    DirtyEvent(this, new EventArgs<bool>(_IsDirty));
            }
        }

        protected override void PrePaint(PaintEventArgs args)
        {
            Graphics graph = args.Graphics;

            Ruler.Paint(graph);
            PaintGrid(graph);
        }

        //private static readonly Color DesignTitleBaseColor = Color.FromArgb(50, 50, 50);
        //private static readonly Color DesignTitleLightingColor = Color.FromArgb(200, 200, 200);
        //protected int CaptionHeight { get; set; }

        //private void PaintTitle(PaintEventArgs args)
        //{
        //    Graphics graph = args.Graphics;
        //    Rectangle rect = new Rectangle(Bounds.Left, Bounds.Top, AutoScrollMinSize.Width, CaptionHeight);

        //    rect = GraphicsMapper.Instance.TransformRectangle(rect);
        //    //if (args.Graphics.ClipBounds.IntersectsWith(rect))
        //    {
        //        Brush brush = new LinearGradientBrush(rect, DesignTitleBaseColor, DesignTitleLightingColor, LinearGradientMode.Horizontal);

        //        graph.FillRectangle(brush, rect);
        //        rect.Inflate(-4, 0);
        //        graph.DrawString(LayerDescription, Font, Brushes.Black, rect);
        //    }
        //}

        protected override void OnPaint(PaintEventArgs args)
        {
            base.OnPaint(args);

            // paint selected tool.
            if (_SelectedTool != null)
                _SelectedTool.Tracker.Paint(args);

            // paint picking tool.
            if (PickingTool != null)
                PickingTool.Paint(args);
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
                    InvalidateRect(_SelectedTool.Tracker.SurroundingRect);
                }
                SelectionService.SetSelectedComponents(new object[] { _SelectedTool ?? (object)this }, SelectionTypes.Remove);

                _SelectedTool = value;
                if (_SelectedTool != null)
                {
                    _SelectedTool.Selected = true;
                    InvalidateRect(_SelectedTool.Tracker.SurroundingRect);
                }
                SelectionService.SetSelectedComponents(new object[] { _SelectedTool ?? (object)this }, SelectionTypes.Add);
            }
        }

        private Point DraggingPoint { get; set; }

        protected override void OnDragEnter(DragEventArgs pArgs)
        {
            base.OnDragEnter(pArgs);

            pArgs.Effect = DragDropEffects.Move;

            Point lPoint = PointToClient(new Point(pArgs.X, pArgs.Y));
            //if (lPoint.Y < CaptionHeight)
            //    pArgs.Effect = DragDropEffects.None;

            //if (pArgs.Data.GetDataPresent( typeof( LineTool ) ))
            //    pArgs.Effect = DragDropEffects.Move;
        }

        private AutoScrollTimer AutoScrollTimer = DummyTimer.Dummy;

        protected override void OnDragOver(DragEventArgs pArgs)
        {
            base.OnDragOver(pArgs);

            AutoScrollTimer lNewTimer = null;
            Point lLocation = PointToClient(new Point(pArgs.X, pArgs.Y));
            int lScrollBarWidth = VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0;
            int lScrollBarHeight = HorizontalScroll.Visible ? SystemInformation.HorizontalScrollBarHeight : 0;
            int lAutoScrollMargin = 20;

            if (lLocation.X < lAutoScrollMargin)
                lNewTimer = new AutoScrollLeftTimer(lLocation, this);
            else if (Width - lScrollBarWidth - lLocation.X < lAutoScrollMargin)
                lNewTimer = new AutoScrollRightTimer(lLocation, this);
            else if (lLocation.Y < lAutoScrollMargin)
                lNewTimer = new AutoScrollUpTimer(lLocation, this);
            else if (Height - lScrollBarHeight - lLocation.Y < lAutoScrollMargin)
                lNewTimer = new AutoScrollDownTimer(lLocation, this);

            AutoScrollTimer.AutoScrollRedir lRedir = AutoScrollTimer.AutoScrollRedir.eNone;

            if (lNewTimer != null)
                lRedir = AutoScrollTimer.Redir(lNewTimer);
            else
                lRedir = AutoScrollTimer.Redir(lLocation);

            if (lRedir.HasFlag(AutoScrollTimer.AutoScrollRedir.eStop))
            {
                AutoScrollTimer.Stop();
                AutoScrollTimer = DummyTimer.Dummy;
            }

            if (lRedir.HasFlag(AutoScrollTimer.AutoScrollRedir.eStartNew))
            {
                lNewTimer.Start();
                AutoScrollTimer = lNewTimer;
            }
        }

        private Rectangle InflateRect(Rectangle pRect, int lWidth, int lHeight)
        {
            pRect.Inflate(lWidth, lHeight);
            return pRect;
        }

        protected override void OnDragDrop(DragEventArgs pArgs)
        {
            base.OnDragDrop(pArgs);

            // drag DrawingTool & drop.
            if (DraggingPoint != Point.Empty)
            {
                string data = pArgs.Data.GetData(typeof(string)) as string;

                data.Split(',').All(e =>
                {
                    int lIndex = Convert.ToInt32(e);
                    DrawingTool lDrawingTool = DrawingTools[lIndex];
                    Point lPoint = GetScrollablePoint(PointToClient(new Point(pArgs.X, pArgs.Y)));

                    InvalidateRect(lDrawingTool.Tracker.SurroundingRect);
                    lDrawingTool.DoDrop(new Point(lPoint.X - DraggingPoint.X, lPoint.Y - DraggingPoint.Y));
                    InvalidateRect(lDrawingTool.Tracker.SurroundingRect);
                    SelectedTool = lDrawingTool;

                    return true;
                });

                IsDirty = true;
            }
            else
            {
                // drag ToolboxItem & drop.
                ToolboxItem lItem = GetService<IToolboxService>().DeserializeToolboxItem(pArgs.Data);
                DrawingTool lTool = lItem.CreateComponents(DesignerHost).FirstOrDefault() as DrawingTool;

                if (lTool != null)
                {
                    lTool.CreatePersistence();
                    Point lLocation = GetScrollablePoint(PointToClient(new Point(pArgs.X, pArgs.Y)));
                    Rectangle lRect = lTool.SurroundingRect;

                    lLocation.Offset(-lRect.Width / 2, -lRect.Height / 2);
                    lTool.Location = lLocation;
                    AddTool(lTool);
                    SelectedTool = lTool;
                    InvalidateRect(lTool.SurroundingRect);
                }
            }
        }

        private Rectangle DragBoxFromMouseDown { get; set; }

        protected override void OnMouseDown(MouseEventArgs pArgs)
        {
            if (pArgs.Button == MouseButtons.Left)
            {
                Point lLocation = GetScrollablePoint(pArgs.Location);
                DrawingTool lTool = HitTest(lLocation);

                if (lTool == null)
                    base.FullDragMode = SelectedToolboxItem == null;
                else
                    lTool.Tracker.StartResize(lLocation);

                SelectedTool = lTool;

                Size lDragSize = GraphicsMapper.Instance.TransformSize(SystemInformation.DragSize);
                DragBoxFromMouseDown = new Rectangle(lLocation, lDragSize);
                DraggingPoint = lLocation;
            }

            base.OnMouseDown(pArgs);
        }

        protected override void OnMouseMove(MouseEventArgs pArgs)
        {
            base.OnMouseMove(pArgs);

            Point lLocation = GetScrollablePoint(pArgs.Location);

            switch (pArgs.Button)
            {
                case MouseButtons.Right:
                    return;

                case MouseButtons.None:
                    Cursor lCurrentCursor = Cursors.Hand;

                    if (SelectedToolboxItem == null
                        && PickingTool == null)
                    {
                        DrawingTools.Any(e =>
                       {
                           Cursor lCursor = e.Tracker.GetCursor(lLocation);
                           bool lRet = false;

                           if (lCursor != Cursors.Default)
                           {
                               lCurrentCursor = lCursor;
                               lRet = true;
                           }

                           return lRet;
                       });
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
                            InvalidateRect(lTracker.SurroundingRect);
                            lTracker.Resize(lLocation);
                            InvalidateRect(lTracker.SurroundingRect);
                        }
                        else if (DragBoxFromMouseDown != Rectangle.Empty)
                        {
                            // start DragDrop on selected tool object.
                            if (!DragBoxFromMouseDown.Contains(lLocation))
                            {
                                int dx = GraphicsMapper.Instance.TransformInt(DraggingPoint.X - SelectedTool.SurroundingRect.Left, CoordinateSpace.Device, CoordinateSpace.Page);
                                int dy = GraphicsMapper.Instance.TransformInt(DraggingPoint.Y - SelectedTool.SurroundingRect.Top, CoordinateSpace.Device, CoordinateSpace.Page);

                                using (new DragImage(SelectedTool.GetDraggingImage(LayerWidth, LayerHeight), dx, dy))
                                {
                                    int lIndex = DrawingTools.IndexOf(SelectedTool);

                                    DoDragDrop(lIndex.ToString(), DragDropEffects.All);
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
                                if (!DragBoxFromMouseDown.Contains(lLocation))
                                {
                                    PickingTool = lItem.CreateComponents(DesignerHost).FirstOrDefault() as DrawingTool;
                                    PickingTool.CreatePersistence();
                                    PickingTool.StartResize(lLocation);
                                }
                            }
                        }
                    }
                    break;
            }

            if (PickingTool != null)
            {
                InvalidateRect(PickingTool.SurroundingRect);
                PickingTool.Resize(lLocation);
                InvalidateRect(PickingTool.SurroundingRect);
            }
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs args)
        {
            base.OnGiveFeedback(args);

            args.UseDefaultCursors = false;
            Cursor.Current = Cursors.SizeAll;
        }

        protected override void OnMouseUp(MouseEventArgs args)
        {
            base.OnMouseUp(args);

            if (FullDragMode)
                FullDragMode = false;

            Point lLocation = GetScrollablePoint(args.Location);

            switch (args.Button)
            {
                case MouseButtons.Right:
                    SelectedTool = HitTest(lLocation);
                    if (SelectedTool != null)
                        _contextMenuStrip.Show(this, args.Location);
                    break;

                case MouseButtons.Left:
                    // End tracker resize operation.
                    if (SelectedTool != null
                        && SelectedTool.Tracker.IsResizing)
                    {
                        Rectangle lRect = SelectedTool.SurroundingRect;
                        int lWidth = SelectedTool.Tracker.Margin + 1;     // +1 for SmoothingMode.AntiAlias

                        InvalidateRect(InflateRect(SelectedTool.SurroundingRect, lWidth, lWidth));
                        SelectedTool.Tracker.EndResize(lLocation);
                        InvalidateRect(InflateRect(SelectedTool.SurroundingRect, lWidth, lWidth));
                        SelectedTool = SelectedTool;    // refresh property grid
                        IsDirty = true;
                    }
                    else if (PickingTool != null)
                    {
                        if (PickingTool.EndResize(lLocation))
                        {
                            AddTool(PickingTool);
                            SelectedTool = PickingTool;
                            InvalidateRect(PickingTool.SurroundingRect);
                            PickingTool = null;
                        }
                    }
                    break;
            }

            DragBoxFromMouseDown = Rectangle.Empty;
            DraggingPoint = Point.Empty;
        }

        protected override void OnMouseDoubleClick(MouseEventArgs args)
        {
            base.OnMouseDoubleClick(args);

            if (PickingTool != null)
            {
                PickingTool.Escape();
                AddTool(PickingTool);
                SelectedTool = PickingTool;
                InvalidateRect(PickingTool.SurroundingRect);
                PickingTool = null;
            }
            else if (SelectedTool != null)
            {
                SelectedTool.OnMouseDoubleClick(this, args);
            }
        }

        public void Cleanup()
        {
            DrawingTools.ToList().All(t =>
            {
                RemoveTool(t);
                return true;
            });
        }

        private void RemoveTool(DrawingTool tool)
        {
            tool.OnRemove();
            DrawingTools.Remove(tool);

            Rectangle lRect = tool.Tracker.SurroundingRect;

            InvalidateRect(lRect);
            IsDirty = true;
        }

        protected override void OnAddTool(DrawingTool tool)
        {
            base.OnAddTool(tool);

            // setup pTool events
            tool.IsDirtyEvent += (pSender, pArgs) =>
            {
                bool lIsDirty = pArgs.Data;

                if (lIsDirty)
                    IsDirty = true;
            };

            IsDirty = true;
        }

        protected override void OnLoadModel(DesignerModel model)
        {
            base.OnLoadModel(model);

            LayerName = model.Name;
            IsDirty = false;
            SelectedTool = null;
        }

        public string Validate()
        {
            string lRet = (this as IDataErrorInfo)[PropertyNames.LayerName];

            if (string.IsNullOrEmpty(lRet))
            {
                var lTools = from tool in DrawingTools
                             let errorMsg = tool.Validate()
                             where !string.IsNullOrEmpty(errorMsg)
                             select new Tuple<DrawingTool, string>(tool, errorMsg);

                if (lTools.Count() > 0)
                {
                    SelectedTool = lTools.First().Item1;

                    var lErrorMsgs = from t in lTools
                                     select t.Item2;

                    lRet = "-- " + string.Join("\n-- ", lErrorMsgs);
                }
            }

            return lRet;
        }

        public void Save(DesignerModel model)
        {
            var persistences = from tool in DrawingTools
                               select tool.Persistence;
            // DrawingTools
            model.Name = LayerName;
            model.Description = LayerDescription;
            model.Width = LayerWidth;
            model.Height = LayerHeight;
            model.Layout = PersistenceFactory.Instance.GetLayout(persistences);

            IsDirty = false;
        }

        void OnDeleteTool(object sender, EventArgs args)
        {
            if (MessageBoxHelper.OKCancelMessage(Properties.Resources.Warning_Delete) == DialogResult.OK)
            {
                RemoveTool(SelectedTool);
                SelectedTool = null;
            }
        }

        void OnBringToFront(object sender, EventArgs args)
        {
            OnBringTo(tools =>
            {
                // move SelectedTool next to last overlapped tool
                int lIndex = tools.Max(t => DrawingTools.IndexOf(t));

                return lIndex + 1;
            });
        }

        void OnBringToBack(object sender, EventArgs args)
        {
            OnBringTo(tools =>
            {
                // move SelectedTool prior to first overlapped tool
                int lIndex = tools.Min(t => DrawingTools.IndexOf(t));

                return lIndex;
            });
        }

        private void OnBringTo(Func<IEnumerable<DrawingTool>, int> getPositionfunc)
        {
            var overlappedTools = GetOverlappedTools(SelectedTool);

            if (overlappedTools.Count() > 0)
            {
                DrawingTools.Remove(SelectedTool);
                DrawingTools.Insert(getPositionfunc(overlappedTools), SelectedTool);
                Invalidate();
                IsDirty = true;
            }
        }

        private IEnumerable<DrawingTool> GetOverlappedTools(DrawingTool tool)
        {
            Graphics graph = Graphics.FromHwnd(Handle);

            return from t in DrawingTools
                   where t != tool
                        && t.IsOverlapped(tool, graph)
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

        private JointRuler Ruler = null;

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

        private void PaintGrid(Graphics graph)
        {
            if (!ShowGrid)
                return;

            Pen pen = new Pen(Color.DarkGray, 1.0f) { DashStyle = DashStyle.Dot };
            int origin = GraphicsMapper.Instance.TransformInt(Ruler.RulerSize.Height);

            graph.TranslateTransform(origin, origin);
            for (int i = 0; i < LayerHeight; i += 50)
                BaseRuler.DrawHorzLine(graph, pen, 0, LayerWidth, i);

            for (int i = 0; i < LayerWidth; i += 50)
                BaseRuler.DrawVertLine(graph, pen, i, 0, LayerHeight);
            graph.TranslateTransform(-origin, -origin);
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
                    if (string.IsNullOrEmpty(LayerName))
                        lRet = Properties.Resources.Error_InvalidLayerName;
                }

                return lRet;
            }
        }
        #endregion

        private string _LayerName = string.Empty;

        [CustomVisible(true)]
        [LocalizedCategory("Appearance")]
        [LocalizedDisplayName("Name")]
        public string LayerName
        {
            get { return _LayerName; }
            set
            {
                _LayerName = value;
                IsDirty = true;
            }
        }

        [CustomVisible(true)]
        [LocalizedCategory("Appearance")]
        [LocalizedDisplayName("Description")]
        public new string LayerDescription
        {
            get { return base.LayerDescription; }
            set
            {
                base.LayerDescription = value;
                IsDirty = true;
            }
        }

        [CustomVisible(true)]
        [LocalizedCategory("Appearance")]
        [LocalizedDisplayName("Width")]
        public new int LayerWidth
        {
            get { return base.LayerWidth; }
            set
            {
                base.LayerWidth = value;
                IsDirty = true;
            }
        }

        [CustomVisible(true)]
        [LocalizedCategory("Appearance")]
        [LocalizedDisplayName("Height")]
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
            return DesignerHost.GetService(typeof(T)) as T;
        }

        public void SetPenAsTransparent(bool transparent)
        {
            foreach(DrawingTool tool in DrawingTools)
            {
                if (!(tool is LineTool))
                    tool.PenColor = transparent ? Color.Transparent : Color.Black;
            }
        }

        public void MoveDown()
        {
            foreach (DrawingTool tool in DrawingTools)
            {
                Point pt = tool.Location;

                tool.Location = new Point(pt.X, pt.Y + 5);
            }
        }

        private void InitializeComponent()
        {
            _deleteToolStripMenuItem = new ToolStripMenuItem()
            {
                Size = new Size(145, 22),
                Text = Properties.Resources.ContextMenu_Delete,
            };
            _bringToBackToolStripMenuItem = new ToolStripMenuItem()
            {
                Size = new Size(145, 22),
                Text = Properties.Resources.ContextMenu_BringToBack,
            };
            _bringToFrontToolStripMenuItem = new ToolStripMenuItem()
            {
                Size = new Size(145, 22),
                Text = Properties.Resources.ContextMenu_BringToFront,
            };

            _contextMenuStrip = new ContextMenuStrip();
            _contextMenuStrip.Items.AddRange(new ToolStripItem[]
            {
                _deleteToolStripMenuItem,
                _bringToBackToolStripMenuItem,
                _bringToFrontToolStripMenuItem
            });
            _contextMenuStrip.Size = new Size(146, 70);
            Size = new Size(ViewConsts.Width, ViewConsts.Height);
        }

        private ContextMenuStrip _contextMenuStrip;
        private ToolStripMenuItem _deleteToolStripMenuItem;
        private ToolStripMenuItem _bringToBackToolStripMenuItem;
        private ToolStripMenuItem _bringToFrontToolStripMenuItem;

        bool IMessageFilter.PreFilterMessage(ref Message msg)
        {
            bool ret = false;

            switch (msg.Msg)
            {
                case WinMessageHelper.WM_KEYDOWN:
                    Keys keyDown = (Keys)msg.WParam.ToInt32();

                    switch (keyDown)
                    {
                        case Keys.ControlKey:
                            KeyboardHelper.Instance.IsCtrlPressing = true;
                            WinMessageHelper.Instance.PostMessage_MouseMove(this);
                            break;
                    }
                    break;

                case WinMessageHelper.WM_KEYUP:
                    Keys keyUp = (Keys)msg.WParam.ToInt32() | Control.ModifierKeys;

                    switch (keyUp)
                    {
                        case Keys.Delete:
                            if (SelectedTool != null
                                && SelectedTool.ProcessKeyMsg(keyUp))
                            {
                                _deleteToolStripMenuItem.PerformClick();
                                ret = true;
                            }
                            break;

                        case Keys.ControlKey:
                            KeyboardHelper.Instance.IsCtrlPressing = false;
                            WinMessageHelper.Instance.PostMessage_MouseMove(this);
                            break;
                    }
                    break;
            }

            return ret;
        }
    }
}
