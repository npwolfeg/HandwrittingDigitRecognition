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

        private static bool isOnTheSameLine(Rectangle rect1, Rectangle rect2, int deltaPercent)
        {
            double delta = Math.Max(rect1.Height, rect2.Height) * deltaPercent / 100;
            return Math.Abs(rect1.Bottom - rect2.Bottom) < delta;
        }

        private static bool isSameNumber(Rectangle rect1, Rectangle rect2)
        {
            return isNear(rect1, rect2) & isHeightTheSame(rect1, rect2, 25) & isOnTheSameLine(rect1, rect2, 25);
        }

        public static List<List<int>> digitsToNumbers(List<HandwrittenDigit> digits)
        {
            List<Rectangle> digitRects = new List<Rectangle>();
            foreach (HandwrittenDigit digit in digits)
                digitRects.Add(digit.bounds);
            return digitsToNumbers(digitRects);
        }

        private static List<List<int>> digitsToNumbers(List<Rectangle> digitRects)
        {

            int[] digitNumbers = new int[digitRects.Count];
            for (int i = 0; i < digitRects.Count; i++)
                digitNumbers[i] = i;
            for (int i = 0; i < digitRects.Count - 1; i++)
                for (int j = i + 1; j < digitRects.Count; j++)
                    if (isSameNumber(digitRects[i], digitRects[j]))
                        digitNumbers[j] = digitNumbers[i];               //now each digit in digitNumbers contains number of its number oO

            HashSet<int> numbers = new HashSet<int>();
            foreach (int number in digitNumbers)
                numbers.Add(number);                                     //set of DIFFERENT numbers

            List<List<int>> result = new List<List<int>>();
            foreach (int number in numbers)
                result.Add(new List<int>());
            for (int i = 0; i < digitNumbers.Length; i++)
                result[numbers.ToList().IndexOf(digitNumbers[i])].Add(i); //list of numbers. each number has a list of its digits

            return result;
        }
    }
}
