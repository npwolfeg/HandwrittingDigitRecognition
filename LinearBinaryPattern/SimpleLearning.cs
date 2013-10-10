using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;

namespace LinearBinaryPattern
{
    public class SimpleLearning
    {
        static int optionsCount = 10;
        public double[, ,] weights = new double[optionsCount, 100, 100];
        string path = @"F:\DigitDB\PictureSaver\";
        //string path = @"F:\DigitDB\PictureSaverThin\";
        public int progress = 0;
        public int maxProgress = 0;
        public bool finished = false;
        public double delta = 1;

        public SimpleLearning()
        {
            for (int n = 0; n < optionsCount; n++)
                for (int i = 0; i < 100; i++)
                    for (int j = 0; j < 100; j++)
                        weights[n, i, j] = 127;
        }

        public void saveWeights(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                for (int n = 0; n < optionsCount; n++)
                    for (int i = 0; i < 100; i++)
                        for (int j = 0; j < 100; j++)
                            sw.WriteLine(weights[n, i, j].ToString());
            }
        }

        public void loadWeights(string path)
        {
            using (StreamReader sw = new StreamReader(path))
            {
                for (int n = 0; n < optionsCount; n++)
                    for (int i = 0; i < 100; i++)
                        for (int j = 0; j < 100; j++)
                            weights[n, i, j] = Convert.ToDouble(sw.ReadLine());
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

        public Bitmap visualize()
        {
            Bitmap result = new Bitmap(1000, 100);
            loadWeights();
            for (int n = 0; n < optionsCount; n++)
                for (int i = 0; i < 100; i++)
                    for (int j = 0; j < 100; j++)
                    {
                        double la = weights[n, i, j];
                        if (la < 0) la = 0;
                        if (la > 255) la = 255;
                        result.SetPixel(n * 100 + i, j, Color.FromArgb(255, (int)la, (int)la, (int)la));
                    }
            return result;
        }

        private double getDistance(Bitmap bmp, int n)
        {
            double result = 0;
            for (int i = 0; i < 100; i++)
                for (int j = 0; j < 100; j++)
                    result += Math.Abs(bmp.GetPixel(i, j).R - weights[n, i, j]);
            return result;
        }

        public List<double> guess(Bitmap bmp)
        {
            List<double> dist = new List<double>();
            for (int n = 0; n < optionsCount; n++)
                dist.Add(getDistance(bmp, n));
            dist = Vector.normalyzeVektor(dist);
            return dist;
        }

        //duplicate
        public int[,] guessAll(int guessingCount, BackgroundWorker bw)
        {
            progress = 0;
            maxProgress = guessingCount * optionsCount;
            int[] count = new int[optionsCount];
            finished = false;
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
                    progress++;
                    bw.ReportProgress((int)((float)progress / maxProgress * 100));
                    
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
            finished = true;
            return result;
        }

        public void learn(Bitmap bmp, int n)
        {
            List<double> arr = guess(bmp);
            int id = arr.IndexOf(arr.Min());
            if (n != id)
            {
                {
                    for (int i = 0; i < 100; i++)
                        for (int j = 0; j < 100; j++)
                        {
                            int realPixel = bmp.GetPixel(i, j).R;
                            if (realPixel == 255)
                            {
                                weights[n, i, j] = addWithLimit(weights[n, i, j], 255);
                                weights[id, i, j] = decWithLimit(weights[id, i, j], 0);
                            }
                            else
                            {
                                weights[n, i, j] = decWithLimit(weights[n, i, j], 0);
                                weights[id, i, j] = addWithLimit(weights[id, i, j], 255);
                            }

                        }
                }
            }
        }

        public void learnKohonen(Bitmap bmp, int n)
        {
            List<double> arr = guess(bmp);
            int id = arr.IndexOf(arr.Min());
            if (n != id)
            {
                for (int i = 0; i < 100; i++)
                    for (int j = 0; j < 100; j++)
                    {
                        int realPixel = bmp.GetPixel(i, j).R;
                        weights[n, i, j] += delta * (realPixel - weights[n, i, j]);
                        weights[id, i, j] += delta * (weights[n, i, j] - realPixel);
                    }
            }
        }

        public double addWithLimit(double x, int limit)
        {
            x += delta;
            if (x > limit)
                x = limit;
            return x;
        }
        public double decWithLimit(double x, int limit)
        {
            x -= delta;
            if (x < limit)
                x = limit;
            return x;
        }


        public void learnAll(Object learningCount)
        {
            int[] count = new int[optionsCount];
            int intLearningCount = (int)learningCount;
            finished = false;
            Bitmap bmp;
            using (StreamReader sr = new StreamReader(path + "count.txt"))
            {
                for (int i = 0; i < optionsCount; i++)
                    count[i] = Convert.ToInt32(sr.ReadLine());
            }
            progress = 0;
            maxProgress = intLearningCount * optionsCount;
            for (int n = 0; n < intLearningCount; n++)
            {
                for (int k = 0; k < optionsCount; k++)
                {
                    progress++;
                    bmp = new Bitmap(path + k.ToString() + n.ToString() + ".bmp");
                    bmp = BmpProcesser.FromAlphaToRGB(bmp);
                    bmp = BmpProcesser.normalizeBitmapRChannel(bmp, 100, 100);
                    learnKohonen(bmp, k);

                }
                delta = -(double)progress / maxProgress + 1;
            }
            finished = true;
        }
    }
}
