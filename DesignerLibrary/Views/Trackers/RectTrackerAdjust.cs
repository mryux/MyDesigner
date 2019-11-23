using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using DesignerLibrary.Helpers;

namespace DesignerLibrary.Trackers
{
    class RectTrackerAdjust : TrackerAdjust
    {
        public RectTrackerAdjust()
        {
        }

        private byte GetIndex(VIndex pVIndex, HIndex pHIndex)
        {
            return (byte)((byte)pVIndex << 4 | (byte)pHIndex);
        }

        public static void GetIndex(byte pIndex, out VIndex pVIndex, out HIndex pHIndex)
        {
            pHIndex = (HIndex)(pIndex & 0x0f);
            pVIndex = (VIndex)(pIndex >> 4 & 0x0f);
        }

        public enum HIndex { eLeft = 1, eCenter, eRight }
        public enum VIndex { eTop = 1, eCenter, eBottom }

        public enum RectPointIndex
        {
            eNone,
            eTopLeft = VIndex.eTop << 4 | HIndex.eLeft,
            eTopCenter = VIndex.eTop << 4 | HIndex.eCenter,
            eTopRight = VIndex.eTop << 4 | HIndex.eRight,
            eMidLeft = VIndex.eCenter << 4 | HIndex.eLeft,
            eMidRight = VIndex.eCenter << 4 | HIndex.eRight,
            eBottomLeft = VIndex.eBottom << 4 | HIndex.eLeft,
            eBottomCenter = VIndex.eBottom << 4 | HIndex.eCenter,
            eBottomRight = VIndex.eBottom << 4 | HIndex.eRight,
        }

        protected override void OnResize(Point pPoint, ref Rectangle pRect)
        {
            Rectangle lRect = pRect;
            Point lLocation = lRect.Location;
            Size lSize = lRect.Size;
            RectPointIndex lMovingPointIndex = (RectPointIndex)MovingPointIndex;

            switch (lMovingPointIndex)
            {
                case RectPointIndex.eTopLeft:
                    lLocation = pPoint;
                    lSize = new Size( lRect.Right - pPoint.X, lRect.Bottom - pPoint.Y );
                    break;

                case RectPointIndex.eTopCenter:
                    lLocation.Y = pPoint.Y;
                    lSize.Height = lRect.Bottom - pPoint.Y;
                    break;

                case RectPointIndex.eTopRight:
                    lLocation.Y = pPoint.Y;
                    lSize = new Size( pPoint.X - lRect.Left, lRect.Bottom - pPoint.Y );
                    break;

                case RectPointIndex.eMidLeft:
                    lLocation.X = pPoint.X;
                    lSize.Width = lRect.Right - pPoint.X;
                    break;

                case RectPointIndex.eMidRight:
                    lSize.Width = pPoint.X - lRect.Left;
                    break;

                case RectPointIndex.eBottomLeft:
                    lLocation.X = pPoint.X;
                    lSize = new Size( lRect.Right - pPoint.X, pPoint.Y - lRect.Top );
                    break;

                case RectPointIndex.eBottomCenter:
                    lSize.Height = pPoint.Y - lRect.Top;
                    break;

                case RectPointIndex.eBottomRight:
                    lSize = new Size( pPoint.X - lRect.Left, pPoint.Y - lRect.Top );
                    break;
            }

            OnRectify( lSize );
            Rectify( lRect, pPoint, ref lLocation, ref lSize, ref lMovingPointIndex );
            MovingPointIndex = (int)lMovingPointIndex;

            // draw circle/square when Control key is pressing
            if (KeyboardHelper.Instance.IsCtrlPressing)
            {
                int lLen = Math.Min( lSize.Width, lSize.Height );

                lSize = new Size( lLen, lLen );
            }

            pRect = new Rectangle( lLocation, lSize );
        }

        protected virtual void OnRectify(Size pSize)
        {
        }

        private void Rectify(Rectangle pRect, Point pPoint, ref Point pLocation, ref Size pSize, ref RectPointIndex pIndex)
        {
            VIndex lVIndex;
            HIndex lHIndex;

            GetIndex( (byte)pIndex, out lVIndex, out lHIndex );

            if (pSize.Height < 0)
            {
                pLocation.Y = lVIndex == VIndex.eTop ? pRect.Bottom : pPoint.Y;

                lVIndex = (lVIndex == VIndex.eTop) ? VIndex.eBottom : VIndex.eTop;
                pSize.Height = -pSize.Height;
            }

            if (pSize.Width < 0)
            {
                pLocation.X = lHIndex == HIndex.eLeft ? pRect.Right : pPoint.X;

                lHIndex = (lHIndex == HIndex.eLeft) ? HIndex.eRight : HIndex.eLeft;
                pSize.Width = -pSize.Width;
            }

            pIndex = (RectPointIndex)GetIndex( lVIndex, lHIndex );
        }
    }
}
