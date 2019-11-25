using DesignerLibrary.Constants;
using DesignerLibrary.Models;
using DesignerLibrary.Views;
using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;

namespace MyDesigner
{
    public partial class RuntimeForm : Form
    {
        public RuntimeForm()
        {
            InitializeComponent();
        }

        public DesignerModel Model { get; set; }
        private IRuntimeView View { get; set; }

        protected override void OnLoad(EventArgs args)
        {
            base.OnLoad(args);

            View = RuntimeViewFactory.Instance.NewRuntimeView();

            View.Load(Model);

            Control control = View as Control;

            control.Dock = DockStyle.Fill;
            Controls.Add(control);
        }

        private void toolStripMenuItemPrint_Click(object sender, EventArgs args)
        {
            OnPrintView((o, a) =>
            {
                View.OnPrint(a);
            });
        }

        private void OnPrintView(Action<object, PrintPageEventArgs> printPage)
        {
            PrintDocument printDocument = new PrintDocument();

            PrinterSettings printerSettings = new PrinterSettings
            {
                PrinterName = ConfigurationManager.AppSettings.Get("Printer"),
            };

            // Create our page settings for the paper size selected
            PageSettings pageSettings = new PageSettings(printerSettings)
            {
                Margins = new Margins(0, 0, 0, 0),
                Landscape = false,
                PaperSize = new PaperSize("Custom", ViewConsts.Width, ViewConsts.Height),
            };

            printDocument.PrinterSettings = printerSettings;
            printDocument.DefaultPageSettings = pageSettings;
            printDocument.PrintPage += (o, a) => printPage(o, a);
            printDocument.Print();
        }

        private void saveAsBmpToolStripMenuItem_Click(object sender, EventArgs args)
        {
            FileDialog dialog = new SaveFileDialog()
            {
                Filter = "Bitmap files (*.bmp)|*.bmp",
            };

            if (dialog.ShowDialog(this) == DialogResult.Cancel)
                return;

            CopytoImage().Save(dialog.FileName, ImageFormat.Bmp);
        }

        private Image CopytoImage()
        {
            Image image = new Bitmap(ViewConsts.Width, ViewConsts.Height);
            Graphics graph = Graphics.FromImage(image);

            graph.Clear(Color.White);
            View.OnDraw(new PaintEventArgs(graph, Rectangle.Empty));

            return image;
        }

        private void printAsBmpToolStripMenuItem_Click(object sender, EventArgs args)
        {
            using (Image image = CopytoImage())
            {
                OnPrintView((o, a) =>
                {
                    Graphics g = a.Graphics;

                    g.DrawImage(image, 0, 0);
                });
            }
        }
    }
}
