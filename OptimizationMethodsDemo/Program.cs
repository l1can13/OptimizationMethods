using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OptimizationMethods
{
    class Program
    {
        public static void Main(string[] args)
        {
            DemoLabFive();
        }

        public static double Testf1(double arg)
        {
            return arg * (arg - 5);
        }

        static double Testf2(Vector args)
        {
            return (args[0] - 4) * (args[0] - 4) + (args[1] - 4) * (args[1] - 4);
        }

        static double Func(Vector args)
        {
            Vector row1 = new Vector(3, 1);
            Vector row2 = new Vector(-3, 4);
            Vector row3 = new Vector(4, -4);
            Vector row4 = new Vector(-4, -1);

            Matrix n = new Matrix(row1, row2, row3, row4);
            Vector b = new Vector(-10, 5, 8, 32);

            return Testf2(args) + LabFourImplementation.ExternalPenalty(args, n, b);
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

            Vector x_0 = new Vector(5, 5);
            Vector x_1 = new Vector(0, 0);

            Console.WriteLine($"x_0 = {x_0}, x_1 = {x_1}\n");
            Console.WriteLine($"BiSect           : {LabTwoImplementation.BiSect(Testf2, x_1, x_0)}");
            Console.WriteLine($"PerCoordDescend  : {LabTwoImplementation.PerCoordDescend(Testf2, x_1)}");
        }

        public static void DemoLabThree()
        {
            Console.WriteLine("\n////////////////////\n");
            Console.WriteLine("/// Lab. work #3 ///\n");
            Console.WriteLine("////////////////////\n\n");

            Vector x_1 = new Vector(0, 0);
            Vector x_0 = new Vector(5, 5);
            Console.WriteLine($"x_0 = {x_0}, x_1 = {x_1}\n");
            Console.WriteLine($"GradientDescend        : {LabThreeImplementation.GradientDescend(Testf2, x_1)}");
            Console.WriteLine($"СonjGradientDescend    : {LabThreeImplementation.СonjGradientDescend(Testf2, x_1)}");
        }

        public static void DemoLabFour()
        {
            Console.WriteLine("\n////////////////////\n");
            Console.WriteLine("/// Lab. work #4 ///\n");
            Console.WriteLine("////////////////////\n\n");

            Vector x_start = new Vector(-3, -4);
            Console.WriteLine($"x_start = {x_start}\n");
            Console.WriteLine($"NewtoneRaphson         : {LabFourImplementation.NewtonRaphson(Testf2, x_start)}");
            Console.WriteLine($"NewtoneRaphson penalty : {LabFourImplementation.NewtonRaphson(Func, x_start)}\n");
        }

        public static void DemoLabFive()
        {
            List<Simplex> simplexes = SimplexReader.ReadSimplexes("sm_task.json");

            simplexes[0].Solve();
        }
    }
}