namespace DesignerLibrary
{
    partial class DesignerSurfaceView
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
            base.Dispose( disposing );
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DesignerSurfaceView));
            this._Panel = new System.Windows.Forms.Panel();
            this._ToolboxControl = new DesignerLibrary.ToolboxControl();
            this.SuspendLayout();
            // 
            // _Panel
            // 
            resources.ApplyResources(this._Panel, "_Panel");
            this._Panel.BackColor = System.Drawing.Color.SkyBlue;
            this._Panel.Name = "_Panel";
            // 
            // toolboxControl1
            // 
            resources.ApplyResources(this._ToolboxControl, "toolboxControl1");
            this._ToolboxControl.Name = "toolboxControl1";
            // 
            // DesignerSurfaceView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._ToolboxControl);
            this.Controls.Add(this._Panel);
            this.Name = "DesignerSurfaceView";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _Panel;
        private ToolboxControl _ToolboxControl;
    }
}
