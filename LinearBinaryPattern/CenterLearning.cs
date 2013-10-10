using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;

namespace LinearBinaryPattern
{
    class CenterLearning
    {
        string path = @"F:\DigitDB\PictureSaver\";
        static int blockRows = 32;
        static int blockCols = 32;
        static int picWidth = 100;
        static int picHeight = 100;
        static int blockWidth = picWidth / blockCols;
        static int blockHeight = picHeight / blockRows;
        static int optionsCount = 10;
        static int vectorLength = 2 * blockRows * blockCols;
        double[][] weights = new double[optionsCount][];

        public double delta = 1;
        public int progress = 0;
        public int maxProgress;
        public bool finished;


        public CenterLearning()
        {
            for (int n = 0; n < optionsCount; n++)
            {
                weights[n] = new double[vectorLength];
                for (int i = 0; i < vectorLength; i++)
                    if (i % 2 == 0)
                        weights[n][i] = blockWidth / 2;
                    else
                        weights[n][i] = blockHeight / 2;
            }
        }

        public void saveWeights(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                for (int n = 0; n < optionsCount; n++)
                    for (int i = 0; i < vectorLength; i++)
                        sw.WriteLine(weights[n][i].ToString());
            }
        }

        public void loadWeights(string path)
        {
            using (StreamReader sw = new StreamReader(path))
            {
                for (int n = 0; n < optionsCount; n++)
                    for (int i = 0; i < vectorLength; i++)
                    weights[n][i] = Convert.ToDouble(sw.ReadLine());
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
            loadWeights();
            for (int n = 0; n < optionsCount; n++)
                using (Graphics g = Graphics.FromImage(result))
                {
                    int counter = 0;
                    for (int i = 0; i < blockCols; i++)
                        for (int j = 0; j < blockRows; j++)
                        {
                            //g.DrawRectangle(new Pen(Color.Blue, 4), n * picWidth + i * blockWidth, j * blockHeight, blockWidth, blockHeight);
                            g.DrawRectangle(new Pen(Color.Orange, 4), new Rectangle(n * picWidth + i * blockWidth + (int)weights[n][counter], j * blockHeight + (int)weights[n][counter+1], 1, 1));
                            //g.DrawLine(new Pen(Color.Black, 1), n * picWidth + i * blockWidth + blockWidth / 2, j * blockHeight, n * picWidth + i * blockWidth + blockWidth / 2, (j + 1) * blockHeight);
                            //g.DrawLine(new Pen(Color.Black, 1), n * picWidth + i * blockWidth, j * blockHeight + blockHeight/2, n * picWidth + (i+1) * blockWidth, j * blockHeight + blockHeight/2);
                            counter++;
                        }
                }
            return result;
        }

        double[] Center(Bitmap bmp)
        {
            int counter = 0;
            double[] result = new double[vectorLength];
            for (int i=0;i<bmp.Width;i++)
                for(int j=0;j<bmp.Height;j++)
                    if (bmp.GetPixel(i, j).R < 255)
                    {
                        counter++;
                        result[0] += i;
                        result[1] += j;
                    }
            if (counter != 0)
            {
                result[0] = result[0] / counter;
                result[1] = result[1] / counter;
            }
            else
            {
                result[0] = blockWidth / 2;
                result[1] = blockHeight / 2;
            }
            return result;
        }

        private double getDistance(double[] vector1, double[] vector2)
        {
            double result = 0;
            for (int i = 0; i < vectorLength; i++)
                    result += Math.Pow(vector1[i] - vector2[i], 2);
            return Math.Sqrt(result);
        }

        public List<double> guess(Bitmap bmp)
        {
            List<double> dist = new List<double>();
            double[] vector1 = new double[vectorLength];
            Rectangle copyRect;
            int counter = 0;

            for(int i=0;i<blockCols;i++)
                for(int j=0;j<blockRows;j++)
                {
                    copyRect = new Rectangle(i * blockWidth, j * blockHeight, blockWidth, blockHeight);
                    Bitmap partOfBmp = BmpProcesser.copyPartOfBitmap(bmp,copyRect);
                    double[] currentCenter = Center(partOfBmp);
                    vector1[counter] = currentCenter[0];
                    vector1[counter+1] = currentCenter[1];
                    counter++;
                }            
            for (int n = 0; n < optionsCount; n++)
                dist.Add(getDistance(vector1, weights[n]));
            dist = Vector.normalyzeVektor(dist);
            return dist;
        }

        //duplicate!!
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

        public void learnKohonen(Bitmap bmp, int n)
        {
            List<double> arr = guess(bmp);
            int id = arr.IndexOf(arr.Min());
            double[] vector1 = new double[vectorLength];
            Rectangle copyRect;
            int counter = 0;
            if (n != id)
            {
                for (int i = 0; i < blockCols; i++)
                    for (int j = 0; j < blockRows; j++)
                    {
                        copyRect = new Rectangle(i * blockWidth, j * blockHeight, blockWidth, blockHeight);
                        Bitmap partOfBmp = BmpProcesser.copyPartOfBitmap(bmp, copyRect);
                        double[] currentCenter = Center(partOfBmp);
                        vector1[counter] = currentCenter[0];
                        vector1[counter + 1] = currentCenter[1];
                        counter++;
                    }  
                for (int i = 0; i < vectorLength; i++)
                {
                    weights[n][i] += (int)delta * (vector1[i] - weights[n][i]);
                    weights[id][i] += (int)delta * (weights[n][i] - vector1[i]);
                }
            }
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

        public void learnAllAverage(Object learningCount)
        {
            double[] vector1 = new double[vectorLength];
            Rectangle copyRect;
            int counter = 0;
            for (int n = 0; n < optionsCount; n++)
            {
                weights[n] = new double[vectorLength];
                for (int i = 0; i < vectorLength; i++)
                        weights[n][i] = 0;
            }

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
                    counter = 0;
                    for (int i = 0; i < blockCols; i++)
                        for (int j = 0; j < blockRows; j++)
                        {
                            copyRect = new Rectangle(i * blockWidth, j * blockHeight, blockWidth, blockHeight);
                            Bitmap partOfBmp = BmpProcesser.copyPartOfBitmap(bmp, copyRect);
                            double[] currentCenter = Center(partOfBmp);
                            vector1[counter] = currentCenter[0];
                            vector1[counter + 1] = currentCenter[1];
                            counter++;
                        }
                    for (int i = 0; i < vectorLength; i++)
                        weights[k][i] += vector1[i];
                }
            }
            for (int k = 0; k < optionsCount; k++)
            for (int i = 0; i < vectorLength; i++)
                weights[k][i] = weights[k][i] / intLearningCount;
            finished = true;
        }

    }
}
