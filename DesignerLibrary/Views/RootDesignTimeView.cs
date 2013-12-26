using DesignerLibrary.Constants;
using DesignerLibrary.DrawingTools;
using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace DesignerLibrary.Views
{
    public partial class RootDesignTimeView : UserControl
    {
        //private static readonly ILog cLog = LogFactory.GetLogger( typeof( RootDesignTimeView ) );
        private DesignSurface _DesignSurface;
        private IDesignerHost DesignerHost { get; set; }
        private ISelectionService SelectionService { get; set; }

        public RootDesignTimeView()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs pArgs)
        {
            base.OnLoad( pArgs );

            _DesignSurface = new DesignSurface();
            _DesignSurface.Loaded += new LoadedEventHandler( OnDesignSurfaceLoaded );

            _ToolboxControl.AddToolboxItem( new ToolboxItem() { TypeName = NameConsts.Pointer, DisplayName = Properties.Resources.Tool_Pointer, Bitmap = new Bitmap( 1, 1 ) } );
            _ToolboxControl.AddToolboxItem( new ToolboxItem( typeof( LineTool ) ) { DisplayName = Properties.Resources.Tool_Line } );
            _ToolboxControl.AddToolboxItem( new ToolboxItem( typeof( RectangleTool ) ) { DisplayName = Properties.Resources.Tool_Rectangle } );
            _ToolboxControl.AddToolboxItem( new ToolboxItem( typeof( EllipseTool ) ) { DisplayName = Properties.Resources.Tool_Ellipse } );
            _ToolboxControl.AddToolboxItem( new ToolboxItem( typeof( PolygonTool ) ) { DisplayName = Properties.Resources.Tool_Polygon } );
            _ToolboxControl.AddToolboxItem( new ToolboxItem( typeof( ArcTool ) ) { DisplayName = Properties.Resources.Tool_Arc } );
            _ToolboxControl.AddToolboxItem( new ToolboxItem( typeof( ImageTool ) ) { DisplayName = Properties.Resources.Tool_Image } );

            DesignerHost = _DesignSurface.GetService( typeof( IDesignerHost ) ) as IDesignerHost;
            DesignerHost.AddService( typeof( IToolboxService ), _ToolboxControl.ToolboxService );
            //DesignerHost.AddService( typeof( IPropertyValueUIService ), new GlyphService() );

            SelectionService = _DesignSurface.GetService( typeof( ISelectionService ) ) as ISelectionService;
            SelectionService.SelectionChanged += OnDesignerSurfaceView_SelectionChanged;

            // Initialise the DesignSurface class
            _DesignSurface.BeginLoad( typeof( RootDesignedComponent ) );
        }

        void OnDesignerSurfaceView_SelectionChanged(object sender, EventArgs pArgs)
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
                Control lDesignView = _DesignSurface.View as Control;

                lDesignView.Dock = DockStyle.Fill;
                DesignerPanel.Controls.Add( lDesignView );

                // set PropertyGrid.Site, so IDataErrorInfo could work properly.
                _PropertyGrid.Site = DesignerHost.RootComponent.Site;
                _PropertyGrid.SelectedObject = lDesignView;
            }
            else
            {
                // log errors
                foreach (object lError in _DesignSurface.LoadErrors)
                {
                    Exception lExceptionError = lError as Exception;
                    string lErrorString;

                    if (lExceptionError != null)
                        lErrorString = lExceptionError.Message;
                    else
                        lErrorString = lError.ToString();

                    //cLog.Error( lErrorString );
                }
            }
        }
    }
}
