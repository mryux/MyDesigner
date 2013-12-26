using System;
using System.Windows.Forms;

namespace DesignerLibrary.Views
{
    public partial class RootRuntimeView : UserControl
    {
        private Control RuntimeView = new RuntimeView();

        public RootRuntimeView()
        {
            InitializeComponent();

            RuntimeView = new RuntimeView();
            this.Controls.Add( RuntimeView );
            RuntimeView.Dock = DockStyle.Fill;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad( e );
        }
    }
}
