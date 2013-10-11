using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using System.Drawing;

namespace LinearBinaryPattern
{
    class LearningProcedures
    {
        string path = @"F:\DigitDB\PictureSaver\";        
        public double delta = 1;
        int optionsCount;
        int vectorLength;
        double[][] weights;
        CenterLearning learner;        

        public LearningProcedures(CenterLearning learner)
        {
            this.learner = learner;
            optionsCount = learner.optionsCount;
            vectorLength = learner.vectorLength;
            initializeWeights();
        }

        private void initializeWeights()
        {
            weights = new double[optionsCount][];
            for (int n = 0; n < optionsCount; n++)
            {
                weights[n] = new double[vectorLength];
                for (int i = 0; i < vectorLength; i++)
                    weights[n][i] = 0;
            }
        }

        static public List<double> guess(double[] vector, int optionsCount, double[][]weights )
        {
            List<double> dist = new List<double>();
            for (int n = 0; n < optionsCount; n++)
                dist.Add(Distances.EuclidDistance(vector, weights[n]));
            dist = Vector.normalyzeVektor(dist);
            return dist;
        }

        public void learnKohonen(double[] vector, int n)
        {
            List<double> arr = guess(vector,optionsCount,weights);
            int id = arr.IndexOf(arr.Min());
            if (n != id)
                for (int i = 0; i < vectorLength; i++)
                {
                    weights[n][i] += delta * (vector[i] - weights[n][i]);
                    weights[id][i] += delta * (weights[n][i] - vector[i]);
                }

            else
                for (int i = 0; i < vectorLength; i++)
                    weights[n][i] += delta * (vector[i] - weights[n][i]);
        }

        public double[][] learnAll(int learningCount, BackgroundWorker bw)
        {
            initializeWeights();
            int progress, maxProgress;
            int[] count = new int[optionsCount];
            Bitmap bmp;
            using (StreamReader sr = new StreamReader(path + "count.txt"))
            {
                for (int i = 0; i < optionsCount; i++)
                    count[i] = Convert.ToInt32(sr.ReadLine());
            }
            progress = 0;
            maxProgress = learningCount * optionsCount;
            for (int n = 0; n < learningCount; n++)
            {
                for (int k = 0; k < optionsCount; k++)
                {
                    progress++;
                    bw.ReportProgress((int)((float)progress / maxProgress * 100));
                    bmp = new Bitmap(path + k.ToString() + n.ToString() + ".bmp");
                    bmp = BmpProcesser.FromAlphaToRGB(bmp);
                    bmp = BmpProcesser.normalizeBitmapRChannel(bmp, 100, 100);
                    learnKohonen(learner.getVector(bmp), k);
                }
                delta = -(double)progress / (1 * maxProgress) + 1;
            }
            return weights;
        }

        public void learnAllAverage(int learningCount)
        {
            initializeWeights();
            int progress, maxProgress;
            double[] vector1 = new double[vectorLength];            
            int[] count = new int[optionsCount];
            Bitmap bmp;
            using (StreamReader sr = new StreamReader(path + "count.txt"))
            {
                for (int i = 0; i < optionsCount; i++)
                    count[i] = Convert.ToInt32(sr.ReadLine());
            }
            progress = 0;
            maxProgress = learningCount * optionsCount;
            for (int n = 0; n < learningCount; n++)
            {
                for (int k = 0; k < optionsCount; k++)
                {
                    progress++;
                    bmp = new Bitmap(path + k.ToString() + n.ToString() + ".bmp");
                    bmp = BmpProcesser.FromAlphaToRGB(bmp);
                    bmp = BmpProcesser.normalizeBitmapRChannel(bmp, 100, 100);
                    vector1 = learner.getVector(bmp);
                    for (int i = 0; i < vectorLength; i++)
                        weights[k][i] += vector1[i];
                }
            }
            for (int k = 0; k < optionsCount; k++)
                for (int i = 0; i < vectorLength; i++)
                    weights[k][i] = weights[k][i] / learningCount;
        }

        public int[,] guessAll(int guessingCount, BackgroundWorker bw)
        {
            int progress, maxProgress;            
            progress = 0;
            maxProgress = guessingCount * optionsCount;
            int[] count = new int[optionsCount];
            Bitmap bmp;
            List<double> arr;
            int ID;
            int[,] result = new int[optionsCount, 2];
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
                    arr = learner.guess(bmp);
                    ID = arr.IndexOf(arr.Min());
                    if (ID == k)
                        result[k, 0]++;
                    else
                        result[ID, 1]++;
                }
            }
            return result;
        }
    }
}
