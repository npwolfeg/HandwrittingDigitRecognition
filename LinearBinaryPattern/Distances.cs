using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearBinaryPattern
{
    static class Distances
    {
        static public double EuclidDistance(double[] vector1, double[] vector2)
        {
            double result = 0;
            if (vector1.Length == vector2.Length)
            for (int i = 0; i < vector1.Length; i++)
                result += Math.Pow(vector1[i] - vector2[i], 2);
            return Math.Sqrt(result);
        }
    }
}
