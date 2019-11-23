using System.Drawing;

namespace DesignerLibrary.Views.Rulers
{
    class VertRuler : BaseRuler
    {
        public VertRuler(Size size)
            : base(size)
        {
        }

        protected override void OnPaint(Graphics graph)
        {
            base.OnPaint(graph);

            int width = PageRulerSize.Width;

            DrawShadow
            (
                (pen, i) => DrawVertLine(graph, pen, i, 0, PageRulerSize.Height),
                (pen, i) => DrawVertLine(graph, pen, width - i, 0, PageRulerSize.Height)
            );

            DrawCalibration(PageRulerSize.Height,
                (unit, notchHeight) => DrawHorzLine(graph, Pens.Black, width - notchHeight, width, unit));

            // draw Numbers
            DrawNumbers(graph, PageRulerSize.Height, (unit, size) =>
            {
                return new PointF(width / 2 - size.Width / 2, unit - size.Height / 2);
            });
        }
    }
}
