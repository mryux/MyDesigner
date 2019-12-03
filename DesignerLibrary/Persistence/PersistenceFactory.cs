using DesignerLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace DesignerLibrary.Persistence
{
    class PersistenceFactory
    {
        private PersistenceFactory()
        {
        }

        public static readonly PersistenceFactory Instance = new PersistenceFactory();

        private static readonly Type[] PersistenceTypes = new Type[]
        {
            typeof(LineToolPersistence),
            typeof(PolygonToolPersistence),
            typeof(RectangleToolPersistence),
            typeof(ArcToolPersistence),
            typeof(EllipseToolPersistence),
            typeof(TextToolPersistence),
            typeof(ImageToolPersistence),
            typeof(BarcodePersistence),
            typeof(TextWithLabelToolPersistence),
            typeof(Group4ToolPersistence),
            typeof(TextUpDownToolPersistence),
        };

        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(RootPersistence), PersistenceTypes);

        public IEnumerable<ToolPersistence> Import(DesignerModel model)
        {
            List<ToolPersistence> ret = new List<ToolPersistence>();

            if (string.IsNullOrEmpty(model.Layout))
                return ret;

            using (TextReader reader = new StringReader(model.Layout))
            {
                RootPersistence persistence = Serializer.Deserialize(reader) as RootPersistence;

                ret.AddRange(persistence.Tools);
            }

            return ret;
        }

        public string GetLayout(IEnumerable<ToolPersistence> persistences)
        {
            string ret = string.Empty;

            // DrawingTools
            RootPersistence rootPersist = new RootPersistence()
            {
                Tools = persistences.ToArray()
            };

            using (TextWriter writer = new StringWriter())
            {
                Serializer.Serialize(writer, rootPersist);
                writer.Flush();
                ret = writer.ToString();
            }

            return ret;
        }
    }

    public class RootPersistence
    {
        public ToolPersistence[] Tools { get; set; }
    }
}
