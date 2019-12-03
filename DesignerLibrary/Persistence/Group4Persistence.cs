using System.IO;

namespace DesignerLibrary.Persistence
{
    public class Group4Persistence : TextToolPersistence
    {
        public Group4Persistence()
            : base(typeof(DrawingTools.Group4Tool))
        {
        }

        public string BottomLeft { get; set; }
        public string BottomRight { get; set; }
        public string TopRight { get; set; }

        protected override void OnDeserialize(BinaryReader reader)
        {
            base.OnDeserialize(reader);

            BottomLeft = reader.ReadString();
            BottomRight = reader.ReadString();
            TopRight = reader.ReadString();
        }
    }
}
