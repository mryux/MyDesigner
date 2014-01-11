
namespace DesignerLibrary.Persistence
{
    public class ImageToolPersistence : RectangleToolPersistence
    {
        public ImageToolPersistence()
        {
        }

        internal override DrawingTools.DrawingTool NewDrawingTool()
        {
            return new DrawingTools.ImageTool();
        }
    }
}
