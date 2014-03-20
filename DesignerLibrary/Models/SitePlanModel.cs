using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DesignerLibrary.Models
{
    public class SitePlanModel
    {
        public string Name { get; set; }
        public string Layout { get; set; }
        public string Description { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public static SitePlanModel FromFile(string pPath)
        {
            using (TextReader reader = new StreamReader( pPath ))
            {
                XmlSerializer lSerializer = new XmlSerializer( typeof( SitePlanModel ) );

                return lSerializer.Deserialize( reader ) as SitePlanModel;
            }
        }

        public void SaveToFile(string pPath)
        {
            using (TextWriter writer = new StreamWriter( pPath ))
            {
                XmlSerializer lSerializer = new XmlSerializer( typeof( SitePlanModel ) );

                lSerializer.Serialize( writer, this );
            }
        }
    }
}
