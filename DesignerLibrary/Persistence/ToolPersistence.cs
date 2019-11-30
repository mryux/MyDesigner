using DesignerLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Serialization;

namespace DesignerLibrary.Persistence
{
    public enum LineWidth { Thin, Medium, Thick }

    public abstract class ToolPersistence
    {
        protected ToolPersistence(Type toolType)
        {
            ToolType = toolType;

            PenColor = Color.Black;
            PenWidth = LineWidth.Thin;
            Location = Point.Empty;
        }

        private Type ToolType { get; set; }

        internal DrawingTools.DrawingTool CreateDrawingTool(IDesignerHost designerHost)
        {
            DrawingTools.DrawingTool ret = null;

            // construct tool via DesignerHost at DesignTime.
            if (designerHost != null)
                ret = designerHost.CreateComponent(ToolType) as DrawingTools.DrawingTool;
            else
            {
                // construct tool via reflection at Runtime.
                ConstructorInfo ci = ToolType.GetConstructor(new Type[] { });

                ret = ci.Invoke(null) as DrawingTools.DrawingTool;
            }

            ret.Persistence = this;
            return ret;
        }

        protected virtual void OnRectDeserialized(Rectangle rect) { }
        protected virtual void OnFillColorDeserialized(Color color) { }
        protected virtual void OnToXml(Dictionary<string, string> map) { }
        protected virtual void OnDeserialize(BinaryReader reader)
        {
            int lVersion = Read<int>(reader);
        }

        [XmlIgnore]
        public Color PenColor { get; set; }

        [XmlElement("PenColor")]
        public int PenColorAsArgb
        {
            get { return PenColor.ToArgb(); }
            set { PenColor = Color.FromArgb(value); }
        }

        [XmlAttribute(AttributeName = "Id")]
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        public LineWidth PenWidth { get; set; }
        public Point Location { get; set; }

        public void ToXml(Dictionary<string, string> map)
        {
            OnToXml(map);
        }

        public void Deserialize(BinaryReader reader)
        {
            // version
            int lVersion = Read<int>(reader);

            // rect
            Rectangle lRect = ReadRectangle(reader);
            OnRectDeserialized(lRect);

            // foreColor
            PenColor = ReadColor(reader);

            // filled
            bool lFilled = Convert.ToBoolean(Read<int>(reader));
            // backColor
            Color lFillColor = ReadColor(reader);

            OnFillColorDeserialized(lFilled ? lFillColor : Color.Transparent);

            // PenWidth
            PenWidth = ReadLineWidth(reader);

            OnDeserialize(reader);
        }

        public static T Read<T>(BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));

            GCHandle lHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T lRet = (T)Marshal.PtrToStructure(lHandle.AddrOfPinnedObject(), typeof(T));
            lHandle.Free();

            return lRet;
        }

        public string ReadString(BinaryReader reader)
        {
            byte lbLen = Read<byte>(reader);

            // handle string up to 255 length
            if (lbLen < byte.MaxValue)
                return ReadString(reader, lbLen);

            // handle string up to 2 bytes length
            ushort lwLen = Read<ushort>(reader);

            if (lwLen < ushort.MaxValue)
                return ReadString(reader, lwLen);

            int liLen = Read<int>(reader);

            if (liLen < int.MaxValue)
                return ReadString(reader, liLen);

            throw new InvalidOperationException(string.Format("strings are supported up to {0} bytes length", int.MaxValue));
        }

        public static string ReadString(BinaryReader reader, int length)
        {
            string lRet = string.Empty;
            byte[] lBytes = reader.ReadBytes(length);

            lRet = Encoding.ASCII.GetString(lBytes);

            return lRet;
        }

        protected Rectangle ReadRectangle(BinaryReader reader)
        {
            int lX = Read<int>(reader);
            int lY = Read<int>(reader);
            int lWidth = Read<int>(reader) - lX;
            int lHeight = Read<int>(reader) - lY;

            return new Rectangle(lX, -lY, lWidth, -lHeight);
        }

        protected Color ReadColor(BinaryReader reader)
        {
            int lColor = Read<int>(reader);
            byte r = (byte)(lColor & 0xff);
            byte g = (byte)(lColor >> 8 & 0xff);
            byte b = (byte)(lColor >> 16 & 0xff);

            return Color.FromArgb(r, g, b);
        }

        private LineWidth ReadLineWidth(BinaryReader reader)
        {
            return (LineWidth)Read<int>(reader);
        }

        public static Point ReadPoint(BinaryReader reader)
        {
            int lX = Read<int>(reader);
            int lY = Read<int>(reader);

            return new Point(lX, -lY);
        }
    }
}
