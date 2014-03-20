using System.ComponentModel;

namespace DesignerLibrary.Attributes
{
    class LocalizedCategoryAttribute : CategoryAttribute
    {
        public LocalizedCategoryAttribute(string pDisplayName)
            : base( pDisplayName )
        {
        }
    }
}
