using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace DesignerLibrary.Helpers
{
    public class SiPropertyDescriptor : PropertyDescriptor
    {
        private object Component { get; set; }

        public SiPropertyDescriptor(object pComponent, string pName, Attribute[] pAttributes)
            : base( pName, pAttributes )
        {
            PropertyInfo lPropertyInfo = pComponent.GetType().GetProperty( pName );

            if (lPropertyInfo == null)
                throw new ArgumentException( string.Format( "Name:{0} is not a valid member of Type:{1}", pName, pComponent.GetType().ToString() ) );

            Component = pComponent;
            _Name = pName;

            DisplayNameAttribute lNameAttribute = pAttributes.First( e => e is DisplayNameAttribute ) as DisplayNameAttribute;

            if (lNameAttribute != null)
                _DisplayName = lNameAttribute.DisplayName;

            CategoryAttribute lCategoryAttribute = pAttributes.First( e => e is CategoryAttribute ) as CategoryAttribute;
            if (lCategoryAttribute != null)
                _Category = lCategoryAttribute.Category;
        }
        
        public override Type ComponentType
        {
            get { return Component.GetType(); }
        }

        public override object GetValue(object component)
        {
            return ComponentType.GetProperty( _Name ).GetValue( Component, null );
        }

        public override Type PropertyType
        {
            get
            {
                return ComponentType.GetProperty( _Name ).PropertyType;
            }
        }

        public override void SetValue(object component, object value)
        {
            ComponentType.GetProperty( _Name ).SetValue( Component, value, null );
        }

        private string _Category;
        public override string Category
        {
            get { return _Category; }
        }

        private string _Name;
        public override string Name
        {
            get { return _Name; }
        }

        private string _DisplayName;
        public override string DisplayName
        {
            get { return string.IsNullOrEmpty( _DisplayName ) ? Name : _DisplayName; }
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override void ResetValue(object component)
        {            
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }
    }
}
