using System;

namespace OptimizationMethods
{
    public static class LabFourImplementation
    {
        public static Vector NewtoneRaphson(FunctionND f, Vector x_start, double eps = 1e-6, int max_iters = 1000)
        {
            Vector x_i = new Vector(x_start);

            Vector x_i_1 = new Vector(x_start);

            int cntr = 0;

            for (; cntr <= max_iters; ++cntr)
            {
                x_i_1 = x_i - Matrix.Invert(Matrix.Hessian(f, x_i, eps)) * Vector.Gradient(f, x_i, eps);

                if ((x_i_1 - x_i).Magnitude < eps)
                {
                    break;
                }

                x_i = x_i_1;
            }
            
            Console.WriteLine($"Newtone - Raphson iterations number : {cntr}");
            
            return (x_i_1 + x_i) * 0.5;
        }

        public static double InternalPenalty(Vector x, Matrix n, Vector d)
        {
            double res = 0.0;
            
            Vector ro = n * x - d;

            for (int i = 0; i < ro.Count; ++i)
            {
                res += 1 / (ro[i] * ro[i]);
            }
            
            return res;
        }

        public static double ExternalPenalty(Vector x, Matrix n, Vector d)
        {
            Vector dist = n * x - d;
            Vector distPowered = new Vector(dist.Count);
            
            for (int i = 0; i < dist.Count; ++i)
            {
                distPowered[i] = dist[i] * dist[i];
            }
            
            Vector distHaviside = Haviside(dist, 1);
            double res = distHaviside.Dot(distPowered);
            
            return res;
        }

        private static Vector Haviside(Vector vector, int value)
        {
            Vector res = new Vector(vector.Count);
            for (int i = 0; i < vector.Count; ++i)
            {
                int resultItem = 0;
                if (Math.Abs(vector[i] - 0) < 1e-16)
                {
                    resultItem = value;
                }
                else if (vector[i] > 0)
                {
                    resultItem = 1;
                }
                res[i] = resultItem;
            }
            return res;
        }

        private static int haviside(double number, int value)
        {
            int resultItem = 0;
            if (Math.Abs(number - 0) < 1e-16)
            {
                resultItem = value;
            }
            else if (number > 0)
            {
                resultItem = 1;
            }
            return resultItem;
        }

        public static double circularInternalPenalty(double r, double x0, double y0, double x, double y)
        {
            return Math.Pow((r * r - Math.Pow(x - x0, 2) - Math.Pow(y - y0, 2)), -2);
        }

        public static double circularExternalPenalty(double r, double x0, double y0, double x, double y)
        {
            double dist = Math.Pow(x - x0, 2) + Math.Pow(y - y0, 2) - r * r;
            int distHaviside = Math.Abs(dist - 0.0) < 1e-16 ? 1 : 0;
            
            return distHaviside * dist;
        }
    }
}
