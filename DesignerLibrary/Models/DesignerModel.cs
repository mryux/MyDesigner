using System.IO;
using System.Xml.Serialization;

namespace DesignerLibrary.Models
{
    public class DesignerModel
    {
        public string Name { get; set; }
        public string Layout { get; set; }
        public string Description { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public static DesignerModel FromFile(string path)
        {
            using (TextReader reader = new StreamReader(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DesignerModel));

                return serializer.Deserialize(reader) as DesignerModel;
            }
        }

        public void SaveToFile(string path)
        {
            using (TextWriter writer = new StreamWriter(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DesignerModel));

                serializer.Serialize(writer, this);
            }
        }
    }
}
