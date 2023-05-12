using System;

namespace OptimizationMethods
{
    public class LabThreeImplementation
    {
        /// <summary>
        /// Метод градиентного спуска для поиска минимума функции многих переменных.
        /// </summary>
        /// <param name="f">Функция, для которой ищется минимум.</param>
        /// <param name="x_start">Начальная точка поиска.</param>
        /// <param name="eps">Пороговое значение для градиента.</param>
        /// <param name="max_iters">Максимальное количество итераций.</param>
        /// <returns>Точка минимума функции.</returns>
        public static Vector GradientDescend(FunctionND f, Vector x_start, double eps = 1e-5, int max_iters = 1000)
        {
            Vector x_i = new Vector(x_start);
            int cntr = 0;

            while (cntr <= max_iters && Vector.Gradient(f, x_i, eps).Magnitude >= eps)
            {
                x_i = LabTwoImplementation.BiSect(
                    f, 
                    x_i - Vector.Gradient(f, x_i, eps), 
                    x_i, eps, 
                    max_iters
                );

                ++cntr;
            }

            return x_i;
        }

        /// <summary>
        /// Метод сопряженного градиентного спуска для нахождения минимума функции многих переменных.
        /// </summary>
        /// <param name="f">Целевая функция.</param>
        /// <param name="x_start">Начальное приближение.</param>
        /// <param name="eps">Точность вычислений.</param>
        /// <param name="max_iters">Максимальное количество итераций.</param>
        /// <returns>Вектор, являющийся аргументом минимума функции.</returns>
        public static Vector СonjGradientDescend(FunctionND f, Vector x_start, double eps = 1e-5, int max_iters = 1000)
        {
            Vector x_i = new Vector(x_start);
            Vector s_i = Vector.Gradient(f, x_start, eps) * (-1.0);

            int cntr = 0;
            while (cntr <= max_iters && s_i.Magnitude >= eps)
            {
                Vector x_i_1 = x_i + s_i;

                x_i_1 = LabTwoImplementation.BiSect(
                    f,
                    x_i, 
                    x_i_1, 
                    eps,
                    max_iters
                );

                Vector s_i_1 = Vector.Gradient(f, x_i_1, eps);
                double omega = Math.Pow(s_i_1.Magnitude, 2) / Math.Pow(s_i.Magnitude, 2);
                
                s_i = s_i * omega - s_i_1;
                x_i = x_i_1;

                ++cntr;
            }

            return x_i;
        }
    }
}
