using System;
using System.Drawing.Printing;
using System.Windows.Forms;

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

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintDocument lDocument = new PrintDocument();

            lDocument.PrintPage += OnPrintPage;
        }

        void OnPrintPage(object sender, PrintPageEventArgs pArgs)
        {
        }
    }
}
