using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using ACorns.Hawkeye.Core.Utils;
using ACorns.Hawkeye.Core.Options;

namespace ACorns.Hawkeye.Core.UI
{
    /// <summary>
    /// Summary description for WindowInfo.
    /// </summary>
    [Serializable]
    public class WindowInfo
    {
        private Point location;
        private Size windowSize;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowInfo"/> class.
        /// </summary>
        public WindowInfo() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowInfo"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="windowSize">Size of the window.</param>
        public WindowInfo(Point location, Size windowSize)
        {
            this.location = location;
            this.windowSize = windowSize;
        }

        public Point Location
        {
            get { return location; }
            set { location = value; }
        }

        public Size WindowSize
        {
            get { return windowSize; }
            set { windowSize = value; }
        }

        public static void Save(Form form)
        {
            try
            {
                WindowInfo winInfo = new WindowInfo(form.Location, form.Size);
                string fileName = GetFileName(form);

                using (StreamWriter fs = new StreamWriter(new FileStream(fileName, FileMode.Create)))
                    fs.WriteLine(winInfo.Location.X + "," + winInfo.Location.Y + "," + winInfo.WindowSize.Width + "," + winInfo.WindowSize.Height);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Could not save form :" + form.Name + " details:" + ex.ToString());
            }
        }

        public static bool Load(Form form)
        {
            try
            {
                string fileName = GetFileName(form);
                if (!File.Exists(fileName))
                    return false;

                WindowInfo winInfo = null;

                using (StreamReader fs = new StreamReader(new FileStream(fileName, FileMode.Open)))
                {
                    string info = fs.ReadLine();
                    if (!String.IsNullOrEmpty(info))
                    {
                        string[] parts = info.Split(',');
                        if (parts.Length == 4)
                        {
                            winInfo = new WindowInfo();
                            Point p = new Point();
                            p.X = Int32.Parse(parts[0]);
                            p.Y = Int32.Parse(parts[1]);
                            winInfo.Location = p;

                            Size s = new Size();
                            s.Width = Int32.Parse(parts[2]);
                            s.Height = Int32.Parse(parts[3]);
                            winInfo.WindowSize = s;

                            if ((p.X == 0 && p.Y == 0) || (s.Height == 0 && s.Width == 0))
                            {
                                winInfo = null;
                            }
                        }
                    }
                }

                if (winInfo != null)
                {
                    form.Location = winInfo.Location;
                    form.Size = winInfo.WindowSize;
                }
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Could not load form :" + form.Name + " details:" + ex.ToString());
                return false;
            }
        }

        private static string GetFileName(Form form)
        {
            return Path.Combine(
                CoreApplicationOptions.Instance.FolderName, form.Name + "Window.ini");
        }
    }
}
