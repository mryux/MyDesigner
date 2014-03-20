﻿using DesignerLibrary.DrawingTools;
using DesignerLibrary.Models;
using DesignerLibrary.Views;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DesignerLibrary.Views
{
    public interface IRuntimeView
    {
        void Load(SitePlanModel pModel);
        IntPtr SiPassMainFrameHandle { get; set; }
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

        void IRuntimeView.Load(SitePlanModel pModel)
        {
            Load( pModel );
        }

        IntPtr IRuntimeView.SiPassMainFrameHandle
        {
            get;
            set;
        }

        protected override void OnAddTool(DrawingTool pTool)
        {
            base.OnAddTool( pTool );

            pTool.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            DrawingTools.ConvertAll( t =>
            {
                (t as IDisposable).Dispose();
                return true;
            } );
        }

        protected override void OnMouseDown(MouseEventArgs pArgs)
        {
            Point lLocation = GetScrollablePoint( pArgs.Location );
            DrawingTool lTool = HitTest( lLocation );

            if (lTool != null)
                lTool.Run( this );
            else
                base.FullDragMode = true;

            base.OnMouseDown( pArgs );
        }

        protected override void OnMouseUp(MouseEventArgs pArgs)
        {
            base.OnMouseUp( pArgs );

            base.FullDragMode = false;
        }
    }
}
