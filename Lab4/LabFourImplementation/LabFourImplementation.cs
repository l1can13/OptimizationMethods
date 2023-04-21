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
    }
}
