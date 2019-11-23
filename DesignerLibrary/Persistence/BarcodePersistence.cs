using System.Drawing;

namespace DesignerLibrary.Persistence
{
    public class BarcodePersistence : RectangleToolPersistence
    {
        public BarcodePersistence()
            : base(typeof(DrawingTools.BarcodeTool))
        {
            Barcode = "012345abcdef";
            Size = new Size(200, 150);
        }

        public string Barcode { get; set; }
    }
}
