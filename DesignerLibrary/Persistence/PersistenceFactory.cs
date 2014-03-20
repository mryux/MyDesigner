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

        [StructLayout( LayoutKind.Explicit )]
        struct SitePlanHeader
        {
            [FieldOffset( 0 )]
            public Int32 Version;

            [FieldOffset( 4 )]
            public Int32 Revision;
        }

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

        private Dictionary<string, Func<ToolPersistence>> PersistenceDict = new Dictionary<string, Func<ToolPersistence>>()
        {
            { "LineItem", () => new LineToolPersistence() },
            { "PolygonItem", () => new PolygonToolPersistence() },
            { "RectItem", () => new RectangleToolPersistence() },
            { "ArcItem", () => new ArcToolPersistence() },
            { "EllipseItem", () => new EllipseToolPersistence() },
            { "TextItem", () => new TextToolPersistence() },
        };

        public void Import(string pPath, SitePlanModel pModel)
        {
            _LoadArray.Clear();

            using (BinaryReader reader = new BinaryReader(new FileStream( pPath, FileMode.Open, FileAccess.Read ), Encoding.ASCII))
            {
                SitePlanHeader lHeader = ToolPersistence.Read<SitePlanHeader>( reader );

                // read drawing tools
                UInt16 lToolSum = ToolPersistence.Read<UInt16>( reader );
                List<ToolPersistence> lPersistences = new List<ToolPersistence>();

                for (int i = 0; i < lToolSum; i++)
                {
                    lPersistences.Add( NewToolPersistence( reader ) );
                }

                // read page infos
                Point lSize = ToolPersistence.ReadPoint( reader );
                int lPageSum = ToolPersistence.Read<int>( reader );
                int lPage1 = ToolPersistence.Read<int>( reader );
                int lPage2 = ToolPersistence.Read<int>( reader );

                pModel.Width = lSize.X;
                pModel.Height = lSize.Y;
                pModel.Layout = GetLayout( lPersistences );
                pModel.Description = string.Format( "import from {0}", Path.GetFileName( pPath ) );

                // read library items
                UInt16 lLibraryItemSum = ToolPersistence.Read<UInt16>( reader );
                for (int i = 0; i < lLibraryItemSum; i++)
                {

                }
            }
        }

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

        private const ushort NewClassTag = 0xFFFF;
        private List<Func<ToolPersistence>> _LoadArray = new List<Func<ToolPersistence>>();

        private ToolPersistence NewToolPersistence(BinaryReader pReader)
        {
            ToolPersistence lRet = null;
            Func<ToolPersistence> lFunc = null;
            ushort lTag = ToolPersistence.Read<ushort>( pReader );

            if (lTag == NewClassTag)
            {
                ToolPersistence.Read<ushort>( pReader );
                // read class name from legacy SitePlan file.
                ushort lLength = ToolPersistence.Read<ushort>( pReader );
                string lType = ToolPersistence.ReadString( pReader, lLength );

                if (!PersistenceDict.ContainsKey( lType ))
                    throw new InvalidOperationException( string.Format( "{0} is not supported!", lType ) );

                lFunc = PersistenceDict[lType];
                _LoadArray.Add( lFunc );
            }
            else
            {
                // get class index from lTag
                int lClassIndex = lTag & 0xff - 1;

                lFunc = _LoadArray[lClassIndex];
                _LoadArray.Add( null );
            }

            if (lFunc != null)
            {
                lRet = lFunc();
                _LoadArray.Add( null );
                // deserialize from pReader
                lRet.Deserialize( pReader );
            }

            return lRet;
        }
    }

    public class SitePlanTools
    {
        public ToolPersistence[] Tools { get; set; }
    }
}
