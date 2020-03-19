namespace DesignerLibrary.Persistence
{
    public class ImageToolPersistence : RectangleToolPersistence
    {
        public ImageToolPersistence()
            : base(typeof(DrawingTools.ImageTool))
        {
        }

        public string ImagePath { get; set; }
    }
}
