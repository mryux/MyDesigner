using DesignerLibrary.Models;
using DesignerLibrary.Views;
using System;
using System.Configuration;
using System.Drawing.Printing;
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
                PaperSize = new PaperSize("Custom", 400, 600),
            };

            printDocument.PrinterSettings = printerSettings;
            printDocument.DefaultPageSettings = pageSettings;
            printDocument.PrintPage += PrintDocument_PrintPage;
            printDocument.Print();
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs args)
        {
            View.OnPrint(args);
        }
    }
}
