using System.ComponentModel;

namespace DesignerLibrary.Attributes
{
    class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        public LocalizedDisplayNameAttribute(string pDisplayName)
            : base( pDisplayName )
        {
        }
    }
}
