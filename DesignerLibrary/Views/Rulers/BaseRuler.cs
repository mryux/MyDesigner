using DesignerLibrary.Constants;
using DesignerLibrary.Helpers;
using System;
using System.Drawing;

namespace DesignerLibrary.Views.Rulers
{
    abstract class BaseRuler
    {
        public Size RulerSize { get; protected set; }
        public Size PageRulerSize { get; set; }

        protected BaseRuler(Size size)
        {
            RulerSize = size;
            PageRulerSize = GraphicsMapper.Instance.TransformSize(size);
            RulerFont = new Font(FontFamily.GenericSerif, 8.0f);
        }

        protected Font RulerFont { get; set; }
        protected Image RulerImage { get; set; }

        protected void DrawShadow(Action<Pen, int> drawUpShadow, Action<Pen, int> drawDownShadow)
        {
            Color[] colors = new Color[] { Color.White, Color.Gray, Color.Gray, Color.DarkGray, Color.Black };
            Pen pen = new Pen(Color.Black, 1.0f);

            for (int i = 0; i < colors.Length; i++)
            {
                pen.Color = colors[i];
                drawUpShadow(pen, i);
            }

            colors = new Color[] { Color.Gray, Color.Gray, Color.White, Color.Gray };

            for (int i = 0; i < colors.Length; i++)
            {
                pen.Color = colors[i];
                drawDownShadow(pen, i);
            }
        }

        protected void DrawCalibration(int length, Action<int, int> drawNotch)
        {
            for (int i = 0; i < length; i += 10)
            {
                int notchLength = ViewConsts.NormalNotchHeight;

                if (i % 100 == 0)
                    notchLength = ViewConsts.HighNotchHeight;
                else if (i % 50 == 0)
                    notchLength = ViewConsts.MiddleNotchHeight;

                drawNotch(i, notchLength);
            }
        }

        public static void DrawHorzLine(Graphics graph, Pen pen, int left, int right, int y)
        {
            graph.DrawLine(pen, new Point(left, y), new Point(right, y));
        }

        public static void DrawVertLine(Graphics graph, Pen pen, int x, int top, int bottom)
        {
            graph.DrawLine(pen, new Point(x, top), new Point(x, bottom));
        }

        protected void DrawNumbers(Graphics graph, int length, Func<int, SizeF, PointF> getPos)
        {
            for (int i = 0; i < length; i += 100)
            {
                string text = (i / 100).ToString();
                SizeF size = graph.MeasureString(text, RulerFont);

                graph.DrawString(text, RulerFont, Brushes.Black, getPos(i, size));
            }
        }

        protected virtual void OnPaint(Graphics graph)
        {
            Rectangle rect = new Rectangle(Point.Empty, PageRulerSize);

            graph.FillRectangle(Brushes.White, rect);
        }

        public void Paint(Graphics graph)
        {
            OnPaint(graph);
        }
    }
}
