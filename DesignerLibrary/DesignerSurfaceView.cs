using System;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using DesignerLibrary.Consts;
using DesignerLibrary.DrawingTools;

namespace DesignerLibrary
{
    public partial class DesignerSurfaceView : UserControl
    {
        private DesignSurface _DesignSurface;
        private IDesignerHost DesignerHost { get; set; }

        public DesignerSurfaceView()
        {
            InitializeComponent();

            _DesignSurface = new DesignSurface();
            _DesignSurface.Loaded += new LoadedEventHandler( OnDesignSurfaceLoaded );

            _ToolboxControl.AddToolboxItem( new ToolboxItem() { DisplayName = NameConsts.Pointer, Bitmap = new Bitmap( 1, 1 ) } );
            _ToolboxControl.AddToolboxItem( new ToolboxItem( typeof( LineTool ) ) );
            _ToolboxControl.AddToolboxItem( new ToolboxItem( typeof( RectangleTool ) ) );
            _ToolboxControl.AddToolboxItem( new ToolboxItem( typeof( EllipseTool ) ) );
            _ToolboxControl.AddToolboxItem( new ToolboxItem( typeof( PolygonTool ) ) );
            _ToolboxControl.AddToolboxItem( new ToolboxItem( typeof( ArcTool ) ) );
        }

        public Panel DesignerPanel
        {
            get { return _Panel; }
        }

        public void LoadDesigner(DesignerLoader pLoader)
        {
            DesignerHost = _DesignSurface.GetService( typeof( IDesignerHost ) ) as IDesignerHost;

            DesignerHost.AddService( typeof( IToolboxService ), _ToolboxControl.ToolboxService );
            // Initialise the DesignSurface class
            if (pLoader == null)
            {
                _DesignSurface.BeginLoad( typeof( RootDesignedComponent ) );
            }
            else
            {
                _DesignSurface.BeginLoad( pLoader );
            }
        }

        void OnDesignSurfaceLoaded(object pSender, LoadedEventArgs pArgs)
        {
            if (pArgs.HasSucceeded)
            {
                Control lDesignView = _DesignSurface.View as Control;

                lDesignView.Dock = DockStyle.Fill;
                DesignerPanel.Controls.Add( lDesignView );
            }
            else
            {
                // log errors
                foreach (object lError in _DesignSurface.LoadErrors)
                {
                    Exception lExceptionError = lError as Exception;

                    //if (lExceptionError != null)
                    //{
                    //    lErrorString += Environment.NewLine + lExceptionError.Message;
                    //}
                    //else
                    //{
                    //    lErrorString += Environment.NewLine + lError.ToString();
                    //}
                    //sLog.Error( lErrorString );
                }
            }
        }
    }
}
