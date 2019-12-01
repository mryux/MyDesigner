using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DesignerLibrary.Helpers
{
    public class GraphicsMapper
    {
        private GraphicsMapper()
        {
        }

        public static readonly GraphicsMapper Instance = new GraphicsMapper();

        private Graphics Graph { get; set; }

        public void Initialize(Control control)
        {
            Graph = Graphics.FromHwnd(control.Handle);
            InitGraphics(Graph);
        }

        public int TransformInt(int value, CoordinateSpace dest = CoordinateSpace.Page, CoordinateSpace source = CoordinateSpace.Device)
        {
            Point pt = new Point(value, 0);

            return TransformPoint(pt, dest, source).X;
        }

        public Point TransformPoint(Point point, CoordinateSpace dest = CoordinateSpace.Page, CoordinateSpace source = CoordinateSpace.Device)
        {
            Point[] points = new Point[] { point };

            Graph.TransformPoints(dest, source, points);
            return points[0];
        }

        public Rectangle TransformRectangle(Rectangle rect, CoordinateSpace dest = CoordinateSpace.Page, CoordinateSpace source = CoordinateSpace.Device)
        {
            Point[] points = new Point[] { rect.Location, new Point(rect.Width, rect.Height) };

            Graph.TransformPoints(dest, source, points);
            return new Rectangle(points[0], new Size(points[1].X, points[1].Y));
        }

        public Size TransformSize(Size size, CoordinateSpace dest = CoordinateSpace.Page, CoordinateSpace source = CoordinateSpace.Device)
        {
            Point[] points = new Point[] { Point.Empty, new Point(size.Width, size.Height) };

            Graph.TransformPoints(dest, source, points);
            return new Size(points[1].X, points[1].Y);
        }

        public static void InitGraphics(Graphics graph)
        {
            graph.PageUnit = GraphicsUnit.Millimeter;
            graph.PageScale = 0.1f;
            graph.SmoothingMode = SmoothingMode.AntiAlias;
        }
    }
}
