using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Serialization;

namespace DesignerLibrary.Persistence
{
    public enum LineWidth { Thin, Medium, Thick }

    public abstract class ToolPersistence
    {
        protected ToolPersistence()
        {
            PenColor = Color.Black;
            PenWidth = LineWidth.Thin;
            Location = Point.Empty;
        }

        internal DrawingTools.DrawingTool CreateDrawingTool()
        {
            return NewDrawingTool();
        }

        internal abstract DrawingTools.DrawingTool NewDrawingTool();

        protected virtual void OnRectDeserialized(Rectangle pRect) { }
        protected virtual void OnFillColorDeserialized(Color pColor) { }
        protected virtual void OnDeserialize(BinaryReader pReader)
        {
            int lVersion = Read<int>( pReader );
        }

        [XmlIgnore]
        public Color PenColor { get; set; }

        [XmlElement("PenColor")]
        public int PenColorAsArgb
        {
            get { return PenColor.ToArgb(); }
            set { PenColor = Color.FromArgb( value ); }
        }

        public LineWidth PenWidth { get; set; }
        public Point Location { get; set; }

        public void Deserialize(BinaryReader pReader)
        {
            // version
            int lVersion = Read<int>( pReader );

            // rect
            Rectangle lRect = ReadRectangle( pReader );
            OnRectDeserialized( lRect );

            // foreColor
            PenColor = ReadColor( pReader );

            // filled
            bool lFilled = Convert.ToBoolean( Read<int>( pReader ) );
            // backColor
            Color lFillColor = ReadColor( pReader );

            OnFillColorDeserialized( lFilled ? lFillColor : Color.Transparent );

            // PenWidth
            PenWidth = ReadLineWidth( pReader );

            OnDeserialize( pReader );
        }

        public static T Read<T>(BinaryReader pReader)
        {
            byte[] bytes = pReader.ReadBytes( Marshal.SizeOf( typeof( T ) ) );

            GCHandle lHandle = GCHandle.Alloc( bytes, GCHandleType.Pinned );
            T lRet = (T)Marshal.PtrToStructure( lHandle.AddrOfPinnedObject(), typeof( T ) );
            lHandle.Free();

            return lRet;
        }

        public string ReadString(BinaryReader pReader)
        {
            byte lbLen = Read<byte>( pReader );

            // handle string up to 255 length
            if (lbLen < byte.MaxValue)
                return ReadString( pReader, lbLen );

            // handle string up to 2 bytes length
            ushort lwLen = Read<ushort>( pReader );

            if (lwLen < ushort.MaxValue)
                return ReadString( pReader, lwLen );

            int liLen = Read<int>( pReader );

            if (liLen < int.MaxValue)
                return ReadString( pReader, liLen );

            throw new InvalidOperationException( string.Format( "strings are supported up to {0} bytes length", int.MaxValue ) );
        }

        public static string ReadString(BinaryReader pReader, int pLength)
        {
            string lRet = string.Empty;
            byte[] lBytes = pReader.ReadBytes( pLength );

            lRet = Encoding.ASCII.GetString( lBytes );

            return lRet;
        }

        protected Rectangle ReadRectangle(BinaryReader pReader)
        {
            int lX = Read<int>( pReader );
            int lY = Read<int>( pReader );
            int lWidth = Read<int>( pReader ) - lX;
            int lHeight = Read<int>( pReader ) - lY;

            return new Rectangle( lX, -lY, lWidth, -lHeight );
        }

        protected Color ReadColor(BinaryReader pReader)
        {
            int lColor = Read<int>( pReader );
            byte r = (byte)(lColor & 0xff);
            byte g = (byte)(lColor >> 8 & 0xff);
            byte b = (byte)(lColor >> 16 & 0xff);

            return Color.FromArgb( r, g, b );
        }

        private LineWidth ReadLineWidth(BinaryReader pReader)
        {
            return (LineWidth)Read<int>( pReader );
        }

        public static Point ReadPoint(BinaryReader pReader)
        {
            int lX = Read<int>( pReader );
            int lY = Read<int>( pReader );

            return new Point( lX, -lY );
        }
    }
}
