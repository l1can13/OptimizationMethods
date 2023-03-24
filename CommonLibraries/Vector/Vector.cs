﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace OptimizationMethods
{
    public delegate double FunctionND(Vector x);
    public class Vector : IEquatable<Vector>
    {
        /// <summary>
        /// заполнение массива
        /// </summary>
        private int fillness = 0;
        /// <summary>
        /// Массив элементов
        /// </summary>
        private double[] data;
        /// <summary>
        /// Размерность вектора
        /// </summary>
        public int Count
        {
            get => fillness;
        }
        /// <summary>
        /// Длина вектра
        /// </summary>
        public double Magnitude
        {
            get
            {
                double mag = 0.0;
                foreach (double element in data) mag += (element * element);
                return Math.Sqrt(mag);
            }
        }
        /// <summary>
        /// Нормированый вариант вектора
        /// </summary>
        public Vector Normalized
        {
            get
            {
                Vector v = new Vector(this);
                double inv_mag = 1.0 / v.Magnitude;
                for (int i = 0; i < v.Count; i++) v[i] *= inv_mag;
                return v;
            }
        }
        /// <summary>
        /// Нормализация вектора
        /// </summary>
        /// <returns></returns>
        public Vector Normalize()
        {
            double inv_mag = 1.0 / Magnitude;
            for (int i = 0; i < Count; i++) this[i] *= inv_mag;
            return this;
        }
        /// <summary>
        /// Скалярное произведение (this;other)
        /// </summary>
        /// <param name="other"></param>
        /// <returns>(this;other)</returns>
        public double Dot(Vector other)
        {
            if (Count != other.Count) throw new Exception("Unable vector dot multiply");
            double dot = 0.0;
            for (int i = 0; i < other.Count; i++) dot += data[i] * other[i];
            return dot;
        }

        public void PushBack(double val)
        {
            if (fillness != data.Length)
            {
                data[fillness] = val;
                fillness++;
                return;
            }

            double[] new_data = new double[(int)(data.Length * 1.5)];
            for (int i = 0; i < Count; i++) new_data[i] = data[i];
            new_data[fillness] = val;
            fillness++;
            data = new_data;
        }
        /// <summary>
        /// Строковое представление вектора
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{ ");
            for (int i = 0; i < Count - 1; i++)
            {
                sb.Append(string.Format("{0,0}, ", String.Format("{0:0.000}", data[i])));
            }
            sb.Append(string.Format("{0,0}", String.Format("{0:0.000}", data[Count - 1])));
            sb.Append(" }");
            return sb.ToString();
        }
        /// <summary>
        /// Базовое сравнение двух векторов
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Vector)) return false;
            return Equals(obj as Vector);
        }
        /// <summary>
        /// Сравнение двух векторов
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals([AllowNull] Vector other)
        {
            if (other.Count != Count) return false;
            for (int i = 0; i < other.Count; i++) if (other[i] != this[i]) return false;
            return true;
        }
        /// <summary>
        /// Хешкод вектора
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => HashCode.Combine(data);
        /// <summary>
        /// Получение элемента вктора
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public double this[int id]
        {
            get => data[id];
            set => data[id] = value;
        }
        /// <summary>
        /// Конструктор вектора из массива
        /// </summary>
        /// <param name="_data"></param>
        public Vector(params double[] _data)
        {
            fillness = _data.Length;
            data = new double[(int)(fillness * 1.5)];
            for (int i = 0; i < fillness; i++) data[i] = _data[i];
        }
        /// <summary>
        /// Конструктор вектора по размеру и элементу по умолчанию
        /// </summary>
        /// <param name="size"></param>
        /// <param name="defaultValue"></param>
        public Vector(int size)// double defaultValue = 0.0)
        {
            fillness = size;
            data = new double[(int)(size * 1.5)];
            for (int i = 0; i < size; i++) data[i] = (0.0);
        }
        /// <summary>
        /// Конструктор копирования
        /// </summary>
        /// <param name="vect"></param>
        public Vector(Vector vect)
        {
            fillness = vect.fillness;
            data = new double[vect.data.Length];
            for (int i = 0; i < vect.fillness; i++) data[i] = vect.data[i];
        }

        /// <summary>
        /// Элементарные математические операции над векторами
        /// </summary>
        public static Vector operator +(Vector a, Vector b)
        {
            if (a.Count != b.Count) throw new Exception("error:: operator+:: vectors of different dimensions");
            Vector res = new Vector(a);
            for (int i = 0; i < a.Count; i++) res[i] = a[i] + b[i];
            return res;
        }
        public static Vector operator +(Vector a, double b)
        {
            Vector res = new Vector(a);
            for (int i = 0; i < a.Count; i++) res[i] = a[i] + b;
            return res;
        }
        public static Vector operator +(double b, Vector a)
        {
            return a + b;
        }
        public static Vector operator -(Vector a, Vector b)
        {
            if (a.Count != b.Count) throw new Exception("error:: operator-:: vectors of different dimensions");
            Vector res = new Vector(a);
            for (int i = 0; i < a.Count; i++) res[i] = a[i] - b[i];
            return res;
        }
        public static Vector operator -(Vector a, double b)
        {
            Vector res = new Vector(a);
            for (int i = 0; i < a.Count; i++) res[i] = a[i] - b;
            return res;
        }
        public static Vector operator -(double b, Vector a)
        {
            Vector res = new Vector(a);
            for (int i = 0; i < a.Count; i++) res[i] = b - a[i];
            return res;
        }
        public static Vector operator *(Vector a, double val)
        {
            Vector res = new Vector(a);
            for (int i = 0; i < a.Count; i++) res[i] = a[i] * val;
            return res;
        }
        public static Vector operator *(double val, Vector a)
        {
            return a * val;
        }
        /// <summary>
        /// Позволяет при иницилизации экземпляра класса вместо:
        /// double [] vals = new double[] {1,2,3};
        /// Vector v = new Vector(rows);
        /// делать так:
        /// Vector v = vals;
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Vector(double[] value) => new Vector(value);
        /// <summary>
        /// Рассчитывет единичный вектор в направлении от a до b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector Direction(Vector a, Vector b)
        {
            if (a.Count != b.Count) return a;
            return (b - a).Normalized;
        }
        /// <summary>
        /// Градиент скалярной функции векторного аргумента 
        /// </summary>
        /// <param name="func">функция для которой рассчитываем градиент</param>
        /// <param name="x">   точка, где рассчитываем градиент</param>
        /// <param name="eps"> шаг центрального разностного аналога</param>
        /// <returns></returns>
        public static Vector Gradient(FunctionND func, Vector x, double eps = 1e-6)
        {
            Vector df = new Vector(x.Count);

            for (int i = 0; i < x.Count; i++) df[i] = Partial(func, x, i, eps);

            return df;
        }
        /// <summary>
        /// Частная производная в точке x вдоль координаты coord_index
        /// </summary>
        /// <param name="func"></param>
        /// <param name="x"></param>
        /// <param name="coord"></param>
        /// <param name="eps"></param>
        /// <returns></returns>
        public static double Partial(FunctionND func, Vector x, int coord_index, double eps = 1e-6)
        {
            if (x.Count <= coord_index) throw new Exception("Partial derivative index out of bounds!");
            x[coord_index] += eps;
            double f_r = func(x);
            x[coord_index] -= (2.0 * eps);
            double f_l = func(x);
            x[coord_index] += eps;
            return (f_r - f_l) / eps * 0.5;
        }

        public static double Partial2(FunctionND func, Vector x, int coord_index_1, int coord_index_2, double eps = 1e-6)
        {
            if (x.Count <= coord_index_2) throw new Exception("Partial derivative index out of bounds!");
            x[coord_index_2] -= eps;
            double f_l = Partial(func, x, coord_index_1, eps);
            x[coord_index_2] += (2 * eps);
            double f_r = Partial(func, x, coord_index_1, eps);
            x[coord_index_2] -= eps;
            return (f_r - f_l) / eps * 0.5;
        }
        /// <summary>
        /// Скалярное произведение (a;b)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>(a;b)</returns>
        public static double Dot(Vector a, Vector b) => a.Dot(b);
    }
}