using System;
using System.IO;

namespace DesignerLibrary.Persistence
{
    public class TextUpDownToolPersistence : TextToolPersistence
    {
        public TextUpDownToolPersistence()
            : base(typeof(DrawingTools.TextUpDownTool))
        {
        }

        protected TextUpDownToolPersistence(Type type)
            : base(type)
        {
        }

        public string BottomRight { get; set; }

        protected override void OnDeserialize(BinaryReader reader)
        {
            base.OnDeserialize(reader);

            BottomRight = reader.ReadString();
        }
    }
}
