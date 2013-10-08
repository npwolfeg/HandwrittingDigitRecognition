using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace LinearBinaryPattern
{
    class DigitsToNumbersLogic
    {
        private static double distance(Rectangle rect1, Rectangle rect2)
        {
            double x1 = rect1.X + rect1.Width / 2;
            double x2 = rect2.X + rect2.Width / 2;
            double y1 = rect1.Y + rect1.Height / 2;
            double y2 = rect2.Y + rect2.Height / 2;
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        private static bool isNear(Rectangle rect1, Rectangle rect2)
        {
            int maxWidth = Math.Max(rect1.Width,rect2.Width);
            return distance(rect1, rect2)/2 < maxWidth;
        }

        private static bool isHeightTheSame(Rectangle rect1, Rectangle rect2, int deltaPercent)
        {
            double delta = Math.Max(rect1.Height, rect2.Height) * deltaPercent / 100;
            return Math.Abs(rect1.Height - rect2.Height) < delta;
        }

        private static bool isSameNumber(Rectangle rect1, Rectangle rect2)
        {
            return isNear(rect1, rect2) & isHeightTheSame(rect1, rect2,25);
        }

        private static int[] digitsToNumbers(List<Rectangle> digitRects)
        {
            int[] digitNumbers = new int[digitRects.Count];
            for (int i = 0; i < digitRects.Count; i++)
                digitNumbers[i] = i;
            for (int i = 0; i < digitRects.Count - 1; i++)
                for (int j = i + 1; j < digitRects.Count; j++)
                    if (isSameNumber(digitRects[i], digitRects[j]))
                        digitNumbers[j] = digitNumbers[i];
            return digitNumbers;
        }

        public static List<Rectangle> numbersRects(List<Rectangle> digitRects)
        {
            int[] digitNumbers = digitsToNumbers(digitRects);
            // to find number of different numbers
            HashSet<int> ints = new HashSet<int>(); 
            foreach (int number in digitNumbers)
                ints.Add(number);
            int count = ints.Count;
            /////////////////////////////////////
            List<Rectangle> result = new List<Rectangle>();
            foreach (int number in ints)
            {
                int left = 100000;
                int right = 0;
                int top = 1000000;
                int bot = 0;
                for(int i=0;i<digitNumbers.Length;i++)
                    if (digitNumbers[i] == number)
                    {
                        if (digitRects[i].Left < left) left = digitRects[i].Left;
                        if (digitRects[i].Right > right) right = digitRects[i].Right;
                        if (digitRects[i].Top < top) top = digitRects[i].Top;
                        if (digitRects[i].Bottom > bot) bot = digitRects[i].Bottom;
                    }
                result.Add(new Rectangle(left,top,right-left,bot-top));
            }
            return result;
        }
    }
}
