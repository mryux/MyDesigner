using System.Windows.Forms;
namespace DesignerLibrary.Views
{
    partial class RootDesignTimeView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            Application.RemoveMessageFilter(DesignView);
            base.Dispose( disposing );
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this._PropertyGrid = new System.Windows.Forms.PropertyGrid();
            this._ToolboxControl = new ToolboxControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(937, 546);
            this.splitContainer1.SplitterDistance = 780;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this._ToolboxControl);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this._PropertyGrid);
            this.splitContainer2.Size = new System.Drawing.Size(323, 546);
            this.splitContainer2.SplitterDistance = 320;
            this.splitContainer2.TabIndex = 0;
            // 
            // propertyGrid1
            // 
            this._PropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._PropertyGrid.Location = new System.Drawing.Point(0, 0);
            this._PropertyGrid.Name = "propertyGrid1";
            this._PropertyGrid.Size = new System.Drawing.Size(323, 157);
            this._PropertyGrid.TabIndex = 0;
            this._PropertyGrid.ToolbarVisible = false;
            // 
            // toolboxControl1
            // 
            this._ToolboxControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._ToolboxControl.Location = new System.Drawing.Point(0, 0);
            this._ToolboxControl.Name = "toolboxControl1";
            this._ToolboxControl.Size = new System.Drawing.Size(323, 385);
            this._ToolboxControl.TabIndex = 0;
            // 
            // RootDesignTimeView
            // 
            this.Controls.Add(this.splitContainer1);
            this.Name = "RootDesignTimeView";
            this.Size = new System.Drawing.Size(937, 546);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private PropertyGrid _PropertyGrid;
        private ToolboxControl _ToolboxControl;
    }
}
