
namespace DesignerLibrary.Persistence
{
    public class EllipseToolPersistence : RectangleToolPersistence
    {
        public EllipseToolPersistence()
        {
        }

        internal override DrawingTools.DrawingTool NewDrawingTool()
        {
            return new DrawingTools.EllipseTool();
        }
    }
}
