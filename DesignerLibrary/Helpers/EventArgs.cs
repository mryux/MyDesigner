using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesignerLibrary.Helpers
{
    public class EventArgs<T> : EventArgs
    {
        public EventArgs(T pData)
        {
            Data = pData;
        }

        public T Data { get; set; }
    }
}
