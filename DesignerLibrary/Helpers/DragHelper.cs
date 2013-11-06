using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace DesignerLibrary.Helpers
{
    public class DragHelper
    {
        #region DllImports
        [DllImport( "comctl32.dll" )]
        private static extern bool InitCommonControls();

        [DllImport( "comctl32.dll", CharSet = CharSet.Auto )]
        private static extern bool ImageList_BeginDrag(IntPtr himlTrack, int iTrack, int dxHotspot, int dyHotspot);

        [DllImport( "comctl32.dll", CharSet = CharSet.Auto )]
        private static extern bool ImageList_DragMove(int x, int y);

        [DllImport( "comctl32.dll", CharSet = CharSet.Auto )]
        private static extern void ImageList_EndDrag();

        [DllImport( "comctl32.dll", CharSet = CharSet.Auto )]
        private static extern bool ImageList_DragEnter(IntPtr hwndLock, int x, int y);

        [DllImport( "comctl32.dll", CharSet = CharSet.Auto )]
        private static extern bool ImageList_DragLeave(IntPtr hwndLock);

        [DllImport( "comctl32.dll", CharSet = CharSet.Auto )]
        private static extern bool ImageList_DragShowNolock(bool fShow);
        #endregion

        public static readonly DragHelper Instance = new DragHelper();

        private DragHelper()
        {
            InitCommonControls();
        }

        public void Initialize(Font pFont)
        {
            Size lSize = new Size( 256, 256 );

            _ImageList.ColorDepth = ColorDepth.Depth32Bit;
            _ImageList.TransparentColor = Color.Transparent;
            _ImageList.Images.Clear();
            _ImageList.ImageSize = lSize;

            Bitmap lBmp = new Bitmap( lSize.Width, lSize.Height );// @"C:\Downloads\dev1.jpg" );
            Graphics lGraph = Graphics.FromImage( lBmp );
            Rectangle lRect = new Rectangle( 0, 0, lSize.Width, lSize.Height );

            lGraph.DrawString( "lineXXX", pFont, Brushes.LightGreen, lRect );
            _ImageList.Images.Add( lBmp );
        }

        public bool BeginDrag()
        {
            return ImageList_BeginDrag( _ImageList.Handle, 0, 5, 5 );
        }

        public void EndDrag()
        {
            ImageList_EndDrag();
        }

        public bool DragMove(Point pPoint)
        {
            return ImageList_DragMove( pPoint.X, pPoint.Y );
        }

        public bool DragEnter(Point pPoint)
        {
            return ImageList_DragEnter( _ImageList.Handle, pPoint.X, pPoint.Y );
        }

        public bool DragLeave()
        {
            return ImageList_DragLeave( _ImageList.Handle );
        }

        public bool DragShowNolock(bool pShow)
        {
            return ImageList_DragShowNolock( pShow );
        }

        private ImageList _ImageList = new ImageList();
    }
}
