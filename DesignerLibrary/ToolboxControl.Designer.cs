namespace DesignerLibrary
{
    partial class ToolboxControl
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToolboxControl));
            this.mToolboxList = new System.Windows.Forms.ListView();
            this.mToolboxImageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // mToolboxList
            // 
            resources.ApplyResources(this.mToolboxList, "mToolboxList");
            this.mToolboxList.FullRowSelect = true;
            this.mToolboxList.Name = "mToolboxList";
            this.mToolboxList.UseCompatibleStateImageBehavior = false;
            this.mToolboxList.View = System.Windows.Forms.View.List;
            // 
            // imageList1
            // 
            this.mToolboxImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            resources.ApplyResources(this.mToolboxImageList, "imageList1");
            this.mToolboxImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // ToolboxControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mToolboxList);
            this.Name = "ToolboxControl";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView mToolboxList;
        private System.Windows.Forms.ImageList mToolboxImageList;
    }
}
