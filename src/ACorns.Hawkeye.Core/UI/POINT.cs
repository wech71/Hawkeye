using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ACorns.Hawkeye.Utils.UI
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
        public POINT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public POINT ToPoint()
        {
            return new POINT(this.x, this.y);
        }

        public static POINT FromPoint(Point pt)
        {
            return new POINT(pt.X, pt.Y);
        }

        public override bool Equals(object obj)
        {
            if (obj is POINT)
            {
                POINT point1 = (POINT) obj;
                if (point1.x == this.x)
                {
                    return (point1.y == this.y);
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (this.x ^ this.y);
        }

        public override string ToString()
        {
            return string.Format("{{X={0}, Y={1}}}", this.x, this.y);
        }

    }
}

