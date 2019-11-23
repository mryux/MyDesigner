using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace DesignerLibrary.Converters
{
    abstract class EnumTypeConverter<T> : TypeConverter
    {
        protected EnumTypeConverter(Dictionary<T, string> pDict)
        {
            TypeDict = pDict;
        }

        private Dictionary<T, string> TypeDict { get; set; }

        public override bool CanConvertFrom(ITypeDescriptorContext pContext, Type pSourceType)
        {
            return pSourceType == typeof( string );
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object pValue)
        {
            string lValue = pValue as string;
            T lRet = default( T );

            if (TypeDict.ContainsValue( lValue ))
                lRet = TypeDict.First( e => e.Value == lValue ).Key;

            return lRet;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object pValue, Type pDestinationType)
        {
            object lRet;

            if (pDestinationType == typeof( string )
                && pValue.GetType() == typeof( T ))
            {
                lRet = TypeDict[(T)pValue];
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
