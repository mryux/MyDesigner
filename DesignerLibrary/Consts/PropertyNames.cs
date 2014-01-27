using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesignerLibrary.Consts
{
    public enum PropertyOrder
    {
        eNone,
        eLocation = 1,
        eBounds = 2,
        eLineColor = 5,
        eFillColor = 6,
        eLineWidth = 7,
        eFileLocation = 10,
    }

    class NameConsts
    {
        public static readonly string Pointer = "Pointer";
    }

    class PropertyNames
    {
        public static readonly string LayerName = "LayerName";

        public static readonly string Location = "Location";
        public static readonly string Bounds = "Bounds";
        public static readonly string PenColor = "PenColor";
        public static readonly string PenWidth = "PenWidth";
        public static readonly string FillColor = "FillColor";
        public static readonly string FileLocation = "FileLocation";

        public static readonly string LocationType = "LocationType";
        public static readonly string LogicalPoint = "LogicalPoint";
    }
}
