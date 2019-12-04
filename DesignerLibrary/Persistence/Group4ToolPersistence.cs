using System.IO;

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

        protected override void OnDeserialize(BinaryReader reader)
        {
            base.OnDeserialize(reader);

            BottomLeft = reader.ReadString();
            TopRight = reader.ReadString();
        }
    }
}
