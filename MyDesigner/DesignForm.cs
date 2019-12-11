﻿using DesignerLibrary.Helpers;
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

        void FireEvent_LoadModel(string path, DesignerModel model)
        {
            if (LoadModelEvent != null)
                LoadModelEvent( this, new EventArgs<Tuple<string, DesignerModel>>( new Tuple<string, DesignerModel>( path, model ) ) );
        }

        void FireEvent_SaveModel()
        {
            if (SaveModelEvent != null)
                SaveModelEvent( this, EventArgs.Empty );
        }

        private void OnNew(object sender, EventArgs e)
        {
            FireEvent_LoadModel(null, new DesignerModel());
        }

        void OnOpen(object sender, EventArgs e)
        {
            FileDialog dialog = new OpenFileDialog()
            {
                CheckFileExists = true
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FireEvent_LoadModel(dialog.FileName, DesignerModel.FromFile(dialog.FileName));
            }
        }

        void OnSave(object sender, EventArgs e)
        {
            FireEvent_SaveModel();
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintDialog dialog = new PrintDialog();
            PrintDocument doc = new PrintDocument();

            dialog.Document = doc;
            doc.PrintPage += OnPrintPage;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                doc.Print();
            }
        }

        void OnPrintPage(object sender, PrintPageEventArgs args)
        {
            rootDesignTimeView1.OnPrint( args );
        }

        private void runtimeModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RuntimeForm form = new RuntimeForm();

            form.Model = rootDesignTimeView1.Model;
            form.Owner = this;
            form.Show( this );
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

        private bool PenAsTransparent { get; set; }
        private void togglePenTransparentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PenAsTransparent = !PenAsTransparent;
            rootDesignTimeView1.SetPenAsTransparent(PenAsTransparent);
        }
    }
}
