using System.Collections.Generic;
using System.Drawing;

namespace DesignerLibrary.Converters
{
    class AlignmentConverter : EnumTypeConverter<StringAlignment>
    {
        private static readonly Dictionary<StringAlignment, string> _Dict = new Dictionary<StringAlignment, string>()
        {
            { StringAlignment.Near, "Left" },
            { StringAlignment.Center, "Center" },
            { StringAlignment.Far, "Right" },
        };

        public AlignmentConverter()
            : base( _Dict )
        {
        }
    }

    class VAlignmentConverter : EnumTypeConverter<StringAlignment>
    {
        private static readonly Dictionary<StringAlignment, string> _Dict = new Dictionary<StringAlignment, string>()
        {
            { StringAlignment.Near, "Top" },
            { StringAlignment.Center, "Center" },
            { StringAlignment.Far, "Bottom" },
        };

        public VAlignmentConverter()
            : base( _Dict )
        {
        }
    }
}
