using DesignerLibrary.Models;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DesignerLibrary.Persistence
{
    public class ImageToolPersistence : RectangleToolPersistence
    {
        public ImageToolPersistence()
            : base( typeof( DrawingTools.ImageTool ) )
        {
        }

        public string ImagePath { get; set; }

        protected override void OnToXml(Dictionary<string, string> pImages)
        {
            base.OnToXml( pImages );

            if (!pImages.ContainsKey( base.Name ))
            {
                //pImages[base.Name] = SitePlanModel.FileToXml( ImagePath );
            }
        }

        protected override void OnLoadFromSitePlanModel(SitePlanModel pModel)
        {
            base.OnLoadFromSitePlanModel( pModel );

            //if (pModel.Images.ContainsKey( base.Name ))
            //{
            //    string lSitePlanDir = ToolPersistence.GetSitePlanDir( pModel.Id );

            //    ImagePath = lSitePlanDir + @"\Images\" + base.Name;
            //}
        }
    }
}
