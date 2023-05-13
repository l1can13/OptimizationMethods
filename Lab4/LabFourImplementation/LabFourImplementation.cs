using System;
using System.Linq;
using System.Numerics;

namespace OptimizationMethods
{
    public static class LabFourImplementation
    {
        public static Vector NewtonRaphson(FunctionND func, Vector x0, double eps = 1e-6, int max_iters = 100)
        {
            Vector x1 = new Vector(x0);
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
           var distance = n * x - d;
           foreach (var item in distance) penalty += item >= 0 ? Math.Pow(item, 2) : 0.0;
           return penalty;
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
