using DesignerLibrary.DrawingTools;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

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
        }

        protected abstract void OnAddTool(DrawingTool pTool);

        protected List<DrawingTool> DrawingTools = new List<DrawingTool>();

        protected void AddTool(DrawingTool pTool)
        {
            OnAddTool( pTool );

            DrawingTools.Add( pTool );
        }

        protected override void OnPaint(PaintEventArgs pArgs)
        {
            base.OnPaint( pArgs );

            pArgs.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            pArgs.Graphics.TranslateTransform( AutoScrollPosition.X, AutoScrollPosition.Y );

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
    }
}