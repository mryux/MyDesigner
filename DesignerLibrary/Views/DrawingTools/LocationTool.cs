using System;
using System.Collections.Generic;
using System.ComponentModel;
using SCF.SiPass.Explorer.Common.Attributes;
using SCF.SiPass.Explorer.Common.Helpers;
using SCF.SiPass.SitePlan.Module.Attributes;
using SCF.SiPass.SitePlan.Module.Constants;
using SCF.SiPass.SitePlan.Module.Converters;
using SCF.SiPass.SitePlan.Module.Persistence;
using SCF.SiPass.SitePlan.Module.TypeEditors;
using SiPass.Entities;

namespace SCF.SiPass.SitePlan.Module.DrawingTools
{
    class LocationTool : RectangleTool
    {
        private LocationType _LocationType = LocationType.Point;
        public LocationType LocationType
        {
            get { return _LocationType; }
            set
            {
                _LocationType = value;
                LogicalPoint = null;
            }
        }

        public LogicalPoint LogicalPoint
        {
            get;
            set;
        }

        protected override ToolPersistence NewPersistence()
        {
            return new LocationToolPersistence();
        }

        protected override IList<PropertyDescriptor> GetPropertyDescriptors()
        {
            IList<PropertyDescriptor> lDescriptors = base.GetPropertyDescriptors();

            lDescriptors.Add( new SiPropertyDescriptor( this, PropertyNames.LocationType,
                new Attribute[] 
                { 
                    CustomVisibleAttribute.Yes,
                    new LocalizedCategoryAttribute( "Category_LocationOrGroup" ),
                    new LocalizedDisplayNameAttribute( "DisplayName_LocationType" ),
                    new PropertyOrderAttribute( 100 ),
                    new TypeConverterAttribute( typeof( LocationTypeConverter ) ),
                } ) );

            lDescriptors.Add( new SiPropertyDescriptor( this, PropertyNames.LogicalPoint,
                new Attribute[] 
                { 
                    CustomVisibleAttribute.Yes,
                    new LocalizedCategoryAttribute( "Category_LocationOrGroup" ),
                    new LocalizedDisplayNameAttribute( "DisplayName_LocationName" ),
                    new PropertyOrderAttribute( 101 ),
                    new EditorAttribute( typeof( SitePlanLocationTypeEditor ), typeof( System.Drawing.Design.UITypeEditor ) ),
                } ) );

            return lDescriptors;
        }
    }
}
