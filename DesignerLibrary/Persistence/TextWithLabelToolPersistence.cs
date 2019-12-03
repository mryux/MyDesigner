using System.IO;

namespace DesignerLibrary.Persistence
{
    public class TextWithLabelToolPersistence : TextToolPersistence
    {
        public TextWithLabelToolPersistence()
            : base(typeof(DrawingTools.TextWithLabelTool))
        {
        }

        public string Label { get; set; }

        protected override void OnDeserialize(BinaryReader reader)
        {
            base.OnDeserialize(reader);

            Label = reader.ReadString();
        }
    }
}
