using System;
using System.Windows.Forms;
using ACorns.Hawkeye.Utils;
using System.Diagnostics;
using System.Resources;
using System.IO;

namespace ACorns.Hawkeye
{
    internal partial class About : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="About"/> class.
        /// </summary>
        public About()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Text = HawkeyeAppUtils.FullApplicationName;

            backHomeBaby.LinkClicked += delegate { Process.Start(backHomeBaby.Text); };
            licenseLink.LinkClicked += delegate { Process.Start(licenseLink.Text); };
            
            using (Stream stream =GetType().Assembly.GetManifestResourceStream(GetType(), "Credits.txt"))
            using (StreamReader reader = new StreamReader(stream))
            {
                creditsBox.Text = reader.ReadToEnd();
                reader.Close();                
            }

            using (Stream stream = GetType().Assembly.GetManifestResourceStream(GetType(), "History.txt"))
            using (StreamReader reader = new StreamReader(stream))
            {
                historyBox.Text = reader.ReadToEnd();
                reader.Close();
            }
        }
    }
}