using DesignerLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace DesignerLibrary.Views
{
    abstract class Ruler
    {
        public Size RulerSize { get; protected set; }

        protected Ruler(Size pSize)
        {
            RulerSize = pSize;
            RulerFont = new Font( FontFamily.GenericSerif, 8.0f );
        }

        protected Font RulerFont { get; set; }
        protected Image RulerImage { get; set; }

        protected void DrawShadow(Action<Pen, int> pDrawUpShadow, Action<Pen, int> pDrawDownShadow)
        {
            Color[] lColors = new Color[] { Color.White, Color.Gray, Color.Gray, Color.DarkGray, Color.Black };
            Pen lPen = new Pen( Color.Black, 1.0f );

            for (int i = 0; i < lColors.Length; i++)
            {
                lPen.Color = lColors[i];
                pDrawUpShadow( lPen, i );
            }

            lColors = new Color[] { Color.Gray, Color.Gray, Color.White, Color.Gray };

            for (int i = 0; i < lColors.Length; i++)
            {
                lPen.Color = lColors[i];
                pDrawDownShadow( lPen, i );
            }
        }

        protected void DrawCalibration(Action<int, int> pDrawNotch)
        {
            int lWidth = GraphicsMapper.Instance.TransformInt( RulerSize.Width );

            for (int i = 0; i < lWidth; i += 10)
            {
                int lNotchHeight = 5;

                if (i % 100 == 0)
                    lNotchHeight = 20;
                else if (i % 50 == 0)
                    lNotchHeight = 10;

                pDrawNotch( i, lNotchHeight );
            }
        }

        public static void DrawHorzLine(Graphics pGraph, Pen pPen, int pLeft, int pRight, int pY)
        {
            pGraph.DrawLine( pPen, new Point( pLeft, pY ), new Point( pRight, pY ) );
        }

        public static void DrawVertLine(Graphics pGraph, Pen pPen, int pX, int pTop, int pBottom)
        {
            pGraph.DrawLine( pPen, new Point( pX, pTop ), new Point( pX, pBottom ) );
        }

        protected void DrawNumber(Graphics pGraph, Func<int, SizeF, PointF> pGetPos)
        {
            int lWidth = GraphicsMapper.Instance.TransformInt( RulerSize.Width );

            for (int i = 0; i < lWidth; i += 10)
            {
                if (i % 100 == 0)
                {
                    string lText = (i / 100).ToString();
                    SizeF lSize = pGraph.MeasureString( lText, RulerFont );

                    pGraph.DrawString( lText, RulerFont, Brushes.Black, pGetPos( i, lSize ) );
                }
            }
        }

        protected abstract void OnPaint(Graphics pGraph);
        public void Paint(Graphics pGraph)
        {
            OnPaint( pGraph );
        }
    }

    class HorzRuler : Ruler
    {
        public HorzRuler(Size pSize)
            : base( pSize )
        {
        }

        protected override void OnPaint(Graphics pGraph)
        {
            Rectangle lRect = new Rectangle( Point.Empty, GraphicsMapper.Instance.TransformSize( RulerSize ) );
            int lHeight = lRect.Height;

            pGraph.FillRectangle( Brushes.White, lRect );
            DrawShadow
            (
                (pen, i) => DrawHorzLine( pGraph, pen, lRect.Left, lRect.Right, i ),
                (pen, i) => DrawHorzLine( pGraph, pen, lRect.Left, lRect.Right, lRect.Bottom - i )
            );
            DrawCalibration( (unit, notchHeight) => DrawVertLine( pGraph, Pens.Black, unit, lHeight - notchHeight, lHeight ) );

            // draw Numbers
            DrawNumber( pGraph, (unit, size) =>
            {
                return new PointF( unit - size.Width / 2, lHeight / 2 - size.Height / 2 );
            } );
        }
    }

    class VertRuler : Ruler
    {
        public VertRuler(Size pSize)
            : base( pSize )
        {
        }

        protected override void OnPaint(Graphics pGraph)
        {
            Rectangle lRect = new Rectangle( Point.Empty, GraphicsMapper.Instance.TransformSize( RulerSize ) );
            int lHeight = lRect.Height;

            pGraph.FillRectangle( Brushes.White, new Rectangle( Point.Empty, new Size( lRect.Height, lRect.Width ) ) );
            DrawShadow
            (
                (pen, i) => DrawVertLine( pGraph, pen, i, lRect.Left, lRect.Right ),
                (pen, i) => DrawVertLine( pGraph, pen, lRect.Bottom - i, lRect.Left, lRect.Right )
            );

            DrawCalibration( (unit, notchHeight) => DrawHorzLine( pGraph, Pens.Black, lHeight - notchHeight, lHeight, unit ) );

            // draw Numbers
            DrawNumber( pGraph, (unit, size) =>
            {
                return new PointF( lHeight / 2 - size.Width / 2, unit - size.Height / 2 );
            } );
        }
    }

    class JointRuler : Ruler
    {
        private List<Ruler> Rulers = new List<Ruler>();

        public JointRuler()
            : base( new Size( 1000, 30 ) )
        {
            Rulers.AddRange( new Ruler[] { new HorzRuler( RulerSize ), new VertRuler( RulerSize ) } );
        }

        protected override void OnPaint(Graphics pGraph)
        {
            int lHeight = GraphicsMapper.Instance.TransformInt( Rulers[0].RulerSize.Height );

            DrawTopLeftCorner( pGraph );

            pGraph.TranslateTransform( lHeight, 0 );
            Rulers[0].Paint( pGraph );

            pGraph.TranslateTransform( -lHeight, lHeight );
            Rulers[1].Paint( pGraph );

            // restore coordinate origin
            pGraph.TranslateTransform( 0, -lHeight );
        }

        private void DrawTopLeftCorner(Graphics pGraph)
        {
            Size lSize = GraphicsMapper.Instance.TransformSize( new Size( RulerSize.Height, RulerSize.Height ) );
            Rectangle lRect = new Rectangle( Point.Empty, lSize );

            pGraph.FillRectangle( Brushes.White, lRect );

            DrawHorzLine( pGraph, Pens.White, lRect.Left, lRect.Right, 0 );
            DrawHorzLine( pGraph, Pens.Gray, lRect.Left + 1, lRect.Right, 1 );
            DrawHorzLine( pGraph, Pens.Gray, lRect.Left + 2, lRect.Right, 2 );
            DrawHorzLine( pGraph, Pens.DarkGray, lRect.Left + 3, lRect.Right, 3 );
            DrawHorzLine( pGraph, Pens.Black, lRect.Left + 3, lRect.Right, 4 );

            DrawVertLine( pGraph, Pens.White, 0, lRect.Top, lRect.Bottom );
            DrawVertLine( pGraph, Pens.Gray, 1, lRect.Top + 1, lRect.Bottom );
            DrawVertLine( pGraph, Pens.Gray, 2, lRect.Top + 2, lRect.Bottom );
            DrawVertLine( pGraph, Pens.DarkGray, 3, lRect.Top + 3, lRect.Bottom );
            DrawVertLine( pGraph, Pens.Black, 4, lRect.Top + 4, lRect.Bottom );
        }
    }
}
