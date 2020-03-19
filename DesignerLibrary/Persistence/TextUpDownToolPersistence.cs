using System;

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

        public bool AlignRight { get; set; }
    }
}
