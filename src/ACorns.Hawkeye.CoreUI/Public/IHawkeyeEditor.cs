using System;
using System.Windows.Forms;

namespace ACorns.Hawkeye.Public
{
    public interface IHawkeyeEditor
    {
        /// <summary>
        /// Gets Hawkeye's main window toolbar.
        /// </summary>
        /// <value>The toolbar.</value>
        ToolBar ToolBar { get; }

        /// <summary>
        /// Gets Hawkeye's main window property grid.
        /// </summary>
        /// <value>The property grid.</value>
        PropertyGrid PropertyGrid { get; }
    }
}
