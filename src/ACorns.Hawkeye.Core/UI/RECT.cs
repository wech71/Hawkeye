using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ACorns.Hawkeye.Core.UI
{
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
        public RECT(int Left, int Top, int Right, int Bottom)
        {
            this.Left = Left;
            this.Top = Top;
            this.Right = Right;
            this.Bottom = Bottom;
        }

        public int Height
        {
            get
            {
                return (this.Bottom - this.Top);
            }
        }
        public int Width
        {
            get
            {
                return (this.Right - this.Left);
            }
        }
        public System.Drawing.Size Size
        {
            get
            {
                return new System.Drawing.Size(this.Width, this.Height);
            }
        }
        public Point Location
        {
            get
            {
                return new Point(this.Left, this.Top);
            }
        }
        public Rectangle ToRectangle()
        {
            return Rectangle.FromLTRB(this.Left, this.Top, this.Right, this.Bottom);
        }

        public static RECT FromRectangle(Rectangle rectangle)
        {
            return new RECT(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
        }

    }
}

