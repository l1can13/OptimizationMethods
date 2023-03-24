using System;
using System.Linq;

namespace OptimizationMethods
{
    public class LabTwoImplementation
    {
        public static Vector BiSect(FunctionND f, Vector x_0, Vector x_1, double eps = 1e-5, int max_iters = 1000)
        {
            Vector x_c, dir = Vector.Direction(x_0, x_1) * eps;

            int cntr = 0;
            while (cntr < max_iters)
            {
                if ((x_1 - x_0).Magnitude < eps) break;

                x_c = (x_1 + x_0) * 0.5;

                x_0 = f(x_c - dir) < f(x_c + dir) ? x_0 : x_c;
                x_1 = f(x_c - dir) < f(x_c + dir) ? x_c : x_1;

                ++cntr;
            }

            Console.WriteLine($"Количество итераций: {cntr}");

            return (x_1 + x_0) * 0.5;
        }

        public static Vector PerCoordDescend(FunctionND f, Vector x_start, double eps = 1e-5, int max_iters = 1000)
        {
            Vector x_0 = new Vector(x_start);
            Vector x_1 = new Vector(x_start);
            double step = 1.0, x_i, y_1, y_0;
            int opt_coord_n = 0, coord_id, i = 0;

            while (i < max_iters)
            {
                coord_id = i % x_0.Count;
                x_1[coord_id] -= eps;
                y_0 = f(x_1);
                x_1[coord_id] += 2 * eps;
                y_1 = f(x_1);
                x_1[coord_id] -= eps;
                x_1[coord_id] = y_0 > y_1 ? x_1[coord_id] + step : x_1[coord_id] - step;
                x_i = x_0[coord_id];
                x_1 = BiSect(f, x_0, x_1, eps, max_iters);
                x_0 = new Vector(x_1);

                if (Math.Abs(x_1[coord_id] - x_i) < eps)
                {
                    opt_coord_n++;

                    if (opt_coord_n == x_1.Count)
                    {
                        Console.WriteLine($"per coord descend iterations number : {i}");

                        return x_0;
                    }
                }
                else
                {
                    opt_coord_n = 0;
                }
                
                ++i;
            }

            Console.WriteLine($"Количество итераций: {max_iters}");

            return x_0;
        }
    }
}
