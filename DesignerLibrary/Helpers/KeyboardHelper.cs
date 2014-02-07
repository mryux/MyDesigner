using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesignerLibrary.Helpers
{
    class KeyboardHelper
    {
        protected KeyboardHelper()
        {

        }

        public static readonly KeyboardHelper Instance = new KeyboardHelper();

        public bool CtrlPressed { get; set; }
    }
}
