using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace DesignerLibrary.Persistence
{
    public class TextToolPersistence : RectangleToolPersistence
    {
        public TextToolPersistence()
            : this(typeof(DrawingTools.TextTool))
        {
        }

        protected TextToolPersistence(Type type)
            : base(type)
        {
            Alignment = StringAlignment.Near;
            TextColor = Color.Black;
            SetLogFont(new Font(FontFamily.GenericSerif, 10.0f));
        }

        protected override void OnDeserialize(BinaryReader reader)
        {
            base.OnDeserialize(reader);

            PenColor = Color.Transparent;
            Text = ReadString(reader);

            // logfont value (made by c++) in legacy SitePlan is different from the value made by C#.
            LOGFONT_Unicode lLogFont = Read<LOGFONT_Ansi>(reader).ToUnicode();
            Font lFont = Font.FromLogFont(lLogFont);

            lFont = new Font(lFont.FontFamily, lFont.Size, lFont.Style, GraphicsUnit.Point);
            SetLogFont(lFont);

            Alignment = (StringAlignment)Read<int>(reader);
            int lIsUserDefinedSize = Read<int>(reader);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class LOGFONT_Ansi
        {
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string lfFaceName;

            public LOGFONT_Unicode ToUnicode()
            {
                return new LOGFONT_Unicode()
                {
                    lfHeight = this.lfHeight,
                    lfWidth = this.lfWidth,
                    lfEscapement = this.lfEscapement,
                    lfOrientation = this.lfOrientation,
                    lfWeight = this.lfWeight,
                    lfItalic = this.lfItalic,
                    lfUnderline = this.lfUnderline,
                    lfStrikeOut = this.lfStrikeOut,
                    lfCharSet = this.lfCharSet,
                    lfOutPrecision = this.lfOutPrecision,
                    lfClipPrecision = this.lfClipPrecision,
                    lfQuality = this.lfQuality,
                    lfPitchAndFamily = this.lfPitchAndFamily,
                    lfFaceName = this.lfFaceName,
                };
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public class LOGFONT_Unicode
        {
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string lfFaceName;
        }

        public string Text { get; set; }

        [XmlIgnore]
        public Color TextColor { get; set; }

        [XmlElement("TextColor")]
        public int TextColorAsArgb
        {
            get { return TextColor.ToArgb(); }
            set { TextColor = Color.FromArgb(value); }
        }

        public void SetLogFont(Font pFont)
        {
            LOGFONT_Unicode lLogFont = new LOGFONT_Unicode();
            pFont.ToLogFont(lLogFont);

            LogFont = lLogFont;
        }

        public LOGFONT_Unicode LogFont { get; set; }
        public StringAlignment Alignment { get; set; }
    }
}
