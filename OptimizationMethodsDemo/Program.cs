﻿using System;
using System.Numerics;

namespace OptimizationMethods
{
    class Program
    {
        public static void Main(string[] args)
        {
            DemoLabFour();
        }

        public static double Testf1(double arg)
        {
            return arg * (arg - 5);
        }

        static double Testf2(Vector args)
        {
            return (args[0] - 2) * (args[0] - 2) + (args[1] - 2) * (args[1] - 2);
        }

        static double Psi1(Vector args)
        {
            return 1 / (5 - args[0] * 2 + args[1] * 3);
        }
        
        /// Уловие 1
        static double Psi2(Vector args)
        {
            return 1 / (6 + args[0] * 3 - args[1]);
        }

        // Ишем минимум функции  Testf2 при условии Psi1 и Psi2(Это внутренний штраф)
        static double Func(Vector args)
        {
            return Testf2(args) + Psi2(args) + Psi1(args);
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
            Console.WriteLine($"PerCoordDescend  : {LabTwoImplementation.PerCoordDescend(Testf2, x_1)}");
        }

        public static void DemoLabThree()
        {
            Console.WriteLine("\n////////////////////\n");
            Console.WriteLine("/// Lab. work #3 ///\n");
            Console.WriteLine("////////////////////\n\n");

            Vector x_1 = new double[] { 0, 0 };
            Vector x_0 = new double[] { 5, 5 };
            Console.WriteLine($"x_0 = {x_0}, x_1 = {x_1}\n");
            Console.WriteLine($"GradientDescend        : {LabThreeImplementation.GradientDescend(Testf2, x_1)}");
            Console.WriteLine($"СonjGradientDescend    : {LabThreeImplementation.СonjGradientDescend(Testf2, x_1)}");
        }

        public static void DemoLabFour()
        {
            Console.WriteLine("\n////////////////////\n");
            Console.WriteLine("/// Lab. work #4 ///\n");
            Console.WriteLine("////////////////////\n\n");

            Vector x_1 = new double[] { 0, 0 };
            Vector x_0 = new double[] { 5, 5 };
            Console.WriteLine($"x_0 = {x_0}, x_1 = {x_1}\n");
            Console.WriteLine($"NewtoneRaphson         : {LabFourImplementation.NewtoneRaphson(Testf2, x_1)}");
            Console.WriteLine($"NewtoneRaphson         : {LabFourImplementation.NewtoneRaphson(Func, x_1)}\n");
            Console.WriteLine($"STRELKOV NewtoneRaphson: {NewtoneRaphson(Testf2, x_1)}");
            Console.WriteLine($"STRELKOV NewtoneRaphson: {NewtoneRaphson(Func, x_1)}\n");
        }

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