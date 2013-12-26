using DesignerLibrary.DrawingTools;
using DesignerLibrary.Views;

namespace DesignerLibrary.Views
{
    class RuntimeView : BaseView
    {
        public RuntimeView()
        {

        }

        protected override void OnAddTool(DrawingTool pTool)
        {
            //(pTool as IComponent).Site = Site;
        }
    }
}
