using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using ACorns.Hawkeye.Core.Options;
using ACorns.Hawkeye.Core.UI;
using ACorns.Hawkeye.Core.Utils;
using ACorns.Hawkeye.Options;
using ACorns.Hawkeye.Plugins;
using ACorns.Hawkeye.Tools;
using ACorns.Hawkeye.Utils;
using ACorns.Hawkeye.Utils.UI;
using ACorns.Hawkeye.Public;

namespace ACorns.Hawkeye
{
    internal partial class HawkeyeEditor : Form, IHawkeyeEditor
    {
        private const string FINDER_TOOLTIP =
            "To start editing .Net objects Drag and Drop the target on ANY .Net Control in ANY .Net process.";

        /// <summary>
        /// Initializes a new instance of the <see cref="HawkeyeEditor"/> class.
        /// </summary>
        public HawkeyeEditor()
        {
            InitializeComponent();

            Text = HawkeyeAppUtils.FullApplicationName;
            toolTip.SetToolTip(windowFinder, FINDER_TOOLTIP);
            base.Icon = SystemUtils.LoadIcon("SmallEye.ico");

            PluginManager.Instance.Initialize(toolBar);	// init :)
        }

        /// <summary>
        /// Gets or sets the selected object.
        /// </summary>
        /// <value>The selected object.</value>
        public object SelectedObject
        {
            get { return propertyGrid.SelectedObject; }
            set { propertyGrid.SelectedObject = value; }
        }

        /// <summary>
        /// Gets or sets the selected window handle.
        /// </summary>
        /// <value>The selected window handle.</value>
        public IntPtr SelectedWindowHandle
        {
            get { return windowFinder.SelectedHandle; }
            set { windowFinder.SelectedHandle = value; }
        }

        /// <summary>
        /// Raises the CreateControl event.
        /// </summary>
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            
            bool extenderCreated = false;
            extenderCreated = TryCreateExtender(GetType());
            if (!extenderCreated)
                extenderCreated = TryCreateExtender(typeof(IHawkeyeEditor));
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!WindowInfo.Load(this))
            {
                int y = SystemInformation.WorkingArea.Bottom - base.Height;
                base.Location = new Point(0, y);
            }

            ShowBalloonHelp();
        }

        private bool TryCreateExtender(Type type)
        {
            if (ApplicationOptions.Instance.HasDynamicExtenders(type))
            {
                DynamicExtenderInfo extender = ApplicationOptions.Instance.GetDynamicExtender(type);
                if (extender != null)
                {
                    extender.CreateExtender(this);
                    return true;
                }
            }

            return false;
        }

        private bool ChangeSelectedObject(object selectedObject)
        {
            if (!CoreApplicationOptions.Instance.AllowSelectOwnedObjects)
            {
                if (selectedObject != null
                    && selectedObject.GetType().Assembly == typeof(HawkeyeEditor).Assembly)
                {
                    if (!(selectedObject is IAllowSelect)) return true;
                }
            }

            try
            {
                propertyGrid.SelectedObject = selectedObject;
                PluginManager.Instance.OnSelectedObjectChanged(selectedObject);

                if (selectedObject != null)
                {
                    string controlName = HawkeyeUtils.GetControlName(selectedObject);
                    if ((controlName == null) || (controlName.Length == 0))
                        Text = HawkeyeAppUtils.AppName + "(unknown object name)";
                    else Text = HawkeyeAppUtils.AppName + controlName;

                    txtType.Text = selectedObject.GetType().FullName;
                    try
                    {
                        txtToString.Text = selectedObject.ToString();
                    }
                    catch (Exception ex)
                    {
                        txtToString.Text = "<ex:>" + ex.Message;
                        Trace.WriteLine("ChangeSelectedObject: selectedObjectToString:" + ex.ToString(), "Hawkeye");
                    }

                    ShowTail(txtType);
                    ShowTail(txtToString);
                    return true;
                }

                txtToString.Text = string.Empty;
                if (NativeUtils.IsTargetInDifferentProcess(this.windowFinder.SelectedHandle))
                {
                    if (windowFinder.IsManagedByClassName)
                    {
                        txtType.Text = "<target in different process. release selection to hook>";
                        //TODO: this always return true on x86 machines!
                        bool flag = NativeUtils.IsProcessX64(NativeUtils.GetProcessForWindow(windowFinder.SelectedHandle));
                        Text = HawkeyeAppUtils.AppName + ".Net " + (flag ? "x64 " : "") + "Process";
                    }
                    else
                    {
                        txtType.Text = "<target not in a managed process>";
                        Text = HawkeyeAppUtils.AppName + "not a managed process";
                    }
                }
                else
                {
                    if (windowFinder.Window.IsValid)
                        txtToString.Text = "ClassName:" + this.windowFinder.Window.ClassName;
                    else txtType.Text = "<no selection or unknown window>";
                    Text = HawkeyeAppUtils.AppName;
                }
                return false;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Concat(new object[] 
                { 
                    "Exception activating object:", selectedObject, ":", ex.ToString() 
                }), "Hawkeye");

                txtType.Text = "Ex:" + ex.Message;
                txtToString.Text = ex.ToString().Replace("\r\n", "--");
                return true;
            }
        }

        private void HideBalloon()
        {
            if (messageBalloon != null)
            {
                messageBalloon.Hide();
                messageBalloon.Dispose();
                messageBalloon = null;
            }
        }

        private void ShowBalloonHelp()
        {
            if (ApplicationOptions.Instance.ShowBalloonHelp)
            {
                messageBalloon = new MessageBalloon(windowFinder);
                messageBalloon.Align = BalloonAlignment.BottomMiddle;
                messageBalloon.TitleIcon = TooltipIcon.Info;
                if (!CoreApplicationOptions.Instance.IsInjected)
                {
                    messageBalloon.Title = "Drag & Drop me on any .Net Control!";
                    messageBalloon.Text = "Hawkeye: Aim. Zoom. Reveal!\r\n\r\nTo start editing .Net objects Drag and Drop the target on ANY .Net Control in ANY .Net process.";
                }
                else
                {
                    messageBalloon.Title = "I just attached to: " + Application.ProductName;
                    messageBalloon.Text = "You can now start editing any object from this process. You can even attach me to another process. Just Drag & Drop me to another .Net Control!";
                }
                messageBalloon.Show();
            }
        }

        private void ShowTail(TextBox txtBox)
        {
            txtBox.SelectionStart = txtBox.Text.Length;
            txtBox.SelectionLength = 0;
        }

        public void ShowToolsWindow(ToolWindowEnum activeToolWindow)
        {
            if ((toolsWindow == null) || toolsWindow.IsDisposed)
            {
                toolsWindow = new ToolsWindow();
                toolsWindow.Closed += new EventHandler(toolsWindow_Closed);
            }

            toolsWindow.Owner = this;
            toolsWindow.Show();
            showToolsButton.Pushed = true;
            toolsWindow.ActiveToolWindow = activeToolWindow;
        }

        private void mainPanel_LocationChanged(object sender, EventArgs e)
        {
            if ((messageBalloon != null) && messageBalloon.Visible)
            {
                messageBalloon.Hide();
                messageBalloon.Show();
            }
        }

        private void propertyGrid_AddToolbarButtons(object sender, EventArgs e)
        {
            showToolsButton = propertyGrid.AddToolbarButton(-1, "Show Tools", "Tools.bmp", 
                delegate
                {
                    if (showToolsButton.Pushed)
                        toolsWindow.Close();
                    else ShowToolsWindow(ToolWindowEnum.Unknown);
                });

            propertyGrid.AddToolbarButton(-1, "About Hawkeye", "WhiteEye.bmp",
                delegate
                {
                    using (Form about = new About())
                        about.ShowDialog(this);
                    //new ACorns.Hawkeye.Resources.About.About().ShowDialog(this);
                });
        }

        private void propertyGrid_SelectRequest(object newObject)
        {
            ChangeSelectedObject(newObject);
        }

        private void toolsWindow_Closed(object sender, EventArgs e)
        {
            showToolsButton.Pushed = false;
        }

        private void windowFinder_ActiveWindowChanged(object sender, EventArgs e)
        {
            object selectedObject = windowFinder.SelectedObject;
            ChangeSelectedObject(selectedObject);
        }

        private void windowFinder_ActiveWindowSelected(object sender, EventArgs e)
        {
            object selectedObject = windowFinder.SelectedObject;
            if ((!ChangeSelectedObject(selectedObject) && (windowFinder.SelectedHandle != IntPtr.Zero)) &&
                NativeUtils.IsTargetInDifferentProcess(windowFinder.SelectedHandle))
            {
                try
                {
                    EditorHawkeyeHook hook = new EditorHawkeyeHook();
                    hook.Hook(this.windowFinder.SelectedHandle, base.Handle);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                }
            }
        }

        private void RuntimeEditor_Closing(object sender, CancelEventArgs e)
        {
            HideBalloon();
            WindowInfo.Save(this);
        }

        private void RuntimeEditor_Deactivate(object sender, EventArgs e)
        {
            HideBalloon();
        }

        private void RuntimeEditor_LocationChanged(object sender, EventArgs e)
        {
            HideBalloon();
        }

        private void windowFinder_MouseDown(object sender, MouseEventArgs e)
        {
            HideBalloon();
        }

        #region IHawkeyeEditor Members

        /// <summary>
        /// Gets Hawkeye's main window toolbar.
        /// </summary>
        /// <value>The toolbar.</value>
        public ToolBar ToolBar
        {
            get { return toolBar; }
        }

        /// <summary>
        /// Gets Hawkeye's main window property grid.
        /// </summary>
        /// <value>The property grid.</value>
        public PropertyGrid PropertyGrid 
        {
            get { return propertyGrid; }
        }

        #endregion
    }
}
