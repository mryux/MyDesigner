using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Collections;
using DesignerLibrary.Consts;

namespace DesignerLibrary
{
    public class CustomToolboxService : ToolboxService
    {
        ToolboxControl mToolboxControl = null;

        #region Properties
        /// <summary>
        /// Gets and sets the Toolbox control which provides the list of
        /// ToolboxItems for this ToolboxService.
        /// </summary>
        public ToolboxControl ToolboxControl
        {
            get { return mToolboxControl; }
            set { mToolboxControl = value; }
        }

        /// <summary>
        /// Gets a collection of strings depicting available categories of the toolbox.
        /// </summary>
        protected override CategoryNameCollection CategoryNames
        {
            get
            {
                // Get a list of category names out of the ListView groups collection
                string[] lGroupNames = new string[mToolboxControl.ToolboxList.Groups.Count];
                ListViewGroup[] lGroups = new ListViewGroup[mToolboxControl.ToolboxList.Groups.Count];
                mToolboxControl.ToolboxList.Groups.CopyTo( lGroups, 0 );
                lGroups.ToList().ConvertAll<string>( lGroup => lGroup.Name ).CopyTo( lGroupNames );

                return new CategoryNameCollection( lGroupNames );
            }
        }

        /// <summary>
        /// Gets the name of the currently selected category.
        /// Set throws a NotImplementedException.
        /// </summary>
        protected override string SelectedCategory
        {
            get
            {
                return mToolboxControl.ToolboxList.SelectedItems[0].Group.Name;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets or sets the currently selected item container.
        /// Used by the design surface when creating a new control on the design surface.
        /// If there is no currently selected item or it is a Pointer item, then
        /// the get method returns null, and the designer allows selection rather
        /// than object creation.
        /// </summary>
        protected override ToolboxItemContainer SelectedItemContainer
        {
            get
            {
                ToolboxItemContainer lRet = null;

                if (mToolboxControl != null
                    && mToolboxControl.ToolboxList != null
                    && mToolboxControl.ToolboxList.SelectedItems.Count > 0)
                {
                    ToolboxItem lSelectedItem = mToolboxControl.ToolboxList.SelectedItems[0].Tag as ToolboxItem;

                    if (lSelectedItem.TypeName != NameConsts.Pointer)
                    {
                        lRet = new ToolboxItemContainer( lSelectedItem );
                    }
                }

                return lRet;
            }

            set
            {
                if (value == null
                    && mToolboxControl != null
                    && mToolboxControl.ToolboxList != null)
                {
                    foreach (ListViewItem lItem in mToolboxControl.ToolboxList.SelectedItems)
                    {
                        lItem.Selected = false;
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Returns an IList containing all items in the toolbox for the given
        /// category.
        /// This method is not yet implemented.
        /// </summary>
        /// <param name="categoryName">The category for which ToolboxItemContainers
        /// are returned.</param>
        /// <returns>A list of ToolboxItemContainers for the items in the toolbox.</returns>
        protected override System.Collections.IList GetItemContainers(string categoryName)
        {
            IList<ToolboxItem> lList = mToolboxControl.ToolboxList.Groups[categoryName].Items as IList<ToolboxItem>;
            return lList.Select<ToolboxItem, ToolboxItemContainer>( lItem => new ToolboxItemContainer( lItem ) ) as IList;
        }

        /// <summary>
        /// Returns an IList containing all items in the toolbox.
        /// This method is not yet implemented.
        /// </summary>
        /// <returns>A list of ToolboxItemContainers for the items in the toolbox.</returns>
        protected override System.Collections.IList GetItemContainers()
        {
            IList<ToolboxItem> lList = mToolboxControl.ToolboxList.Items as IList<ToolboxItem>;
            return lList.Select<ToolboxItem, ToolboxItemContainer>( lItem => new ToolboxItemContainer( lItem ) ) as IList;
        }

        /// <summary>
        /// Not implemented, calling this method will throw a NotImplementedException.
        /// </summary>
        protected override void Refresh()
        {
            throw new NotImplementedException();
        }
    }
}
