using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace DesignerLibrary.TypeEditors
{
    public class ImageFileTypeEditor : ModalUITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext pContext, IServiceProvider pProvider, object pValue)
        {
            IWindowsFormsEditorService lEditorService = null;
            object lRet = null;

            if (pProvider != null)
            {
                lEditorService = pProvider.GetService( typeof( IWindowsFormsEditorService ) ) as IWindowsFormsEditorService;

                if (lEditorService != null)
                {
                    OpenFileDialog lForm = new OpenFileDialog();

                    lForm.Filter = "Image files (*.jpg; *.png; *.bmp; *.gif)|*.jpg;*.png;*.bmp;*.gif|All files (*.*)|*.*";
                    if (lForm.ShowDialog() == DialogResult.OK)
                    {
                        lRet = lForm.FileName;
                    }
                }
            }

            return lRet;
        }
    }
}
