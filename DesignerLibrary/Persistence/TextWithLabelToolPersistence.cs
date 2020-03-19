namespace DesignerLibrary.Persistence
{
    public class TextWithLabelToolPersistence : TextToolPersistence
    {
        public TextWithLabelToolPersistence()
            : base(typeof(DrawingTools.TextWithLabelTool))
        {
        }

        public string Label { get; set; }
    }
}
