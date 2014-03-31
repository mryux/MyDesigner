using DesignerLibrary.Models;
using DesignerLibrary.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyDesigner
{
    public partial class RuntimeForm : Form
    {
        public RuntimeForm()
        {
            InitializeComponent();
        }

        public SitePlanModel Model { get; set; }

        protected override void OnLoad(EventArgs pArgs)
        {
            base.OnLoad( pArgs );

            IRuntimeView lView = RuntimeViewFactory.Instance.NewRuntimeView();

            lView.Load( Model );

            Control lControl = lView as Control;

            lControl.Dock = DockStyle.Fill;
            Controls.Add( lControl );
        }
    }
}
