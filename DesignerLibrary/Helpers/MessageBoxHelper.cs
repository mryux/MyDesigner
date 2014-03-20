using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DesignerLibrary.Helpers
{
    class MessageBoxHelper
    {
        public static DialogResult OKCancelMessage(string pMessage)
        {
            return MessageBox.Show( pMessage, "", MessageBoxButtons.OKCancel );
        }
    }
}
