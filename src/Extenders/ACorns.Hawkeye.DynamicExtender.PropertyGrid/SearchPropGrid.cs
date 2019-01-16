using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using ACorns.Hawkeye.Core.Utils.Accessors;
using ACorns.Hawkeye.Public;

namespace ACorns.PropertyGridExtender
{
    /// <summary>
    /// Dynamic Subclass extender for any Property Grid.
    /// This extender will add a small search box to any PropertyGrid to which it is attached.
    /// </summary>
    public class SearchPropGrid : IDynamicSubclass
    {
        private PropertyGrid propertyGrid;

        private Control toolBar;
        private TextBox textBox;

        private object gridView;

        private FieldAccesor allGridItemsAC;
        private FieldAccesor topLevelGridItemsAC;

        private FieldAccesor gridViewEntries;

        private FieldAccesor totalProps;
        private FieldAccesor selectedRow;

        private MethodAccesor setScrollOffset;

        private PropertyAccesor selectGridEntry;

        private MethodAccesor layoutWindow;

        private GridItem[] allGridItems;

        private Color originalBackColor;

        public SearchPropGrid()
        {

        }

        public void Attach(object target)
        {
            Type targetType = target.GetType();
            if (targetType.FullName == "System.Windows.Forms.PropertyGridInternal.PropertyGridView")
            {
                FieldAccesor ownerGrid = new FieldAccesor(target, "ownerGrid");
                this.propertyGrid = ownerGrid.Get() as PropertyGrid;
            }
            else
            {
                this.propertyGrid = target as PropertyGrid;
            }

            if (propertyGrid.IsHandleCreated)
            {
                GrabObjects();
            }
            else
            {
                propertyGrid.HandleCreated += new EventHandler(propertyGrid_HandleCreated);
                propertyGrid.VisibleChanged += new EventHandler(propertyGrid_VisibleChanged);
            }
        }
        private void propertyGrid_HandleCreated(object sender, EventArgs e)
        {
            GrabObjects();
        }
        private void propertyGrid_VisibleChanged(object sender, EventArgs e)
        {
            GrabObjects();
        }
        private void GrabObjects()
        {
            if (toolBar != null)
                return;

            FieldAccesor toolBarAccesor = new FieldAccesor(propertyGrid, "toolBar");
            if (toolBarAccesor.IsValid)
            {
                toolBarAccesor.Get();
            }
            else
            {
                toolBarAccesor = new FieldAccesor(propertyGrid, "toolStrip");
                toolBarAccesor.Get();

                // for a toolstrip we have to "fix" the readonly stuff on the controls :(
                Control strip = toolBarAccesor.Value as Control;
                FieldAccesor readonlyFlag = new FieldAccesor(strip.Controls, "_isReadOnly");
                readonlyFlag.Set(false);
            }

            toolBar = toolBarAccesor.Value as Control;
            AttachNewToolButtons();

            FieldAccesor gridViewAccesor = new FieldAccesor(propertyGrid, "gridView");
            gridViewAccesor.Get();
            this.gridView = gridViewAccesor.Value;

            allGridItemsAC = new FieldAccesor(gridView, "allGridEntries");
            topLevelGridItemsAC = new FieldAccesor(gridView, "topLevelGridEntries");

            totalProps = new FieldAccesor(gridView, "totalProps");
            selectedRow = new FieldAccesor(gridView, "selectedRow");

            setScrollOffset = new MethodAccesor(gridView.GetType(), "SetScrollOffset");
            selectGridEntry = new PropertyAccesor(gridView, "SelectedGridEntry");
            layoutWindow = new MethodAccesor(gridView.GetType(), "Refresh");

            propertyGrid.SelectedObjectsChanged += new EventHandler(propertyGrid_SelectedObjectsChanged);
            propertyGrid.PropertyTabChanged += new PropertyTabChangedEventHandler(propertyGrid_PropertyTabChanged);
            propertyGrid.PropertySortChanged += new EventHandler(propertyGrid_PropertySortChanged);
        }

        private void AttachNewToolButtons()
        {
            textBox = new TextBox();
            textBox.Location = new Point(0, 0);
            textBox.Size = new Size(70, textBox.Height);
            textBox.BorderStyle = BorderStyle.Fixed3D;
            textBox.Font = new Font("Tahoma", 8.25f);

            originalBackColor = textBox.BackColor;

            textBox.TextChanged += new EventHandler(textBox_TextChanged);
            textBox.VisibleChanged += new EventHandler(textBox_VisibleChanged);

            toolBar.Controls.Add(textBox);
            toolBar.SizeChanged += new EventHandler(toolBar_SizeChanged);

            FixSearchBoxLocation();
        }

        private void toolBar_SizeChanged(object sender, EventArgs e)
        {
            FixSearchBoxLocation();
        }

        private void FixSearchBoxLocation()
        {
            textBox.Location = new Point(toolBar.Width - textBox.Width - 2, (toolBar.Height - textBox.Height) / 2);
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            string search = textBox.Text.ToLower();

            if (search.Length == 0)
            {
                textBox.BackColor = originalBackColor;
            }
            else
            {
                if (search.Length >= 1 && search.StartsWith("?"))
                {
                    textBox.BackColor = Color.LightBlue;
                }
                else
                {
                    textBox.BackColor = Color.Coral;
                }
            }

            // dynamic filter this grid			
            if (propertyGrid.SelectedObject != null)
            {
                GridItem[] items = GridViewItems;
                if (items == null)
                    return;

                if (search.Length == 0 && allGridItems != null)
                {
                    //GridViewItems = allGridItems;
                    allGridItems = null;

                    propertyGrid.Refresh();
                }
                else
                {
                    if (allGridItems == null)
                        allGridItems = items;

                    bool checkContains = false;
                    if (search.StartsWith("?"))
                    {
                        search = search.Substring(1);
                        checkContains = true;
                    }

                    ArrayList newList = new ArrayList();
                    foreach (GridItem item in allGridItems)
                    {
                        if (item.Label != null && item.GridItemType == GridItemType.Property)
                        {
                            if (checkContains)
                            {
                                if (item.Label.ToLower().IndexOf(search) != -1)
                                {
                                    newList.Add(item);
                                }
                            }
                            else
                            {
                                if (item.Label.ToLower().StartsWith(search))
                                {
                                    newList.Add(item);
                                }
                            }
                        }
                    }

                    GridItem[] newItems = new GridItem[newList.Count];
                    newList.CopyTo(newItems);

                    GridViewItems = newItems;
                }
            }
        }

        private GridItem[] GridViewItems
        {
            get
            {
                object items = allGridItemsAC.Get();
                if (items == null)
                    return null;

                if (gridViewEntries == null)
                {
                    gridViewEntries = new FieldAccesor(items, "entries");
                }

                gridViewEntries.Target = allGridItemsAC.Get();
                GridItem[] entries = gridViewEntries.Get() as GridItem[];

                return entries;
            }
            set
            {
                object items = allGridItemsAC.Get();
                if (items == null)
                    return;

                bool wasFocused = textBox.Focused;

                if (gridViewEntries == null)
                {
                    gridViewEntries = new FieldAccesor(items, "entries");
                }

                setScrollOffset.Invoke(gridView, 0);

                gridViewEntries.Target = allGridItemsAC.Get();
                gridViewEntries.Set(value);

                gridViewEntries.Target = topLevelGridItemsAC.Get();
                gridViewEntries.Set(value);

                totalProps.Set(value.Length);
                selectedRow.Set(0);

                if (value.Length > 0)
                {
                    selectGridEntry.Restore(value[0]);
                }

                (gridView as Control).Invalidate();

                if (wasFocused)
                {
                    textBox.Focus();
                }
            }
        }

        private void propertyGrid_SelectedObjectsChanged(object sender, EventArgs e)
        {
            textBox.Text = String.Empty;
        }
        private void propertyGrid_PropertyTabChanged(object s, PropertyTabChangedEventArgs e)
        {
            textBox.Text = String.Empty;
        }
        private void propertyGrid_PropertySortChanged(object sender, EventArgs e)
        {
            textBox.Text = String.Empty;
        }

        private void textBox_VisibleChanged(object sender, EventArgs e)
        {
            if (textBox.Visible)
            {
                textBox.VisibleChanged -= new EventHandler(textBox_VisibleChanged);
            }
        }
    }
}
