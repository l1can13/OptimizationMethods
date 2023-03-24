using System;
using System.Numerics;

namespace OptimizationMethods
{
    class Program
    {
        public static void Main(string[] args)
        {
            DemoLabTwo();
        }

        public static double Testf1(double arg)
        {
            return arg * (arg - 5);
        }

        static double Testf2(Vector args)
        {
            return (args[0] - 2) * (args[0] - 2) + (args[1] - 2) * (args[1] - 2);
        }

        public static void DemoLabOne()
        {
            Console.WriteLine("\n////////////////////\n");
            Console.WriteLine("/// Lab. work #1 ///\n");
            Console.WriteLine("////////////////////\n\n");

            double x_0 = 10;
            double x_1 = -1;

            Console.WriteLine($"x_0 = {x_0}, x_1 = {x_1}\n");
            Console.WriteLine($"BiSect      : {LabOneImplementation.BiSect(Testf1, x_0, x_1, 1e-3)}");
            Console.WriteLine($"GoldenRatio : {LabOneImplementation.GoldenRatio(Testf1, x_0, x_1, 1e-3)}");
            Console.WriteLine($"Fibonacchi  : {LabOneImplementation.Fibonacci(Testf1, x_0, x_1, 1e-3)}\n");
        }

        public static void DemoLabTwo()
        {
            Console.WriteLine("\n////////////////////\n");
            Console.WriteLine("/// Lab. work #2 ///\n");
            Console.WriteLine("////////////////////\n\n");

            Vector x_0 = new double[] { 5, 5 };
            Vector x_1 = new double[] { 0, 0 };
            
            Console.WriteLine($"x_0 = {x_0}, x_1 = {x_1}\n");
            Console.WriteLine($"BiSect           : {LabTwoImplementation.BiSect(Testf2, x_1, x_0)}");
            Console.WriteLine($"BiSectSTRELKOV   : {BiSect(Testf2, x_1, x_0)}");
            Console.WriteLine($"PerCoordDescend  : {LabTwoImplementation.PerCoordDescend(Testf2, x_1)}");
            Console.WriteLine($"PerCoordDescendSTRELKOV   : {PerCoordDescend(Testf2, x_1)}");
        }

        public static Vector PerCoordDescend(FunctionND f, Vector x_start, double eps = 1e-5, int max_iters = 1000)
        {
            Vector x_0 = new Vector(x_start);

            Vector x_1 = new Vector(x_start);

            double step = 1.0;

            double x_i, y_1, y_0;

            int opt_coord_n = 0, coord_id;

            int i = 0;

            for (i = 0; i < max_iters; i++)
            {
                coord_id = i % x_0.Count;

                x_1[coord_id] -= eps;

                y_0 = f(x_1);

                x_1[coord_id] += 2 * eps;

                y_1 = f(x_1);

                x_1[coord_id] -= eps;

                x_1[coord_id] = y_0 > y_1 ? x_1[coord_id] += step : x_1[coord_id] -= step;

                x_i = x_0[coord_id];

                x_1 = BiSect(f, x_0, x_1, eps, max_iters);

                x_0 = new Vector(x_1);

                if (Math.Abs(x_1[coord_id] - x_i) < eps)
                {
                    opt_coord_n++;

                    if (opt_coord_n == x_1.Count)
                    {
#if DEBUG
                        Console.WriteLine($"per coord descend iterations number : {i}");
#endif
                        return x_0;
                    }
                    continue;
                }
                opt_coord_n = 0;
            }
#if DEBUG
            Console.WriteLine($"per coord descend iterations number : {max_iters}");
#endif
            return x_0;
        }

        public static Vector BiSect(FunctionND f, Vector x_0, Vector x_1, double eps = 1e-5, int max_iters = 1000)
        {
            Vector x_c, dir;

            dir = Vector.Direction(x_0, x_1) * eps;

            int cntr = 0;

            for (; cntr != max_iters; cntr++)
            {
                if ((x_1 - x_0).Magnitude < eps) break;

                x_c = (x_1 + x_0) * (0.5);

                if (f(x_c + dir) > f(x_c - dir))
                {
                    x_1 = x_c;
                    continue;
                }
                x_0 = x_c;
            }
#if DEBUG
            Console.WriteLine($"dihotomia iterations number : {cntr}");
#endif
            return (x_1 + x_0) * 0.5;
        }
    }
}