using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace DesignerLibrary.Helpers
{
    public class DragImage : IDisposable
    {
        #region IDisposable Members

        public void Dispose()
        {
            Timer.Enabled = false;
            Timer.Dispose();

            ImageList_DragLeave( IntPtr.Zero );
            ImageList_EndDrag();
            ImageList.Dispose();

        }

        #endregion

        #region External

        [DllImport( "COMCTL32.DLL" )]
        static extern bool ImageList_BeginDrag(IntPtr himlTrack, int iTrack, int dxHotspot, int dyHotspot);

        [DllImport( "COMCTL32.DLL" )]
        static extern bool ImageList_DragEnter(IntPtr hwndLock, int x, int y);

        [DllImport( "COMCTL32.DLL" )]
        static extern bool ImageList_DragMove(int x, int y);

        [DllImport( "COMCTL32.DLL" )]
        static extern bool ImageList_DragLeave(IntPtr hwndLock);

        [DllImport( "COMCTL32.DLL" )]
        static extern void ImageList_EndDrag();

        #endregion

        public DragImage(Image pImage, int dxHotspot, int dyHotspot)
        {
            ImageList.TransparentColor = Color.Magenta;
            ImageList.ImageSize = new Size( Math.Min( pImage.Width, 256 ), Math.Min( pImage.Height, 256 ) );
            ImageList.ColorDepth = ColorDepth.Depth32Bit;
            ImageList.Images.Add( pImage );

            ImageList_BeginDrag( ImageList.Handle, 0, dxHotspot, dyHotspot );
            ImageList_DragEnter( IntPtr.Zero, Control.MousePosition.X, Control.MousePosition.Y );
            Timer.Tick += new EventHandler( OnTimerTick );
            Timer.Interval = 10;
            Timer.Enabled = true;
        }

        void OnTimerTick(object sender, EventArgs e)
        {
            ImageList_DragMove( Control.MousePosition.X, Control.MousePosition.Y );
        }

        System.Windows.Forms.ImageList ImageList = new System.Windows.Forms.ImageList();

        System.Windows.Forms.Timer Timer = new System.Windows.Forms.Timer();
    }
}
