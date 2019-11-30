using DesignerLibrary.Constants;
using System.Collections.Generic;
using System.Drawing;

namespace DesignerLibrary.Views.Rulers
{
    class JointRuler : BaseRuler
    {
        private List<BaseRuler> Rulers = new List<BaseRuler>();

        public JointRuler()
            : base(new Size(ViewConsts.RulerHeight, ViewConsts.RulerHeight))
        {
            Rulers.AddRange(new BaseRuler[]
            {
                new HorzRuler(new Size(ViewConsts.Width, ViewConsts.RulerHeight)),
                new VertRuler(new Size(ViewConsts.RulerHeight, ViewConsts.Height)),
            });
        }

        protected override void OnPaint(Graphics graph)
        {
            int cornerHeight = PageRulerSize.Height;
            int width = Rulers[0].PageRulerSize.Width;
            int height = Rulers[1].PageRulerSize.Height;

            DrawTopLeftCorner(graph, cornerHeight);

            graph.TranslateTransform(cornerHeight, 0);
            Rulers[0].Paint(graph);

            graph.TranslateTransform(-cornerHeight, cornerHeight);
            Rulers[1].Paint(graph);

            graph.TranslateTransform(cornerHeight, 0);
            DrawHorzLine(graph, Pens.Black, 0, width, height);
            DrawVertLine(graph, Pens.Black, width, 0, height);

            // restore coordinate origin
            graph.TranslateTransform(-cornerHeight, -cornerHeight);
        }

        private void DrawTopLeftCorner(Graphics graph, int height)
        {
            Rectangle rect = new Rectangle(Point.Empty, new Size(height, height));

            graph.FillRectangle(Brushes.White, rect);

            DrawHorzLine(graph, Pens.White, rect.Left, rect.Right, 0);
            DrawHorzLine(graph, Pens.Gray, rect.Left + 1, rect.Right, 1);
            DrawHorzLine(graph, Pens.Gray, rect.Left + 2, rect.Right, 2);
            DrawHorzLine(graph, Pens.DarkGray, rect.Left + 3, rect.Right, 3);
            DrawHorzLine(graph, Pens.Black, rect.Left + 3, rect.Right, 4);

            DrawVertLine(graph, Pens.White, 0, rect.Top, rect.Bottom);
            DrawVertLine(graph, Pens.Gray, 1, rect.Top + 1, rect.Bottom);
            DrawVertLine(graph, Pens.Gray, 2, rect.Top + 2, rect.Bottom);
            DrawVertLine(graph, Pens.DarkGray, 3, rect.Top + 3, rect.Bottom);
            DrawVertLine(graph, Pens.Black, 4, rect.Top + 4, rect.Bottom);
        }
    }
}
