namespace DesignerLibrary.Persistence
{
    public class Group4ToolPersistence : TextUpDownToolPersistence
    {
        public Group4ToolPersistence()
            : base(typeof(DrawingTools.Group4Tool))
        {
        }

        public string BottomLeft { get; set; }
        public string TopRight { get; set; }
    }
}
