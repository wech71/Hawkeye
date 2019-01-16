using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Collections.Generic;

using ACorns.Hawkeye.Utils;
using ACorns.Hawkeye.Utils.Menus;
using ACorns.Hawkeye.Core.Utils.Accessors;
using ACorns.Hawkeye.Options;
using ACorns.Hawkeye.Tabs.Fields;
using ACorns.Hawkeye.Tabs.Events;
using ACorns.Hawkeye.Tabs.Methods;
using ACorns.Hawkeye.Tabs.Properties;
using ACorns.Hawkeye.Tabs.ProcessInfo;
using ACorns.Hawkeye.Tabs.Toolbar;
using ACorns.Hawkeye.Tools.Reflector;



namespace ACorns.Hawkeye.Tabs
{
    internal partial class XPropertyGrid : PropertyGrid
    {
        #region Private memnbers

        private const int maxHistoryDepth = 10; // remember the 10 last selections.

        private int activeObjectCount = -1;
        private List<object> historyObjects = new List<object>();
        private FieldAccesor gridViewAccessor;
        private FieldAccesor peMainAccessor;
        private MethodAccesor recursivelyExpandAccessor;

        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem selectThisItem;
        private ToolStripMenuItem showSourceCodeForItem;
        private ToolStripMenuItem goBackOneItem;
        private ToolStripMenuItem goForwardOneItem;
        private ToolStripMenuItem goParentItem;

        private ToolBar externalToolBar;
        private ToolBar gridToolBar;

        private ToolBarButton btnRefresh;
        private ToolBarButton btnBack;
        private ToolBarButton btnForward;
        private ToolBarButton btnParent;
        private ToolBarButton btnCollapseAll;
        private ToolBarButton btnExpandAll;
        private ToolBarButton btnShowSourceCode;
        private ToolBarButton btnAddExtender;
        private ToolBarButton btnHighlightWindow;

        private WindowProperties highlightWindowProperties = new WindowProperties();

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="XPropertyGrid"/> class.
        /// </summary>
        public XPropertyGrid()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XPropertyGrid"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public XPropertyGrid(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        #endregion

        #region Events definition

        public event SelectedObjectRequestHandler SelectRequest;
        public event EventHandler AddToolbarButtons;

        #endregion

        #region Properties

        public ToolBar ExternalToolBar
        {
            get { return externalToolBar; }
            set { externalToolBar = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the commands pane can be made visible for the currently selected objects.
        /// </summary>
        /// <value></value>
        /// <returns>true if the commands pane can be made visible; otherwise, false.
        /// </returns>
        public override bool CanShowCommands
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the commands pane is visible for objects that expose verbs.
        /// </summary>
        /// <value></value>
        /// <returns>true if the commands pane is visible; otherwise, false. The default is true.
        /// </returns>
        public override bool CommandsVisibleIfAvailable
        {
            get { return true; }
            set { base.CommandsVisibleIfAvailable = value; }
        }

        #endregion

        #region Public API

        public ToolBarButton AddToolbarButton(int index, string text, string imageName, EventHandler eventHandler)
        {
            return ToolbarUtils.AddButton(index, gridToolBar, text, imageName, eventHandler);
        }

        #endregion

        #region Initializations & UI refresh

        private void InitContextMenu()
        {
            // TODO: switch to a regular ToolStrip, then use png/32bits images so that 
            // transparency has not to be fixed this way.
            Bitmap leftImage = new Bitmap(ACorns.Hawkeye.Properties.Resources.LeftArrow);
            leftImage.MakeTransparent(Color.Silver);
            Bitmap rightImage = new Bitmap(ACorns.Hawkeye.Properties.Resources.RightArrow);
            rightImage.MakeTransparent(Color.Silver);
            Bitmap upImage = new Bitmap(ACorns.Hawkeye.Properties.Resources.UpArrow);
            upImage.MakeTransparent(Color.Silver);
            Bitmap reflectorImage = new Bitmap(ACorns.Hawkeye.Properties.Resources.Reflector);
            reflectorImage.MakeTransparent(Color.Silver);

            goBackOneItem = new ToolStripMenuItem("Back", leftImage);
            goForwardOneItem = new ToolStripMenuItem("Forward", rightImage);
            goParentItem = new ToolStripMenuItem("Parent", upImage);
            selectThisItem = new ToolStripMenuItem("Select");
            showSourceCodeForItem = new ToolStripMenuItem("Show source code", reflectorImage);

            selectThisItem.Click += new EventHandler(selectThisItem_Click);
            showSourceCodeForItem.Click += new EventHandler(showSourceCodeForItem_Click);
            goBackOneItem.Click += new EventHandler(goBackOneItem_Click);
            goForwardOneItem.Click += new EventHandler(goForwardOneItem_Click);
            goParentItem.Click += new EventHandler(goParentItem_Click);

            contextMenu = new ContextMenuStrip();
            contextMenu.Items.AddRange(new ToolStripItem[]
            {
                selectThisItem,
                showSourceCodeForItem,
                new ToolStripSeparator(),
                goBackOneItem, 
                goForwardOneItem,
                goParentItem
            });

            ContextMenuStrip = contextMenu;
        }

        private void InitCustomContextMenus()
        {
            foreach (PropertyTab tab in this.PropertyTabs)
            {
                IPropertyGridTab handler = tab as IPropertyGridTab;
                if (handler != null)
                {
                    handler.PropertyGrid = this;
                }
            }

            foreach (PropertyTab tab in this.PropertyTabs)
            {
                ICustomMenuHandler handler = tab as ICustomMenuHandler;
                if (handler != null)
                {
                    handler.RegisterMenuItems(this.contextMenu);
                }
            }
            RefreshValidContextMenus();
        }

        private void RefreshValidContextMenus()
        {
            foreach (ToolStripItem menuItem in this.contextMenu.Items)
            {
                CustomMenuItem cMenuItem = menuItem as CustomMenuItem;
                if (cMenuItem != null)
                {
                    if (cMenuItem.OwnerTab != this.SelectedTab)
                    {
                        cMenuItem.Visible = false;
                    }
                    else
                    {
                        cMenuItem.Visible = true;
                    }
                }
            }
        }

        private void InitCustomToolbar()
        {
            gridToolBar = externalToolBar;
            if (externalToolBar == null)
                return;
            externalToolBar.ButtonClick += new ToolBarButtonClickEventHandler(externalToolBar_ButtonClick);
            RegisterToolbarButtons(false);

            // Grab the toolbar inside the Property Grid
            //			FieldAccesor toolBarAccesor = new FieldAccesor(typeof(PropertyGrid), this, "toolBar");

            /*			if ( toolBarAccesor.IsValid )
                        {
                            // we might be running on a box with .Net 2.0 and NO .Net 1.1
                            toolBarAccesor.Save();
                            gridToolBar = toolBarAccesor.Value as ToolBar;

                            RegisterToolbarButtons(true);
                        }
                        else
                        {
                            Trace.WriteLine("We might be using .Net 2.0 and we have to use a ToolStrip!");

                            gridToolBar = externalToolBar;
                            externalToolBar.Visible = true;
                            externalToolBar.ButtonClick += new ToolBarButtonClickEventHandler(externalToolBar_ButtonClick);

                            RegisterToolbarButtons(false);


                            FieldAccesor toolStripAccessor = new FieldAccesor(typeof(PropertyGrid), this, "toolStrip");
                            if ( toolStripAccessor.IsValid )
                            {
                                toolStripAccessor.Save();
                                object toolStrip = toolStripAccessor.Value;
					
                                PropertyAccesor toolStripItemsAccessor = new PropertyAccesor(toolStrip, "Items");
                                toolStripItemsAccessor.Save();

                                object items = toolStripItemsAccessor.Value;
                                ObjectHandle instance = Activator.CreateInstance("System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "System.Windows.Forms.ToolStripButton");
                                object newButton = instance.Unwrap();

                                PropertyAccesor countAcc = new PropertyAccesor(items, "Count").Save();
                                int count = (int)countAcc.Value;

                                MethodAccesor addItem = new MethodAccesor(items.GetType(), "Insert");
                                addItem.Invoke(items, new object[]{ count, newButton } );
                            }
                        }*/
        }

        private void RegisterToolbarButtons(bool modifyExisting)
        {
            if (modifyExisting)
            {
                ToolbarUtils.AddDelimiter(gridToolBar);
                ToolbarUtils.DelButton(gridToolBar, "Property Pages");
            }

            btnRefresh = ToolbarUtils.AddButton(gridToolBar, "Refresh", "Refresh.bmp", new EventHandler(btnRefresh_Click));

            btnBack = ToolbarUtils.AddButton(gridToolBar, "Back", "LeftArrow.bmp", new EventHandler(btnBack_Click));
            btnForward = ToolbarUtils.AddButton(gridToolBar, "Forward", "RightArrow.bmp", new EventHandler(btnForward_Click));
            btnParent = ToolbarUtils.AddButton(gridToolBar, "Parent", "UpArrow.bmp", new EventHandler(btnParent_Click));

            btnHighlightWindow = ToolbarUtils.AddButton(gridToolBar, "Highlight Window", "Highlight.bmp", new EventHandler(highlightWindow_Click));
            btnExpandAll = ToolbarUtils.AddButton(gridToolBar, "Collapse and Group by Category", "CollapseAll.bmp", new EventHandler(btnCollapseAll_Click));
            btnCollapseAll = ToolbarUtils.AddButton(gridToolBar, "Expand All", "ExpandAll.bmp", new EventHandler(btnExpandAll_Click));
            btnShowSourceCode = ToolbarUtils.AddButton(gridToolBar, "Show SourceCode In Reflector", "Reflector.bmp", new EventHandler(showSourceCodeForItem_Click));
            btnAddExtender = ToolbarUtils.AddButton(gridToolBar, "Add Dynamic Extender", "AddDynamicExtender.bmp", new EventHandler(addDynamicExtender_click));

            btnAddExtender.Enabled = false;
            btnBack.Enabled = false;
            btnForward.Enabled = false;
            btnParent.Enabled = false;

            if (AddToolbarButtons != null) AddToolbarButtons(this, EventArgs.Empty);

            RefreshToolbarButtonsAndMenusState();
        }

        private void RefreshToolbarButtonsAndMenusState()
        {
            object selected = SelectedObject;
            bool enabled = selected != null;

            btnRefresh.Enabled = enabled;
            btnExpandAll.Enabled = enabled;
            btnCollapseAll.Enabled = enabled;
            showSourceCodeForItem.Enabled = btnShowSourceCode.Enabled = enabled;
            selectThisItem.Enabled = SelectedGridItem != null;
            goParentItem.Enabled = btnParent.Enabled = GetParent(SelectedObject) != null;

            btnAddExtender.Enabled = selected != null &&
                ApplicationOptions.Instance.HasDynamicExtenders(selected.GetType());
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Called when the control is created.
        /// </summary>
        protected override void OnCreateControl()
        {
            DrawFlatToolbar = true;
            HelpVisible = true;

            PropertySort = PropertySort.Alphabetical;

            InitContextMenu();

            goBackOneItem.Enabled = false;
            goForwardOneItem.Enabled = false;
            if (btnBack != null) btnBack.Enabled = false;
            if (btnForward != null) btnForward.Enabled = false;

            base.OnCreateControl();

            // Add New Tabs here
            base.PropertyTabs.AddTabType(typeof(AllPropertiesTab));
            base.PropertyTabs.AddTabType(typeof(AllFieldsTab));
            base.PropertyTabs.AddTabType(typeof(InstanceEventsTab));
            base.PropertyTabs.AddTabType(typeof(MethodsTab));
            base.PropertyTabs.AddTabType(typeof(ProcessInfoTab));

            historyObjects.Clear();

            InitCustomToolbar();

            InitCustomContextMenus();

            Type type = typeof(PropertyGrid);
            if (ApplicationOptions.Instance.HasDynamicExtenders(type))
            {
                DynamicExtenderInfo extender = ApplicationOptions.Instance.GetDynamicExtender(type);
                if (extender != null) extender.CreateExtender(this);
                // TODO: log if extender == null
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.PropertyGrid.SelectedObjectsChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnSelectedObjectsChanged(EventArgs e)
        {
            // put in history
            if (SelectedObject != null)
            {
                if (!historyObjects.Contains(SelectedObject))
                {
                    if (activeObjectCount < historyObjects.Count - 1)
                        historyObjects.RemoveRange(activeObjectCount + 1, historyObjects.Count - activeObjectCount - 1);

                    historyObjects.Add(SelectedObject);
                    activeObjectCount = historyObjects.Count;
                    goBackOneItem.Enabled = btnBack.Enabled = true;
                    goForwardOneItem.Enabled = btnForward.Enabled = false;

                    int historyCount = historyObjects.Count;
                    if (historyCount > maxHistoryDepth)
                        historyObjects.RemoveRange(0, historyCount - maxHistoryDepth);
                }
                else activeObjectCount = historyObjects.IndexOf(SelectedObject);
            }

            base.OnSelectedObjectsChanged(e);
            RefreshToolbarButtonsAndMenusState();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.PropertyGrid.PropertyTabChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PropertyTabChangedEventArgs"/> that contains the event data.</param>
        protected override void OnPropertyTabChanged(PropertyTabChangedEventArgs e)
        {
            base.OnPropertyTabChanged(e);
            RefreshValidContextMenus();
        }

        #endregion

        #region Actions

        private void SelectThisItem()
        {
            GridItem selectedGridItem = this.SelectedGridItem;
            if (selectedGridItem == null) return;

            SelectItem(selectedGridItem.Value);
        }

        private void GoBackOneItem()
        {
            if (activeObjectCount > 0)
            {
                activeObjectCount--;
                goForwardOneItem.Enabled = btnForward.Enabled = true;
            }
            else goBackOneItem.Enabled = btnBack.Enabled = false;

            InvokeSelectRequest(GetActiveObject());
        }

        private void GoForwardOneItem()
        {
            if (activeObjectCount < historyObjects.Count)
            {
                activeObjectCount++;
                goBackOneItem.Enabled = btnBack.Enabled = true;
            }
            else goForwardOneItem.Enabled = btnForward.Enabled = false;

            InvokeSelectRequest(GetActiveObject());
        }

        private void SelectParentItem()
        {
            SelectItem(GetParent(SelectedObject));
        }

        private void ExpandAll()
        {
            if (SelectedObject == null) return;

            Visible = false;
            try
            {
                if (gridViewAccessor == null)
                {
                    gridViewAccessor = new FieldAccesor(this, "gridView");
                    peMainAccessor = new FieldAccesor(this, "peMain");
                }

                object gridView = gridViewAccessor.Get();
                object peMain = peMainAccessor.Get();
                if (recursivelyExpandAccessor == null)
                    recursivelyExpandAccessor = new MethodAccesor(gridView.GetType(), "RecursivelyExpand");

                recursivelyExpandAccessor.Invoke(gridView, new object[] { peMain, false, true, 2 });
            }
            finally { Visible = true; }
        }

        private void CollapseAll()
        {
            if (SelectedObject == null) return;

            Visible = false;
            try
            {
                PropertySort = PropertySort.CategorizedAlphabetical;
                CollapseAllGridItems();
            }
            finally { Visible = true; }
        }

        private void ShowSourceCode()
        {
            GridItem selectedGridItem = SelectedGridItem;
            if (selectedGridItem != null) ReflectorRouter.Instance.ShowSourceCode(
                selectedGridItem.PropertyDescriptor);
        }

        private void HighlightWindow()
        {
            if (!(SelectedObject is Control)) return;

            IntPtr handle = ((Control)SelectedObject).Handle;
            if (handle != IntPtr.Zero)
                highlightWindowProperties.SetWindowHandle(handle, Point.Empty);
        }

        private void HandleExternalToolBarButtonClick(ToolBarButton button)
        {
            if (button == null) return;
            EventHandler handler = button.Tag as EventHandler;
            if (handler != null) handler(externalToolBar, EventArgs.Empty);
        }

        //TODO: is this useful???
        private void AddDynamicExtender()
        {
            if (SelectedObject == null) return;

            Type selectedObjectType = SelectedObject.GetType();
            if (!ApplicationOptions.Instance.HasDynamicExtenders(selectedObjectType)) return;

            DynamicExtenderInfo extender = ApplicationOptions.Instance.GetDynamicExtender(selectedObjectType);
            if (extender != null) extender.CreateExtender(SelectedObject);
            // TODO: log if extender == null
        }

        #endregion

        #region Private implementation

        private object GetParent(object child)
        {
            if (child == null) return null;
            if (child is Control)
                return ((Control)child).Parent;
            else return null;
        }

        private void SelectItem(object value)
        {
            if (value == null) return;

            if (value is IRealValueHolder) // recursively open this.
                SelectItem(((IRealValueHolder)value).RealValue);
            else InvokeSelectRequest(value);
        }

        private object GetActiveObject()
        {
            if (activeObjectCount >= 0 && activeObjectCount < historyObjects.Count)
                return historyObjects[activeObjectCount];
            else return null;
        }

        private void InvokeSelectRequest(object newObject)
        {
            if (newObject != null && SelectRequest != null) 
                SelectRequest(newObject);
        }

        #endregion

        #region Event handlers

        private void selectThisItem_Click(object sender, EventArgs e) { SelectThisItem(); }

        private void goBackOneItem_Click(object sender, EventArgs e) { GoBackOneItem(); }

        private void goForwardOneItem_Click(object sender, EventArgs e) { GoForwardOneItem(); }

        private void goParentItem_Click(object sender, EventArgs e) { SelectParentItem(); }

        private void btnRefresh_Click(object sender, EventArgs e) { Refresh(); }

        private void btnBack_Click(object sender, EventArgs e) { GoBackOneItem(); }

        private void btnForward_Click(object sender, EventArgs e) { GoForwardOneItem(); }

        private void btnParent_Click(object sender, EventArgs e) { SelectParentItem(); }

        private void btnCollapseAll_Click(object sender, EventArgs e) { CollapseAll(); }

        private void btnExpandAll_Click(object sender, EventArgs e) { ExpandAll(); }

        private void showSourceCodeForItem_Click(object sender, EventArgs e) { ShowSourceCode(); }

        private void highlightWindow_Click(object sender, EventArgs e) { HighlightWindow(); }

        private void externalToolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e) { HandleExternalToolBarButtonClick(e.Button); }

        private void addDynamicExtender_click(object sender, EventArgs e) { AddDynamicExtender(); }

        #endregion
    }
}
