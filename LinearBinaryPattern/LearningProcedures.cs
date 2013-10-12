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
        //public double delta = 1;
        //int optionsCount;
        //int vectorLength;
        CenterLearning learner;        

        public LearningProcedures(CenterLearning learner)
        {
            this.learner = learner;
            //optionsCount = learner.optionsCount;
            //vectorLength = learner.vectorLength;
        }

        static public List<double> guess(double[] vector, int optionsCount, double[][]weights)
        {
            List<double> dist = new List<double>();
            for (int n = 0; n < optionsCount; n++)
                dist.Add(Distances.EuclidDistance(vector, weights[n]));
            dist = Vector.normalyzeVektor(dist);
            return dist;
        }

        public double[][] learnKohonen(double[] vector, int n, double[][] weights, int optionsCount, double delta)
        {
            List<double> arr = guess(vector,optionsCount,weights);
            int id = arr.IndexOf(arr.Min());
            if (n != id)
                for (int i = 0; i < vector.Length; i++)
                {
                    weights[n][i] += delta * (vector[i] - weights[n][i]);
                    weights[id][i] += delta * (weights[n][i] - vector[i]);
                }

            else
                for (int i = 0; i < vector.Length; i++)
                    weights[n][i] += delta * (vector[i] - weights[n][i]);
            return weights;
        }

        public double[][] learnAll(int learningCount, BackgroundWorker bw, bool linearDelta, double deltaAtTheEnd, Object parameters)
        {
            double delta = 1;
            int optionsCount = learner.optionsCount;
            int vectorLength = learner.vectorLength;
            double[][] weights;
            weights = new double[optionsCount][];
            for (int n = 0; n < optionsCount; n++)
            {
                weights[n] = new double[vectorLength];
                for (int i = 0; i < vectorLength; i++)
                    weights[n][i] = 0;
            }
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
                    learnKohonen(learner.getVector(bmp,parameters), k, weights, optionsCount,delta);
                }
                if (deltaAtTheEnd >= 1)
                    deltaAtTheEnd = 0.99;
                if (linearDelta)
                {
                    delta = -(double)progress / ((1/(1-deltaAtTheEnd)) * maxProgress) + 1;
                }
                else
                {
                    if (deltaAtTheEnd <=0)
                        deltaAtTheEnd = 0.01;
                    double a = deltaAtTheEnd / (1 - deltaAtTheEnd);
                    delta = a * learningCount / ((double)progress + a * learningCount);
                }
            }
            return weights;
        }

        public double[][] learnAllAverage(int learningCount, BackgroundWorker bw, Object parameters)
        {
            int optionsCount = learner.optionsCount;
            int vectorLength = learner.vectorLength;
            double[][] weights;
            weights = new double[optionsCount][];
            for (int n = 0; n < optionsCount; n++)
            {
                weights[n] = new double[vectorLength];
                for (int i = 0; i < vectorLength; i++)
                    weights[n][i] = 0;
            }

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
                    bw.ReportProgress((int)((float)progress / maxProgress * 100));
                    bmp = new Bitmap(path + k.ToString() + n.ToString() + ".bmp");
                    bmp = BmpProcesser.FromAlphaToRGB(bmp);
                    bmp = BmpProcesser.normalizeBitmapRChannel(bmp, 100, 100);
                    vector1 = learner.getVector(bmp, parameters);
                    for (int i = 0; i < vectorLength; i++)
                        weights[k][i] += vector1[i];
                }
            }
            for (int k = 0; k < optionsCount; k++)
                for (int i = 0; i < vectorLength; i++)
                    weights[k][i] = weights[k][i] / learningCount;
            return weights;
        }

        public int[,] guessAll(int guessingCount, BackgroundWorker bw)
        {
            int optionsCount = learner.optionsCount;
            int vectorLength = learner.vectorLength;
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

        private void saveGuess(int[,] rightNwrong, string currenPath)
        {
            int sum = 0;
            currenPath += " resut = ";
            using (StreamWriter sw = new StreamWriter(currenPath + ".txt"))
            {
                for (int i = 0; i < 10; i++)
                {
                    sw.WriteLine(i.ToString() + " " + rightNwrong[i, 0].ToString() + " " + rightNwrong[i, 1].ToString());
                    sum += rightNwrong[i, 0];
                }
                sw.WriteLine(sum);
            }
            File.Move(currenPath + ".txt", currenPath + sum + ".txt");
        }

        public void AutoTest(BackgroundWorker bw, string path, List<Object> parameters)
        {
            string currenPath;
            bool linearDelta = false;
            for (int x = 0; x < 2; x++) //to test with different delta functions
            {
                for (double deltaAtTheEnd = 0; deltaAtTheEnd < 0.5; deltaAtTheEnd += 0.2)
                {
                    string deltaFunc;
                    if (linearDelta)
                        deltaFunc = " linearDelta ";
                    else
                        deltaFunc = " nonLinearDelta ";
                    currenPath = path + "kohonen" + deltaFunc + deltaAtTheEnd.ToString();
                    learner.learnAllKohonen(100, bw, false, deltaAtTheEnd,parameters[0]);
                    learner.saveWeights(currenPath + ".txt");                    
                    saveGuess(learner.guessAll(100, bw),currenPath);
                }
                linearDelta = true;
            }
            currenPath = path + "average ";
            learner.learnAllAverage(100, bw, parameters[0]);
            learner.saveWeights(currenPath + ".txt");
            saveGuess(learner.guessAll(100, bw), currenPath);            
        }
    }
}
