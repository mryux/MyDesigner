using System.ComponentModel;
using System.Drawing.Design;

namespace DesignerLibrary.TypeEditors
{
    public class ModalUITypeEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext pContext)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
