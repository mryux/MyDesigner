using DesignerLibrary.Constants;
using DesignerLibrary.DrawingTools;
using DesignerLibrary.Helpers;
using DesignerLibrary.Models;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace DesignerLibrary.Views
{
    public interface IRuntimeView
    {
        void Load(DesignerModel model);
        void OnPrint(PrintPageEventArgs args);
    }

    public class RuntimeViewFactory
    {
        private RuntimeViewFactory()
        {
        }

        public static readonly RuntimeViewFactory Instance = new RuntimeViewFactory();

        public IRuntimeView NewRuntimeView()
        {
            return new RuntimeView();
        }
    }

    class RuntimeView : BaseView, IRuntimeView
    {
        public RuntimeView()
        {
        }

        void IRuntimeView.Load(DesignerModel model)
        {
            Load(model);
        }

        void IRuntimeView.OnPrint(PrintPageEventArgs args)
        {
            OnPrint(args);
        }

        protected override void PrePaint(PaintEventArgs args)
        {
            base.PrePaint(args);

            Point pt = GraphicsMapper.Instance.TransformPoint(new Point(0, -ViewConsts.Height));
            args.Graphics.TranslateTransform(pt.X, pt.Y);
        }

        protected override void OnAddTool(DrawingTool tool)
        {
            base.OnAddTool(tool);

            tool.RuntimeInitialize(this);
        }

        protected override void Dispose(bool disposing)
        {
            DrawingTools.All(t =>
            {
                (t as IDisposable).Dispose();
                return true;
            });
        }

        protected override void OnMouseDown(MouseEventArgs args)
        {
            Point lLocation = GetScrollablePoint(args.Location);
            DrawingTool lTool = HitTest(lLocation);

            if (lTool != null)
                lTool.Run(this);
            else
                base.FullDragMode = true;

            base.OnMouseDown(args);
        }

        protected override void OnMouseUp(MouseEventArgs args)
        {
            base.OnMouseUp(args);

            base.FullDragMode = false;
        }
    }
}
