﻿namespace DesignerLibrary.Consts
{
    public enum PropertyOrder
    {
        eNone,
        eId = 1,
        eName = 2,
        eLocation = 3,
        eBounds = 4,
        eLineColor = 5,
        eFillColor = 6,
        eLineWidth = 7,
        eFileLocation = 10,

        Label = 18,
        Text = 19,
        TextColor = 20,
        Barcode = 21,
        Font = 22,
        Alignment = 23,
        VAlignment = 24,
        TimeFormat = 25,
        Value = 26,
        BottomLeft = 27,
        BottomRight = 28,
        TopRight = 29,
        AlignRight = 30,

        eLogicalPointType = 100,
        eLogicalPointName = 101,

        eAction = 200,
    }

    class NameConsts
    {
        public static readonly string Pointer = "Pointer";
    }

    class PropertyNames
    {
        public static readonly string LayerName = "LayerName";

        public static readonly string Id = "Id";
        public static readonly string Name = "Name";
        public static readonly string Location = "Location";
        public static readonly string Bounds = "Bounds";
        public static readonly string PenColor = "PenColor";
        public static readonly string PenWidth = "PenWidth";
        public static readonly string FillColor = "FillColor";
        public static readonly string FileLocation = "FileLocation";

        public static readonly string LocationType = "LocationType";
        public static readonly string LogicalPoint = "LogicalPoint";

        public static readonly string Action = "Action";

        public static readonly string Text = "Text";
        public static readonly string Label = "Label";
        public static readonly string TopRight = "TopRight";
        public static readonly string BottomLeft = "BottomLeft";
        public static readonly string BottomRight = "BottomRight";
        public static readonly string TextColor = "TextColor";
        public static readonly string Font = "Font";
        public static readonly string Alignment = "Alignment";
        public static readonly string AlignRight = "AlignRight";
        public static readonly string Barcode = "Barcode";

        public static readonly string ControlType = "ControlType";
        public static readonly string VAlignment = "VAlignment";
        public static readonly string TimeFormat = "TimeFormat";
    }
}
