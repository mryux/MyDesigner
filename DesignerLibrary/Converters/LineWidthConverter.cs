using DesignerLibrary.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace DesignerLibrary.Converters
{
    class LineWidthConverter : TypeConverter
    {
        public enum LineWidth { Thin, Medium, Thick }

        private static readonly Dictionary<LineWidth, string> TypeDict = new Dictionary<LineWidth, string>()
        {
            { LineWidth.Thin, Resources.LineWidth_Thin },
            { LineWidth.Medium, Resources.LineWidth_Medium },
            { LineWidth.Thick, Resources.LineWidth_Thick },
        };

        public override bool CanConvertFrom(ITypeDescriptorContext pContext, Type pSourceType)
        {
            return pSourceType == typeof( string );
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object pValue)
        {
            string lValue = pValue as string;
            LineWidth lRet = LineWidth.Thick;

            if (TypeDict.ContainsValue( lValue ))
                lRet = TypeDict.First( e => e.Value == lValue ).Key;

            return lRet;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object pValue, Type pDestinationType)
        {
            object lRet;

            if (pDestinationType == typeof( string )
                && pValue.GetType() == typeof( LineWidth ))
            {
                string lText = string.Format( "LineWidth_{0}", pValue.ToString() );

                lRet = Resources.ResourceManager.GetString( lText, culture );
            }
            else
                lRet = base.ConvertTo( context, culture, pValue, pDestinationType );

            return lRet;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var lValues = (from pair in TypeDict
                           select pair.Value).ToList();

            return new StandardValuesCollection( lValues );
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
