using DesignerLibrary.Helpers;
using DesignerLibrary.Models;
using System;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace MyDesigner
{
    public partial class DesignForm : Form
    {
        public DesignForm()
        {
            InitializeComponent();
        }

        private event EventHandler<EventArgs<Tuple<string, DesignerModel>>> LoadModelEvent;
        private event EventHandler SaveModelEvent;

        protected override void OnLoad(EventArgs pArgs)
        {
            base.OnLoad( pArgs );

            LoadModelEvent += rootDesignTimeView1.OnLoadModel;
            SaveModelEvent += rootDesignTimeView1.OnSaveModel;

            OnNew( this, EventArgs.Empty );
        }

        void FireEvent_LoadModel(string pTitle, DesignerModel pModel)
        {
            if (LoadModelEvent != null)
                LoadModelEvent( this, new EventArgs<Tuple<string, DesignerModel>>( new Tuple<string, DesignerModel>( pTitle, pModel ) ) );
        }

        void FireEvent_SaveModel()
        {
            if (SaveModelEvent != null)
                SaveModelEvent( this, EventArgs.Empty );
        }

        private void OnNew(object sender, EventArgs e)
        {
            FireEvent_LoadModel( "New", new DesignerModel() );
        }

        void OnOpen(object sender, EventArgs e)
        {
            FileDialog lDialog = new OpenFileDialog();

            lDialog.CheckFileExists = true;
            if(lDialog.ShowDialog() == DialogResult.OK)
            {
                string lTitle = System.IO.Path.GetFileName( lDialog.FileName );

                FireEvent_LoadModel( lTitle, DesignerModel.FromFile( lDialog.FileName ) );
            }
        }

        void OnSave(object sender, EventArgs e)
        {
            FireEvent_SaveModel();
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintDialog lDialog = new PrintDialog();

            PrintDocument lDocument = new PrintDocument();

            lDialog.Document = lDocument;
            lDocument.PrintPage += OnPrintPage;
            if (lDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                lDocument.Print();
            }
        }

        void OnPrintPage(object sender, PrintPageEventArgs pArgs)
        {
            rootDesignTimeView1.OnPrint( pArgs );
        }

        private void runtimeModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RuntimeForm lForm = new RuntimeForm();

            lForm.Model = rootDesignTimeView1.Model;
            lForm.Owner = this;
            lForm.Show( this );
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
