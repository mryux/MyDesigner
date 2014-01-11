using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DesignerLibrary;

namespace MyDesigner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs pArgs)
        {
            base.OnLoad( pArgs );

            toolStripMenuItemOpen.Click += OnOpen;
            toolStripMenuItemSave.Click += OnSave;
        }

        void OnOpen(object sender, EventArgs e)
        {
            FileDialog lDialog = new OpenFileDialog();

            lDialog.CheckFileExists = true;
            if(lDialog.ShowDialog() == DialogResult.OK)
            {
                rootDesignTimeView1.Open(lDialog.FileName);
            }
        }

        void OnSave(object sender, EventArgs e)
        {
            FileDialog lDialog = new SaveFileDialog();

            if(lDialog.ShowDialog() == DialogResult.OK)
            {
                rootDesignTimeView1.Save(lDialog.FileName);
            }
        }
    }
}
