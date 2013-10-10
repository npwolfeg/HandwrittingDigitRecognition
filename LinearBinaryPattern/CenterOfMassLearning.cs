using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LinearBinaryPattern
{
    class CenterOfMassLearning
    {
        string path = @"F:\DigitDB\PictureSaver\";
        static int optionsCount = 10;
        Point[] weights = new Point[optionsCount]; // weights there are the average coords of black pixels

        public CenterOfMassLearning()
        {
            for (int n = 0; n < optionsCount; n++)
                weights[n] = new Point(50, 50);
        }

        public void saveWeights(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                for (int n = 0; n < optionsCount; n++)
                {
                    sw.WriteLine(weights[n].X.ToString());
                    sw.WriteLine(weights[n].Y.ToString());
                }
            }
        }

        public void loadWeights(string path)
        {
            using (StreamReader sw = new StreamReader(path))
            {
                for (int n = 0; n < optionsCount; n++)
                {
                    weights[n].X = Convert.ToInt32(sw.ReadLine());
                    weights[n].Y = Convert.ToInt32(sw.ReadLine());
                }
            }
        }

        public void loadWeights()
        {
            OpenFileDialog of = new OpenFileDialog();
            if (of.ShowDialog() == DialogResult.OK)
            {
                loadWeights(of.FileName);
            }
        }

        public void saveWeights()
        {
            SaveFileDialog sf = new SaveFileDialog();
            if (sf.ShowDialog() == DialogResult.OK)
            {
                saveWeights(sf.FileName);
            }
        }

        public Bitmap visualize()
        {
            Bitmap result = new Bitmap(1000, 100);
            //loadWeights();
            for (int n = 0; n < optionsCount; n++)
                using(Graphics g = Graphics.FromImage(result))
                    g.DrawRectangle(new Pen(Color.Orange, 4),new Rectangle(n * 100 + weights[n].X, weights[n].Y,5,5));
            return result;
        }

        Point Center(Bitmap bmp)
        {
            int counter = 0;
            Point result = new Point(0, 0);
            for (int i=0;i<bmp.Width;i++)
                for(int j=0;j<bmp.Height;j++)
                    if (bmp.GetPixel(i, j).R < 255)
                    {
                        counter++;
                        result.X += i;
                        result.Y += j;
                    }
            result.X = result.X / counter;
            result.Y = result.Y / counter;
            return result;
        }

        private double getDistance(Point p1, Point p2)
        {
            double result = 0;
            for (int i = 0; i < 100; i++)
                for (int j = 0; j < 100; j++)
                    result += Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
            return result;
        }

        public List<double> guess(Bitmap bmp)
        {
            List<double> dist = new List<double>();
            Point currentCenter = Center(bmp);
            for (int n = 0; n < optionsCount; n++)
                dist.Add(getDistance(currentCenter, weights[n]));
            dist = Vector.normalyzeVektor(dist);
            return dist;
        }

        public int[,] guessAll(int guessingCount)
        {
            int[] count = new int[optionsCount];
            //finished = false;
            Bitmap bmp;
            List<double> arr;
            int ID;
            int[,] result = new int[10, 2];
            using (StreamReader sr = new StreamReader(path + "count.txt"))
            {
                for (int i = 0; i < 10; i++)
                    count[i] = Convert.ToInt32(sr.ReadLine());
            }
            for (int n = 0; n < guessingCount; n++)
            {
                for (int k = 0; k < optionsCount; k++)
                {
                    bmp = new Bitmap(path + k.ToString() + n.ToString() + ".bmp");
                    bmp = BmpProcesser.FromAlphaToRGB(bmp);
                    bmp = BmpProcesser.normalizeBitmapRChannel(bmp, 100, 100);
                    arr = guess(bmp);
                    ID = arr.IndexOf(arr.Min());
                    if (ID == k)
                        result[k, 0]++;
                    else
                        result[ID, 1]++;
                }
            }
            //finished = true;
            return result;
        }

        /*public void learnKohonen(Bitmap bmp, int n)
        {
            List<double> arr = guess(bmp);
            int id = arr.IndexOf(arr.Min());
            if (n != id)
            {
                        int realPixel = bmp.GetPixel(i, j).R;
                        for (int x = 0; x < optionsCount; x++)
                        {
                            weights[n, i, j] += delta * (realPixel - weights[n, i, j]);
                            weights[id, i, j] += delta * (weights[n, i, j] - realPixel);
                        }
            }
        }*/

    }
}
