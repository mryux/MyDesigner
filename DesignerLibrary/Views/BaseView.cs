using DesignerLibrary.DrawingTools;
using DesignerLibrary.Persistence;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace DesignerLibrary.Views
{
    // RootDesignerView is a simple control that will be displayed 
    // in the designer window.
    abstract class BaseView : ScrollableControl
    {
        protected BaseView()
        {
            BackColor = Color.LightGreen;

            DoubleBuffered = true;
            ResizeRedraw = true; 
            AutoScrollMinSize = new Size( 1000, 1000 );
            CaptionHeight = 30;
        }

        protected virtual void OnAddTool(DrawingTool pTool)
        {
        }

        protected static readonly Type[] PersistenceTypes = new Type[]
        {
            typeof( EllipseToolPersistence ),
            typeof( ImageToolPersistence ),
            typeof( LineToolPersistence ),
            typeof( PolygonToolPersistence ),
            typeof( RectangleToolPersistence ),
        };

        protected virtual void OnLoadModel(string pFilePath)
        {
            using (TextReader reader = new StreamReader(pFilePath))
            {
                XmlSerializer lSerializer = new XmlSerializer(typeof(SitePlanTools), PersistenceTypes);
                SitePlanTools lTools = lSerializer.Deserialize(reader) as SitePlanTools;

                LayerName = lTools.Name;
                LayerDescription = lTools.Description;
                LayerWidth = lTools.Width;
                LayerHeight = lTools.Height;
                lTools.Persistences.All(persistence =>
                {
                    DrawingTool lTool = persistence.CreateDrawingTool();

                    lTool.Persistence = persistence;
                    AddTool(lTool);
                    return true;
                });

                Invalidate();
            }
        }

        protected List<DrawingTool> DrawingTools = new List<DrawingTool>();

        protected void AddTool(DrawingTool pTool)
        {
            OnAddTool( pTool );

            DrawingTools.Add( pTool );
        }

        public void Load(string pPath)
        {
            OnLoadModel( pPath );
        }

        private static readonly Color DesignTitleBaseColor = Color.FromArgb( 50, 50, 50 );
        private static readonly Color DesignTitleLightingColor = Color.FromArgb( 200, 200, 200 );
        private int CaptionHeight { get; set; }

        protected override void OnPaint(PaintEventArgs pArgs)
        {
            base.OnPaint( pArgs );

            Graphics lGraph = pArgs.Graphics;

            lGraph.SmoothingMode = SmoothingMode.AntiAlias;
            lGraph.TranslateTransform( AutoScrollPosition.X, AutoScrollPosition.Y );

            // paint title
            Rectangle lRect = new Rectangle( Bounds.Left, Bounds.Top, AutoScrollMinSize.Width, CaptionHeight );

            if (pArgs.ClipRectangle.IntersectsWith( lRect ))
            {
                Brush lBrush = new LinearGradientBrush( lRect, DesignTitleBaseColor, DesignTitleLightingColor, LinearGradientMode.Horizontal );

                lGraph.FillRectangle( lBrush, lRect );
                lRect.Inflate( -4, 0 );
                lGraph.DrawString( LayerDescription, Font, Brushes.White, lRect );
            }

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

        private string _Name = string.Empty;
        protected string LayerName
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _Description = string.Empty;
        protected string LayerDescription
        {
            get { return _Description; }
            set
            {
                _Description = value;

                Rectangle lRect = new Rectangle( Bounds.Left, Bounds.Top, Bounds.Width, CaptionHeight );
                InvalidateRect( lRect );
            }
        }

        protected int LayerWidth
        {
            get { return AutoScrollMinSize.Width; }
            set
            {
                AutoScrollMinSize = new Size(value, AutoScrollMinSize.Height);
            }
        }

        protected int LayerHeight
        {
            get { return AutoScrollMinSize.Height; }
            set
            {
                AutoScrollMinSize = new Size(AutoScrollMinSize.Width, value);
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
            Point lPoint = pPoint;

            lPoint.Offset( -AutoScrollPosition.X, -AutoScrollPosition.Y );
            return lPoint;
        }

        /// <summary>
        /// map pRect to rectangle in current view
        /// </summary>
        /// <param name="pRect">rect in scroll control</param>
        protected void InvalidateRect(Rectangle pRect)
        {
            Rectangle lRect = pRect;

            lRect.Offset( AutoScrollPosition.X, AutoScrollPosition.Y );
            Invalidate( lRect );
        }
    }

    public class SitePlanTools
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public ToolPersistence[] Persistences { get; set; }
    }
}