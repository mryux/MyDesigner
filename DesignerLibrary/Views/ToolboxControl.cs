using DesignerLibrary.Consts;
using DesignerLibrary.DrawingTools;
using DesignerLibrary.Helpers;
using System;
using System.Linq;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Drawing;

namespace DesignerLibrary.Views
{
    public partial class ToolboxControl : UserControl
    {
        public ToolboxControl()
        {
            InitializeComponent();

            // Initialise the ToolboxService
            mToolboxService = new CustomToolboxService();
            mToolboxService.ToolboxControl = this;

            mToolboxList.ItemDrag += OnItemDrag;
        }

        CustomToolboxService mToolboxService = null;
        bool mIsDragging = false;

        #region Properties
        /// <summary>
        /// Gets and sets the list of ToolboxItems (via the ListView) contained
        /// in this control. Typically used by the ToolboxService to get the
        /// selected ToolboxItem for the Design Surface. (I.e. when adding a
        /// control to the surface)
        /// </summary>
        [Browsable( false )]
        public ListView ToolboxList
        {
            get { return mToolboxList; }
            set { mToolboxList = value; }
        }

        /// <summary>
        /// Gets the ToolboxService attached to this control
        /// </summary>
        public CustomToolboxService ToolboxService
        {
            get { return mToolboxService; }
        }

        /// <summary>
        /// Gets whether a drag/drop operation has been started on this control.
        /// </summary>
        public bool IsDragging
        {
            get { return mIsDragging; }
        }
        #endregion

        /// <summary>
        /// Adds the given ToolboxItem to the control's list of ToolboxItems.
        /// The ToolboxItem must have a valid Bitmap before it can be added
        /// </summary>
        /// <param name="lItem">The ToolboxItem to add.</param>
        /// <exception cref="ArgumentNullException">
        ///     <para><paramref name="lItem"/>ToolboxItem.Bitmap is null</para>
        /// </exception>
        public void AddToolboxItem(ToolboxItem lItem)
        {
            AddToolboxItem( lItem, "" );
        }

        /// <summary>
        /// Adds the given ToolboxItem to the control's list of ToolboxItems, with the 
        /// specified category.
        /// </summary>
        /// <param name="lItem">The ToolboxItem to add.</param>
        /// <param name="lCategory">The Toolbox Category in which to add the 
        /// ToolboxItem 
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para><paramref name="lItem"/>ToolboxItem.Bitmap is null</para>
        /// </exception>
        public void AddToolboxItem(ToolboxItem pItem, string pCategory)
        {
            ListViewItem lToolboxItem = new ListViewItem( pItem.DisplayName );
            ToolboxItem lItem = GetToolboxItem( pItem.TypeName + "," + pItem.AssemblyName );
            if (lItem == null)
            {
                lItem = pItem;
            }
            lToolboxItem.Tag = lItem;
            mToolboxImageList.Images.Add( pItem.TypeName, pItem.Bitmap );
            lToolboxItem.ImageKey = pItem.TypeName;

            if (string.IsNullOrEmpty(pCategory))
            {
                mToolboxList.Items.Add( lToolboxItem );
            }
            else
            {
                ListViewGroup lCategoryGroup = new ListViewGroup( pCategory, pCategory );

                if (!mToolboxList.Groups.Contains( lCategoryGroup ))
                    mToolboxList.Groups.Add( lCategoryGroup );

                mToolboxList.Groups[pCategory].Items.Add( lToolboxItem );
            }
        }

        private ToolboxItem GetToolboxItem(string pTypeName)
        {
            Type lToolboxItemType = Type.GetType( pTypeName );
            ToolboxItem lItem = null;

            if (lToolboxItemType != null)
                lItem = System.Drawing.Design.ToolboxService.GetToolboxItem( lToolboxItemType );

            return lItem;
        }

        /// <summary>
        /// Removes all items from the ListView.
        /// </summary>
        public void Clear()
        {
            mToolboxImageList.Images.Clear();
            mToolboxList.Clear();
        }

        /// <summary>
        /// Handles the Drag event which raises a DoDragDrop event used by the
        /// DesignSurface to drag items from the Toolbox to the DesignSurface.
        /// If the currently selected item is the pointer item then the drag drop 
        /// operation is not started.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button == MouseButtons.Left
                && e.Item.GetType() == typeof( ListViewItem ))
            {
                ToolboxItem lItem = (e.Item as ListViewItem).Tag as ToolboxItem;

                if (lItem.TypeName != NameConsts.Pointer)
                {
                    mIsDragging = true;

                    IToolboxService lToolboxService = mToolboxService as IToolboxService;
                    object lDataObject = lToolboxService.SerializeToolboxItem( lItem );
                    DrawingTool lTool = lItem.CreateComponents().FirstOrDefault() as DrawingTool;

                    lTool.CreatePersistence();
                    Rectangle lRect = lTool.SurroundingRect;

                    using (DragImage image = new DragImage(lTool.DefaultImage, lRect.Width/2, lRect.Height/2))
                    {
                        DoDragDrop(lDataObject, DragDropEffects.All);
                    }

                    mIsDragging = false;
                }
            }
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs pArgs)
        {
            base.OnGiveFeedback(pArgs);

            pArgs.UseDefaultCursors = false;
            Cursor.Current = Cursors.SizeAll;
        }
    }
}
