using DesignerLibrary.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Serialization;

namespace DesignerLibrary.Persistence
{
    class PersistenceFactory
    {
        private PersistenceFactory()
        {
        }

        public static readonly PersistenceFactory Instance = new PersistenceFactory();
        
        protected static readonly Type[] PersistenceTypes = new Type[]
        {
            typeof( LineToolPersistence ),
            typeof( PolygonToolPersistence ),
            typeof( RectangleToolPersistence ),
            typeof( ArcToolPersistence ),
            typeof( EllipseToolPersistence ),
            typeof( TextToolPersistence ),
            typeof( ImageToolPersistence ),
        };
                
        public IEnumerable<ToolPersistence> Import(SitePlanModel pModel)
        {
            List<ToolPersistence> lRet = new List<ToolPersistence>();

            if (!string.IsNullOrEmpty( pModel.Layout ))
            {
                XmlSerializer lSerializer = new XmlSerializer( typeof( SitePlanTools ), PersistenceTypes );

                using (TextReader reader = new StringReader( pModel.Layout ))
                {
                    SitePlanTools lTools = lSerializer.Deserialize( reader ) as SitePlanTools;

                    lRet.AddRange( lTools.Tools );
                }
            }

            return lRet;
        }

        public string GetLayout(IEnumerable<ToolPersistence> pPersistences)
        {
            string lRet = string.Empty;

            // DrawingTools
            XmlSerializer lSerializer = new XmlSerializer( typeof( SitePlanTools ), PersistenceTypes );
            SitePlanTools lTools = new SitePlanTools()
            {
                Tools = pPersistences.ToArray()
            };

            using (TextWriter writer = new StringWriter())
            {
                lSerializer.Serialize( writer, lTools );
                writer.Flush();
                lRet= writer.ToString();
            }

            return lRet;
        }
    }

    public class SitePlanTools
    {
        public ToolPersistence[] Tools { get; set; }
    }
}
