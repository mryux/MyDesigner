using System;
using System.Drawing;
using System.Windows.Forms;

namespace DesignerLibrary.Helpers
{
    abstract class AutoScrollTimer : Timer
    {
        public enum AutoScrollDirection { eNone, eLeft, eRight, eUp, eDown }
        public enum AutoScrollRedir { eNone, eStop = 1 << 0, eStartNew = 1 << 1 }

        public Point Location { get; private set; }
        protected ScrollableControl ScrollableCtrl { get; set; }

        protected AutoScrollTimer(Point pPoint, ScrollableControl pControl)
        {
            Location = pPoint;
            ScrollableCtrl = pControl;

            Interval = 100;
        }

        protected abstract Point UpdatePosition(Point pPoint);
        protected abstract bool IsEnd();
        protected abstract AutoScrollRedir OnRedir(Point pPoint);
        
        public AutoScrollRedir Redir(Point pPoint)
        {
            return OnRedir( pPoint );
        }
        
        public AutoScrollRedir Redir(AutoScrollTimer pTimer)
        {
            AutoScrollRedir lRet = AutoScrollRedir.eNone;

            if ((AutoScrollDirection)Tag == AutoScrollDirection.eNone)
                lRet = AutoScrollRedir.eStartNew;
            else
            {
                byte lDirThis = (byte)((AutoScrollDirection)Tag - 1);
                byte lDirOther = (byte)((AutoScrollDirection)pTimer.Tag - 1);

                if ((lDirThis & 0x02) == (lDirOther & 0x02))
                {
                    if ((lDirThis & 0x01) != (lDirOther & 0x01))
                        lRet = AutoScrollRedir.eStop;
                }
                else
                    lRet = AutoScrollRedir.eStop | AutoScrollRedir.eStartNew;
            }

            return lRet;
        }

        protected override void OnTick(EventArgs pArgs)
        {
            base.OnTick( pArgs );

            Point lLocation = Point.Empty;

            // update scroll position.
            lLocation.Offset( -ScrollableCtrl.AutoScrollPosition.X, -ScrollableCtrl.AutoScrollPosition.Y );
            lLocation = UpdatePosition( lLocation );
            ScrollableCtrl.AutoScrollPosition = lLocation;

            // stop timer if scroll reachs the end.
            if (IsEnd())
                Stop();
        }
    }

    class DummyTimer : AutoScrollTimer
    {
        public static readonly AutoScrollTimer Dummy = new DummyTimer();

        protected DummyTimer()
            : base( Point.Empty, null )
        {
            Tag = AutoScrollDirection.eNone;
        }

        protected override Point UpdatePosition(Point pPoint)
        {
            return Point.Empty;
        }

        protected override bool IsEnd()
        {
            return true;
        }

        protected override AutoScrollRedir OnRedir(Point pPoint)
        {
            return AutoScrollRedir.eNone;
        }
        
        protected override void OnTick(EventArgs pArgs)
        {
            Stop();
        }
    }

    class AutoScrollLeftTimer : AutoScrollTimer
    {
        public AutoScrollLeftTimer(Point pPoint, ScrollableControl pControl)
            : base( pPoint, pControl )
        {
            Tag = AutoScrollDirection.eLeft;
        }

        protected override Point UpdatePosition(Point pPoint)
        {
            pPoint.X -= 4;
            return pPoint;
        }

        protected override bool IsEnd()
        {
            int lScrollPos = -ScrollableCtrl.AutoScrollPosition.X;

            return lScrollPos <= 0;
        }

        protected override AutoScrollRedir OnRedir(Point pPoint)
        {
            AutoScrollRedir lRet = AutoScrollRedir.eStop;

            if (pPoint.X < Location.X)
                lRet = AutoScrollRedir.eNone;

            return lRet;
        }
    }

    class AutoScrollUpTimer : AutoScrollTimer
    {
        public AutoScrollUpTimer(Point pPoint, ScrollableControl pControl)
            : base( pPoint, pControl )
        {
            Tag = AutoScrollDirection.eUp;
        }

        protected override Point UpdatePosition(Point pPoint)
        {
            pPoint.Y -= 4;
            return pPoint;
        }

        protected override bool IsEnd()
        {
            int lScrollPos = -ScrollableCtrl.AutoScrollPosition.Y;

            return lScrollPos <= 0;
        }

        protected override AutoScrollRedir OnRedir(Point pPoint)
        {
            AutoScrollRedir lRet = AutoScrollRedir.eStop;

            if (pPoint.Y < Location.Y)
                lRet = AutoScrollRedir.eNone;

            return lRet;
        }
    }

    class AutoScrollRightTimer : AutoScrollTimer
    {
        public AutoScrollRightTimer(Point pPoint, ScrollableControl pControl)
            : base( pPoint, pControl )
        {
            Tag = AutoScrollDirection.eRight;
        }

        protected override Point UpdatePosition(Point pPoint)
        {
            pPoint.X += 4;
            return pPoint;
        }

        protected override bool IsEnd()
        {
            int lScrollPos = -ScrollableCtrl.AutoScrollPosition.X;
            int lScrollBarWidth = ScrollableCtrl.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0;

            return lScrollPos + Location.X + lScrollBarWidth >= ScrollableCtrl.AutoScrollMinSize.Width;
        }

        protected override AutoScrollRedir OnRedir(Point pPoint)
        {
            AutoScrollRedir lRet = AutoScrollRedir.eStop;

            if (pPoint.X > Location.X)
                lRet = AutoScrollRedir.eNone;

            return lRet;
        }
    }

    class AutoScrollDownTimer : AutoScrollTimer
    {
        public AutoScrollDownTimer(Point pPoint, ScrollableControl pControl)
            : base( pPoint, pControl )
        {
            Tag = AutoScrollDirection.eDown;
        }

        protected override Point UpdatePosition(Point pPoint)
        {
            pPoint.Y += 4;
            return pPoint;
        }

        protected override bool IsEnd()
        {
            int lScrollPos = -ScrollableCtrl.AutoScrollPosition.Y;
            int lScrollBarHeight = ScrollableCtrl.HorizontalScroll.Visible ? SystemInformation.HorizontalScrollBarHeight : 0;

            return lScrollPos + Location.Y + lScrollBarHeight >= ScrollableCtrl.AutoScrollMinSize.Height;
        }

        protected override AutoScrollRedir OnRedir(Point pPoint)
        {
            AutoScrollRedir lRet = AutoScrollRedir.eStop;

            if (pPoint.Y > Location.Y)
                lRet = AutoScrollRedir.eNone;

            return lRet;
        }
    }
}
