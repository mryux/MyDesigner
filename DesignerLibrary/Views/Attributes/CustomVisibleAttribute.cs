using System;

namespace DesignerLibrary.Attributes
{
    public class CustomVisibleAttribute : Attribute
	{
        public CustomVisibleAttribute(bool pVisible)
        {
            Visible = pVisible;
        }

        public bool Visible { get; set; }

        public static readonly CustomVisibleAttribute No = new CustomVisibleAttribute( false );
        public static readonly CustomVisibleAttribute Yes = new CustomVisibleAttribute( true );
    }
}
