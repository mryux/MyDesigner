using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DesignerLibrary.Trackers
{
    abstract class TrackerAdjust
    {
        public int MovingPointIndex { get; set; }

        protected abstract void OnResize(Point pPoint, ref Rectangle pRect);

        public void Resize(Point pPoint, ref Rectangle pRect)
        {
            OnResize( pPoint, ref pRect );
        }
    }
}
