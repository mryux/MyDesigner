using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesignerLibrary.Attributes
{
	public class PropertyOrderAttribute : Attribute
	{
        public PropertyOrderAttribute(int pOrder)
        {
            Order = pOrder;
        }

        public int Order { get; set; }
	}
}
