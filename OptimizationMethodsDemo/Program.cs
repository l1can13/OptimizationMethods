using System;
using System.Numerics;

namespace OptimizationMethods
{
    class Program
    {
        public static void Main(string[] args)
        {
            DemoLabThree();
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
    }
}