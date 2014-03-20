using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DesignerLibrary.Helpers
{
    class GraphicsMapper
    {
        private GraphicsMapper()
        {
        }

        public static readonly GraphicsMapper Instance = new GraphicsMapper();

        private Graphics Graph { get; set; }

        public void Initialize(Control pControl)
        {
            Graph = Graphics.FromHwnd( pControl.Handle );
            InitGraphics( Graph );
        }

        public int TransformInt(int pValue, CoordinateSpace pDest = CoordinateSpace.Page, CoordinateSpace pSource = CoordinateSpace.Device)
        {
            Point lPt = new Point( pValue, 0 );

            return TransformPoint( lPt, pDest, pSource ).X;
        }

        public Point TransformPoint(Point pPoint, CoordinateSpace pDest = CoordinateSpace.Page, CoordinateSpace pSource = CoordinateSpace.Device)
        {
            Point[] lPoints = new Point[] { pPoint };

            Graph.TransformPoints( pDest, pSource, lPoints );
            return lPoints[0];
        }

        public Rectangle TransformRectangle(Rectangle pRect, CoordinateSpace pDest = CoordinateSpace.Page, CoordinateSpace pSource = CoordinateSpace.Device)
        {
            Point[] lPoints = new Point[] { pRect.Location, new Point( pRect.Width, pRect.Height ) };

            Graph.TransformPoints( pDest, pSource, lPoints );
            return new Rectangle( lPoints[0], new Size( lPoints[1].X, lPoints[1].Y ) );
        }

        public Size TransformSize(Size pSize, CoordinateSpace pDest = CoordinateSpace.Page, CoordinateSpace pSource = CoordinateSpace.Device)
        {
            Point[] lPoints = new Point[] { Point.Empty, new Point( pSize.Width, pSize.Height ) };

            Graph.TransformPoints( pDest, pSource, lPoints );
            return new Size( lPoints[1].X, lPoints[1].Y );
        }

        public static void InitGraphics(Graphics pGraph)
        {
            pGraph.PageUnit = GraphicsUnit.Millimeter;
            pGraph.PageScale = 0.1f;
            pGraph.SmoothingMode = SmoothingMode.AntiAlias;
        }
    }
}
