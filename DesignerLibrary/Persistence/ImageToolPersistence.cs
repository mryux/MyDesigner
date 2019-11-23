using System.Collections.Generic;

namespace DesignerLibrary.Persistence
{
    public class ImageToolPersistence : RectangleToolPersistence
    {
        public ImageToolPersistence()
            : base(typeof(DrawingTools.ImageTool))
        {
        }

        public string ImagePath { get; set; }

        protected override void OnToXml(Dictionary<string, string> images)
        {
            base.OnToXml(images);

            if (!images.ContainsKey(base.Name))
            {
                //pImages[base.Name] = SitePlanModel.FileToXml( ImagePath );
            }
        }
    }
}
