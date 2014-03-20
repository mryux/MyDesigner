
using System.Windows.Forms;
namespace DesignerLibrary.Helpers
{
    class WinMessageHelper
    {
        private WinMessageHelper()
        {
        }

        public static readonly WinMessageHelper Instance = new WinMessageHelper();

        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;

        public void PostMessage_MouseMove(Control pControl)
        {
        }
    }
}
