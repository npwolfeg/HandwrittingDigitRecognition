using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace LinearBinaryPattern
{
    class Pixel
    {
        public int x, y;
        public Color color;

        public Pixel(int x, int y, Color c)
        {
            this.x = x;
            this.y = y;
            this.color = c;
        }
    }
}
