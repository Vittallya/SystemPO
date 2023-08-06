using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    internal class Raschet
    {
        public double Raschet1(int a, int b, int h, out double average, out double min)
        {
            double y = 0;
            min = double.MaxValue;
            double total = 0;

            double positiveTotal = 0;
            int positiveCount = 0;

            for (double x = a;  x <= b; x += h)
            {
                if (x < 5)
                    y = 2 * Math.Sin(x);
                else if (x >= 5 && x <= 6)
                    y = Math.Sqrt(x + 1);
                else
                    y = Math.Pow(x, 0.3333);
                total += y;

                if(y < min)
                    min = y;

                if (y > 0)
                {
                    positiveTotal += y;
                    positiveCount++;
                }

            }

            average = positiveTotal / positiveCount;

            return y;
        }
    }
}
