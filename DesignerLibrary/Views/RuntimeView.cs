using DesignerLibrary.Constants;
using DesignerLibrary.DrawingTools;
using DesignerLibrary.Helpers;
using DesignerLibrary.Models;
using DesignerLibrary.Views.Rulers;
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
        void OnDraw(PaintEventArgs args);
        void SetValues(string[] values);
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

        private string[] RuntimeValues { get; set; }
        void IRuntimeView.SetValues(string[] values)
        {
            RuntimeValues = values;
        }

        void IRuntimeView.OnPrint(PrintPageEventArgs args)
        {
            OnPrint(args);
        }

        void IRuntimeView.OnDraw(PaintEventArgs args)
        {
            OnPaint(args);
        }

        protected override void PrePaint(PaintEventArgs args)
        {
            base.PrePaint(args);

            Point pt = GraphicsMapper.Instance.TransformPoint(new Point(ViewConsts.RulerHeight, ViewConsts.RulerHeight));
            Graphics graph = args.Graphics;

            int width = GraphicsMapper.Instance.TransformInt(ViewConsts.Width);
            int height = GraphicsMapper.Instance.TransformInt(ViewConsts.Height);

            BaseRuler.DrawHorzLine(graph, Pens.Black, 0, width, height);
            BaseRuler.DrawVertLine(graph, Pens.Black, width, 0, height);

            graph.TranslateTransform(-pt.X, -pt.Y);
        }

        protected override void OnAddTool(DrawingTool tool)
        {
            base.OnAddTool(tool);

            if (tool.Id > 0 && tool.Id <= RuntimeValues.Length)
                tool.RuntimeValue = RuntimeValues[tool.Id - 1];

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
