using System.Drawing;

namespace DesignerLibrary.Views.Rulers
{
    class HorzRuler : BaseRuler
    {
        public HorzRuler(Size size)
            : base(size)
        {
        }

        protected override void OnPaint(Graphics graph)
        {
            base.OnPaint(graph);

            int height = PageRulerSize.Height;

            DrawShadow
            (
                (pen, i) => DrawHorzLine(graph, pen, 0, PageRulerSize.Width, i),
                (pen, i) => DrawHorzLine(graph, pen, 0, PageRulerSize.Width, height - i)
            );

            DrawCalibration(PageRulerSize.Width, 
                (unit, notchHeight) => DrawVertLine(graph, Pens.Black, unit, height - notchHeight, height));

            // draw Numbers
            DrawNumbers(graph, PageRulerSize.Width, (unit, size) =>
            {
                return new PointF(unit - size.Width / 2, height / 2 - size.Height / 2);
            });
        }
    }
}
