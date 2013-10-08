using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearBinaryPattern
{
    class Vector
    {
        public static double[] normalyzeVektor(double[] vektor)
        {
            double sum = 0;
            for (int i = 0; i < 300; i++)
                sum += vektor[i];
            for (int i = 0; i < 300; i++)
                vektor[i] = vektor[i] / sum;
            return vektor;
        }

        public static List<double> normalyzeVektor(List<double> vektor)
        {
            double sum = 0;
            for (int i = 0; i < vektor.Count; i++)
                sum += vektor[i];
            for (int i = 0; i < vektor.Count; i++)
            {
                vektor[i] = vektor[i] / sum;
                vektor[i] = Math.Round(vektor[i], 3) * 100;
            }
            return vektor;
        }
        public static List<int> normalyzeVektor(List<int> vektor)
        {
            double sum = 0;
            for (int i = 0; i < vektor.Count; i++)
                sum += vektor[i];
            for (int i = 0; i < vektor.Count; i++)
            {
                double temp = vektor[i] / sum;
                vektor[i] = (int) (Math.Round(temp, 4) * 100);
            }
            return vektor;
        }
    }
}
