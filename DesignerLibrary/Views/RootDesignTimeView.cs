using DesignerLibrary.Consts;
using DesignerLibrary.DrawingTools;
using DesignerLibrary.Helpers;
using DesignerLibrary.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace DesignerLibrary.Views
{
    public partial class RootDesignTimeView : UserControl
    {
        private static readonly ILog cLog = LogManager.GetLogger(typeof(RootDesignTimeView));
        private DesignSurface _DesignSurface;
        private IDesignerHost DesignerHost { get; set; }
        private ISelectionService SelectionService { get; set; }

        public RootDesignTimeView()
        {
            InitializeComponent();
        }

        private static readonly Dictionary<Type, string> ToolMap = new Dictionary<Type, string>()
        {
            { typeof(LineTool), Properties.Resources.Tool_Line },
            { typeof(RectangleTool), Properties.Resources.Tool_Rectangle },
            { typeof(EllipseTool), Properties.Resources.Tool_Ellipse },
            { typeof(PolygonTool), Properties.Resources.Tool_Polygon },
            { typeof(ArcTool), Properties.Resources.Tool_Arc },
            { typeof(ImageTool), Properties.Resources.Tool_Image },
            { typeof(TextTool), Properties.Resources.Tool_Text },
            { typeof(BarcodeTool), Properties.Resources.Tool_Barcode },
        };

        protected override void OnLoad(EventArgs args)
        {
            base.OnLoad(args);

            _DesignSurface = new DesignSurface();
            _DesignSurface.Loaded += new LoadedEventHandler(OnDesignSurfaceLoaded);

            _ToolboxControl.AddToolboxItem(new ToolboxItem() { TypeName = NameConsts.Pointer, DisplayName = Properties.Resources.Tool_Pointer, Bitmap = new Bitmap(1, 1) });
            ToolMap.All(pair =>
            {
                _ToolboxControl.AddToolboxItem(new ToolboxItem(pair.Key) { DisplayName = pair.Value });
                return true;
            });

            DesignerHost = _DesignSurface.GetService(typeof(IDesignerHost)) as IDesignerHost;
            DesignerHost.AddService(typeof(IToolboxService), _ToolboxControl.ToolboxService);
            //DesignerHost.AddService( typeof( IPropertyValueUIService ), new GlyphService() );

            SelectionService = _DesignSurface.GetService(typeof(ISelectionService)) as ISelectionService;
            SelectionService.SelectionChanged += OnDesignerSurfaceView_SelectionChanged;

            // Initialise the DesignSurface class
            _DesignSurface.BeginLoad(typeof(RootDesignedComponent));
        }

        void OnDesignerSurfaceView_SelectionChanged(object sender, EventArgs args)
        {
            object lObj = SelectionService.PrimarySelection;

            _PropertyGrid.SelectedObject = lObj;
        }

        public Panel DesignerPanel
        {
            get { return splitContainer1.Panel1; }
        }

        void OnDesignSurfaceLoaded(object pSender, LoadedEventArgs pArgs)
        {
            if (pArgs.HasSucceeded)
            {
                DesignTimeView designView = _DesignSurface.View as DesignTimeView;

                Application.AddMessageFilter(designView);
                designView.DirtyEvent += OnDirtyEvent;
                designView.Dock = DockStyle.Fill;
                DesignerPanel.Controls.Add(designView);

                // set PropertyGrid.Site, so IDataErrorInfo could work properly.
                _PropertyGrid.Site = DesignerHost.RootComponent.Site;
                _PropertyGrid.SelectedObject = designView;
            }
            else
            {
                // log errors
                foreach (object error in _DesignSurface.LoadErrors)
                {
                    Exception ex = error as Exception;
                    string errorString;

                    if (ex != null)
                        errorString = ex.Message;
                    else
                        errorString = error.ToString();

                    cLog.Error(errorString);
                }
            }
        }

        void OnDirtyEvent(object sender, EventArgs<bool> args)
        {
            bool isDirty = args.Data;

            Parent.Text = Title;
            if (isDirty)
                Parent.Text += "*";
        }

        public void OnLoadModel(object sender, EventArgs<Tuple<string, DesignerModel>> args)
        {
            if (DesignView.IsDirty)
            {
                DialogResult result = MessageBox.Show("Current Model has been changed, are you sure to save it?", "Warning", MessageBoxButtons.YesNoCancel);

                if (result != DialogResult.Cancel)
                {
                    if (result == DialogResult.Yes)
                    {
                        OnSaveModel(this, EventArgs.Empty);
                    }
                }
            }

            // clean up
            DesignView.Cleanup();

            Title = args.Data.Item1;
            DesignView.Load(args.Data.Item2);
        }

        private string Title { get; set; }

        public void OnSaveModel(object sender, EventArgs args)
        {
            FileDialog dialog = new SaveFileDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                DesignerModel model = new DesignerModel();

                DesignView.Save(model);
                model.SaveToFile(dialog.FileName);
            }
        }

        public DesignerModel Model
        {
            get
            {
                DesignerModel model = new DesignerModel();

                DesignView.Save(model);
                return model;
            }
        }

        public void OnPrint(PrintPageEventArgs args)
        {
            DesignView.OnPrint(args);
        }

        private DesignTimeView DesignView
        {
            get { return DesignerPanel.Controls.OfType<DesignTimeView>().First(); }
        }
    }
}
