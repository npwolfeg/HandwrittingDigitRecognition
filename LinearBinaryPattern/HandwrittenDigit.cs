using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace LinearBinaryPattern
{
    class HandwrittenDigit
    {
        public Rectangle bounds;
        public HashSet<Point> points;
        public List<int> possiebleDigits;

        public HandwrittenDigit()
        {
        }

        public HandwrittenDigit(Rectangle bounds, HashSet<Point> points, List<int> possiebleDigits)
        {
            this.bounds = bounds;
            this.points = points;
            this.possiebleDigits = possiebleDigits;
        }
    }
}