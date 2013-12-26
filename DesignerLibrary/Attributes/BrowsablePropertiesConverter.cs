using System;
using System.ComponentModel;
using System.Linq;

namespace DesignerLibrary.Attributes
{
    class BrowsablePropertiesConverter : ExpandableObjectConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            // collect browsable property descriptors.
            var lProperties = (from propertyDesc in base.GetProperties( context, value, attributes ).OfType<PropertyDescriptor>()
                               where propertyDesc.Attributes.Contains( CustomVisibleAttribute.Yes )
                               orderby GetOrderIndex( propertyDesc )
                               select propertyDesc).ToList();

            // return property descriptors collection with same order of insertion.
            return new PropertyDescriptorCollection( lProperties.ToArray() ).Sort( lProperties.Select( e => e.Name ).ToArray() );
        }

        private int GetOrderIndex(PropertyDescriptor pDescriptor)
        {
            PropertyOrderAttribute lOrderAttribute = pDescriptor.Attributes.OfType<PropertyOrderAttribute>().FirstOrDefault();
            int lRet = 0;

            if (lOrderAttribute != null)
                lRet = lOrderAttribute.Order;

            return lRet;
        }
    }
}
