using System.Collections.Generic;
using DesignerLibrary.Persistence;

namespace DesignerLibrary.Converters
{
    class LineWidthConverter : EnumTypeConverter<LineWidth>
    {
        private static readonly Dictionary<LineWidth, string> _Dict = new Dictionary<LineWidth, string>()
        {
            { LineWidth.Thin, Properties.Resources.LineWidth_Thin },
            { LineWidth.Medium, Properties.Resources.LineWidth_Medium },
            { LineWidth.Thick, Properties.Resources.LineWidth_Thick },
        };

        public LineWidthConverter()
            : base( _Dict )
        {
        }
    }
}
