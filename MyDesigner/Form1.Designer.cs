namespace MyDesigner
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.rootDesignTimeView1 = new DesignerLibrary.Views.RootDesignTimeView();
            this.SuspendLayout();
            // 
            // rootDesignTimeView1
            // 
            resources.ApplyResources(this.rootDesignTimeView1, "rootDesignTimeView1");
            this.rootDesignTimeView1.Name = "rootDesignTimeView1";
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.rootDesignTimeView1);
            this.Name = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private DesignerLibrary.Views.RootDesignTimeView rootDesignTimeView1;

    }
}

