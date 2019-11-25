namespace MyDesigner
{
    partial class RuntimeForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemPrint = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsBmpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printAsBmpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemPrint,
            this.saveAsBmpToolStripMenuItem,
            this.printAsBmpToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(211, 104);
            // 
            // toolStripMenuItemPrint
            // 
            this.toolStripMenuItemPrint.Name = "toolStripMenuItemPrint";
            this.toolStripMenuItemPrint.Size = new System.Drawing.Size(210, 24);
            this.toolStripMenuItemPrint.Text = "Print";
            this.toolStripMenuItemPrint.Click += new System.EventHandler(this.toolStripMenuItemPrint_Click);
            // 
            // saveAsBmpToolStripMenuItem
            // 
            this.saveAsBmpToolStripMenuItem.Name = "saveAsBmpToolStripMenuItem";
            this.saveAsBmpToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            this.saveAsBmpToolStripMenuItem.Text = "Save as bmp";
            this.saveAsBmpToolStripMenuItem.Click += new System.EventHandler(this.saveAsBmpToolStripMenuItem_Click);
            // 
            // printAsBmpToolStripMenuItem
            // 
            this.printAsBmpToolStripMenuItem.Name = "printAsBmpToolStripMenuItem";
            this.printAsBmpToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            this.printAsBmpToolStripMenuItem.Text = "Print as bmp";
            this.printAsBmpToolStripMenuItem.Click += new System.EventHandler(this.printAsBmpToolStripMenuItem_Click);
            // 
            // RuntimeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1235, 812);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "RuntimeForm";
            this.Text = "RuntimeForm";
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPrint;
        private System.Windows.Forms.ToolStripMenuItem saveAsBmpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printAsBmpToolStripMenuItem;
    }
}