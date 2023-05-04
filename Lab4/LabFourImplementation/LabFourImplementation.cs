using System;
using System.Linq;
using System.Numerics;

namespace OptimizationMethods
{
    public static class LabFourImplementation
    {
        public static Vector NewtoneRaphson(FunctionND func, Vector x0, double eps = 1e-6, int max_iters = 100)
        {
            Vector x1 = new Vector(x0.Count);
            for (int i = 0; i < max_iters; i++)
            {
                try
                {
                    Matrix h = Matrix.Invert(Matrix.Hessian(func, x0, eps));
                    x1 = x0 - h * Vector.Gradient(func, x0, eps);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                if ((x1 - x0).Magnitude < eps)
                {
                    break;
                }
                x0 = x1;
            }
            return x0;
        }

        public static double InternalPenalty(Vector x, Matrix n, Vector b, double alpha = 1)
        {
            return Enumerable.Range(0, n.NRows)
                .SelectMany(i => Enumerable.Range(0, n.NCols)
                    .Select(j => Math.Pow((n[i][j] * x[j] - b[i]) * alpha, -2.0)))
                .Sum();
        }

        public static double ExternalPenalty(Vector x, Matrix n, Vector d)
        {
            double penalty = 0.0;
            Vector temp_x = new Vector(x.Count);

            for (int i = 0; i < x.Count; i++)
            {
                if (x[i] < 0)
                {
                    penalty += Math.Pow(x[i], 2);
                    temp_x[i] = 0;
                }
                else if (x[i] > 10)
                {
                    penalty += Math.Pow(x[i] - 10, 2);
                    temp_x[i] = 10;
                }
                else
                {
                    temp_x[i] = x[i];
                }
            }

            Vector gradient = Matrix.Transpose(n) * (n * temp_x - d);
            Vector projected_gradient = new Vector(x.Count);

            for (int i = 0; i < x.Count; i++)
            {
                if (temp_x[i] <= 0 && gradient[i] > 0)
                {
                    projected_gradient[i] = 0;
                }
                else if (temp_x[i] >= 10 && gradient[i] < 0)
                {
                    projected_gradient[i] = 0;
                }
                else
                {
                    projected_gradient[i] = gradient[i];
                }
            }

            x = temp_x - Matrix.Invert(Matrix.Transpose(n) * n) * projected_gradient;

            Vector dist = n * x - d;
            Vector distPowered = new Vector(dist.Count);

            for (int i = 0; i < dist.Count; ++i)
            {
                distPowered[i] = dist[i] * dist[i];
            }

            Vector distHaviside = Haviside(dist, 1);
            double res = distHaviside.Dot(distPowered);

            return res + penalty;
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
