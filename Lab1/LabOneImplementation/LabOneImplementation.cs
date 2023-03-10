using System;

namespace OptimizationMethods
{
    /// <summary>
    /// Реализация лабораторной работы №1.
    /// </summary>
    public class LabOneImplementation
    {
        /// <summary>
        /// Делегат-функция f(x).
        /// </summary>
        /// <param name="x">Аргумент функции.</param>
        /// <returns>Значение функции.</returns>
        public delegate double Function1D(double x);

        /// <summary>
        /// Константа phi.
        /// </summary>
        private static readonly double Phi = 1.61803398874989484820;

        /// <summary>
        /// Обмен значениями.
        /// </summary>
        /// <typeparam name="T">Тип заданного значения.</typeparam>
        /// <param name="lhs">Ссылка.</param>
        /// <param name="rhs">Ссылка.</param>
        private static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        /// <summary>
        /// Бисекция.
        /// Численное решение уравнение f(x) = 0 методом бискции.
        /// Отрезок [x_0, x_1].
        /// </summary>
        /// <param name="f">Уравнение.</param>
        /// <param name="x_0">Левая граница отрезка.</param>
        /// <param name="x_1">Правая граница отрезка.</param>
        /// <param name="eps">Точность.</param>
        /// <param name="max_iters">Максимальное количество итераций.</param>
        /// <returns></returns>
        public static double BiSect(Function1D f, double x_0, double x_1, double eps = 1e-6, int max_iters = 1000)
        {
            double x_c = 0.0;

            if (x_0 > x_1)
            {
                Swap(ref x_0, ref x_1);
            }

            int cntr = 0;

            while (true)
            {
                x_c = (x_0 + x_1) * 0.5;

                if (Math.Abs(x_1 - x_0) < eps)
                {
                    return x_c;
                }

                if (f(x_c + eps) > f(x_c - eps))
                {
                    x_1 = x_c;
                }
                else
                {
                    x_0 = x_c;
                }

                ++cntr;
                if (cntr == max_iters)
                {
                    return x_c;
                }
            }
        }

        /// <summary>
        /// Алгоритм оптимизации одномерной функции методом золотого сечения.
        /// </summary>
        /// <param name="f">Функция.</param>
        /// <param name="x_0">Левая граница отрезка.</param>
        /// <param name="x_1">Правая граница отрезка.</param>
        /// <param name="eps">Точность.</param>
        /// <param name="max_iters">Максимальное количество итераций.</param>
        /// <returns>Точка минимума функции.</returns>
        public static double GoldenRatio(Function1D f, double x_0, double x_1, double eps = 1e-6, int max_iters = 1000)
        {
            if (x_0 > x_1) Swap(ref x_0, ref x_1);

            double a = x_0, b = x_1;
            double c = b - (b - a) / Phi;
            double d = a + (b - a) / Phi;

            int cntr = 0;

            while (Math.Abs(c - d) > eps && cntr < max_iters)
            {
                if (f(c) < f(d))
                {
                    x_1 = d;
                }
                else
                {
                    x_0 = c;
                }

                c = x_1 - (x_1 - x_0) / Phi;
                d = x_0 + (x_1 - x_0) / Phi;

                ++cntr;
            }

            return (x_0 + x_1) * 0.5;
        }

        /// <summary>
        /// Вычисление всех чисел Фибоначчи от 0 до заданного индекса.
        /// </summary>
        /// <param name="n">Индекс предельного числа фибоначчи.</param>
        /// <returns>Массив int[] чисел фибоначчи до заданного индекса.</returns>
        private static int[] FibonacciNumbers(int index)
        {
            if (index <= 0)
            {
                return new int[0];
            }

            int[] numbers = new int[index];

            numbers[0] = 0;

            if (index > 1)
            {
                numbers[1] = 1;

                for (int i = 2; i < index; i++)
                {
                    numbers[i] = numbers[i - 1] + numbers[i - 2];
                }
            }

            return numbers;
        }

        /// <summary>
        /// Алгоритм поиска ближайшей пары чисел Фибоначчи к заданному значению.
        /// </summary>
        /// <param name="value">Значение.</param>
        /// <param name="f_n">Число фибоначчи.</param>
        /// <param name="f_n_1">Число фибоначчи.</param>
        private static void ClosestFibonacciPair(double value, ref int f_n, ref int f_n_1)
        {
            if (value < 1)
            {
                return;
            }

            f_n_1 = 1;

            int f_tmp;
            while (f_n < value && f_n <= int.MaxValue - f_n_1)
            {
                f_tmp = f_n;
                f_n = f_n_1;
                f_n_1 += f_tmp;
            }
        }

        /// <summary>
        /// Метод оптимизации функции одной переменной с помощью чисел Фибоначчи.
        /// </summary>
        /// <param name="f">Функция одной переменной.</param>
        /// <param name="x0">Левая граница отрезка.</param>
        /// <param name="x1">Правая граница отрезка.</param>
        /// <param name="eps">Точность.</param>
        /// <returns>Приближение точки минимума функции.</returns>
        public static double Fibonacci(Function1D f, double x0, double x1, double eps = 1e-6)
        {
            if (x0 > x1)
            {
                Swap(ref x0, ref x1);
            }

            double a = x0, b = x1, dx;

            int f_n = 0, f_n_1 = 0, f_tmp, cntr = 0;

            ClosestFibonacciPair((b - a) / eps, ref f_n, ref f_n_1);

            while (f_n != f_n_1)
            {
                if (x1 - x0 < eps)
                {
                    break;
                }

                ++cntr;

                dx = (b - a);
                f_tmp = f_n_1 - f_n;

                x0 = a + dx * ((double)f_tmp / f_n_1);
                x1 = a + dx * ((double)f_n / f_n_1);

                f_n_1 = f_n;
                f_n = f_tmp;

                if (f(x0) < f(x1))
                {
                    b = x1;
                }
                else
                {
                    a = x0;
                }
            }

            return (x1 + x0) * 0.5;
        }
    }
}
