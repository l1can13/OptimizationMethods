using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace OptimizationMethods
{
    public enum Sign
    {
        Equal = 0,
        Less = 1,
        More = 2
    }

    public enum SimplexProblemType
    {
        Min = 0,
        Max = 1,
    }

    public class Simplex
    {
        ////////////////////
        /// Lab. work #5 ///
        ////////////////////
        /// <summary>
        /// список знаков в неравенств в системе ограничений
        /// </summary>
        private List<Sign> _inequalities;

        /// <summary>
        /// список индексов переменных которые войдут в целевую функию, модифицируя ее
        /// </summary>
        private List<int> _fModArgs;

        /// <summary>
        /// индексы естественных переменных
        /// </summary>
        private List<int> _naturalArgsIds;

        /// <summary>
        /// список индексов текущих базисных переменных 
        /// </summary>
        private List<int> _basisArgs;

        /// <summary>
        /// Симплекс таблица
        /// </summary>
        private Matrix _simplexTable;

        /// <summary>
        /// матрица ограничений
        /// </summary>
        private Matrix _boundsMatrix;

        /// <summary>
        /// вектор ограничений
        /// </summary>
        private Vector _boundsVector;

        /// <summary>
        /// вектор стоимостей
        /// </summary>
        private Vector _pricesVector;

        /// <summary>
        /// режим поиска решения
        /// </summary>
        private SimplexProblemType mode = SimplexProblemType.Max;

        public bool IsTargetFuncModified()
        {
            return _fModArgs.Count != 0;
        }

        /// <summary>
        /// Проверяет оптимальность текущего опорного плана. Исследуется положительность 
        /// симплекс-разностей в последней строке СТ в диапазоне от 1:n-1.
        /// Если целевая функция была модифицирована, то исследует две последних строки.
        /// Если среди элементов от 1:n-1 в последней строке нет отрицательных, то проверяет 
        /// на неотрицательность только те элементы предпоследней строки, которые не являются
        /// искусственными.
        /// </summary>
        /// <param name="A">СМ таблицa</param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public bool IsPlanOptimal()
        {
            Vector lastRow = _simplexTable[_simplexTable.NRows - 1];
            bool isOptimal = CheckRowPositivity(lastRow);

            if (IsTargetFuncModified() && !isOptimal)
            {
                Vector penultimateRow = _simplexTable[_simplexTable.NRows - 2];
                isOptimal = CheckRowPositivityForNaturalArgs(penultimateRow);
            }

            return isOptimal;
        }

        private bool CheckRowPositivity(Vector row)
        {
            return row.All(v => v >= 0);
        }

        private bool CheckRowPositivityForNaturalArgs(Vector row)
        {
            foreach (int id in _naturalArgsIds)
            {
                if (row[id] < 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Определяет ведущий столбец. Среди элементов строки симплекс-разностей ищет максимальны по модулю 
        /// отрицательный элемент. Если целевая функция была модифицирована и среди последней строки нет отрицательных
        /// элементов, то посик таковых будет продолжен среди только тех элементов предпоследней строки, которые не
        /// являются искусственными.
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        private int GetMainCol()
        {
            Vector lastRow = _simplexTable[_simplexTable.NRows - 1];
            double delta = 0;
            int index = -1;

            FindMaxNegativeElement(lastRow, ref delta, ref index);

            if (IsTargetFuncModified() && index == -1)
            {
                Vector penultimateRow = _simplexTable[_simplexTable.NRows - 2];
                FindMaxNegativeElementForNaturalArgs(penultimateRow, ref delta, ref index);
            }

            return index;
        }

        private void FindMaxNegativeElement(Vector row, ref double delta, ref int index)
        {
            for (int i = 0; i < row.Count - 1; i++)
            {
                if (row[i] < delta)
                {
                    delta = row[i];
                    index = i;
                }
            }
        }

        private void FindMaxNegativeElementForNaturalArgs(Vector row, ref double delta, ref int index)
        {
            foreach (int id in _naturalArgsIds)
            {
                if (row[id] < delta)
                {
                    delta = row[id];
                    index = id;
                }
            }
        }

        /// <summary>
        /// Определяет ведущую строку 
        /// </summary>
        /// <param name="simplex_col">ведущий столбец</param>
        /// <param name="A">СМ таблица</param>
        /// <returns></returns>
        private int GetMainRow(int simplexCol)
        {
            double delta = double.MaxValue;
            int index = -1;
            int bIndex = _simplexTable[0].Count - 1;

            int rowsCount = IsTargetFuncModified() 
                ? _simplexTable.NRows - 2 
                : _simplexTable.NRows - 1;

            for (int i = 0; i < rowsCount; i++)
            {
                double aIk = _simplexTable[i][simplexCol];

                if (aIk < 0) continue;
                if (_simplexTable[i][bIndex] / aIk > delta) continue;

                delta = _simplexTable[i][bIndex] / aIk;
                index = i;
            }

            return index;
        }


        /// <summary>
        /// строит виртуальный базисный вектор
        /// </summary>
        /// <param name="ineq_id"></param>
        /// <param name="_ineq"></param>
        /// <param name="col_index"></param>
        /// <param name="col_index_aditional"></param>
        private void BuildVirtualBasisCol(int ineqId, Sign inequality, ref int colIndex, ref int colIndexAdditional)
        {
            for (int row = 0; row < _simplexTable.NRows; row++)
            {
                if (row == ineqId)
                {
                    _simplexTable[row].PushBack(1.0);
                    continue;
                }
                _simplexTable[row].PushBack(0.0);
            }

            if (inequality == Sign.Equal)
            {
                colIndex = _simplexTable[0].Count - 1;
                colIndexAdditional = _simplexTable[0].Count - 1;
            }
            else if (inequality == Sign.More)
            {
                for (int row = 0; row < _simplexTable.NRows; row++)
                {
                    if (row == ineqId)
                    {
                        _simplexTable[row].PushBack(-1.0);
                        _simplexTable[row].PushBack(1.0);
                        continue;
                    }

                    _simplexTable[row].PushBack(0.0);
                    _simplexTable[row].PushBack(0.0);
                }

                colIndex = _simplexTable[0].Count - 2;
                colIndexAdditional = _simplexTable[0].Count - 1;
            }
            else
            {
                colIndex = _simplexTable[0].Count - 1;
                colIndexAdditional = -1;
            }
        }


        /// <summary>
        /// Строит СМ таблицу для задачи вида:
        /// Маирица системы ограниченй:
        ///		|u 0 0|	
        /// A = |0 v 0|
        ///		|0 0 w|
        /// Вектор ограничений
        ///		|a|	
        /// b = |d|
        ///		|f|
        /// с -коэффициенты целевой функции 
        /// f = (x,c)->extr
        ///	|u 0 0|   |x| <= |b|
        /// |0 v 0| * |x| >= |f|
        ///	|0 0 w|   |x| =  |d|
        /// 
        ///  СМ таблицу из A,b,c параметров
        /// </summary>
        /// <param name="A"> Ax <= b   -> (A|I)(x|w) = b </param>
        /// <param name="c"> (c,x) ->((-c|0),(x|w)) </param>
        /// <param name="ineq"> знак неравентсва =, >=, <= </param>
        /// <param name="b"></param>
        ///( A|I)  b
        ///(-c|0)  F(x,c)
        private void BuildSimplexTable()
        {
            _simplexTable = new Matrix(_boundsMatrix);
            _naturalArgsIds.Clear();
            _basisArgs.Clear();
            _fModArgs.Clear();

            // Изменение знаков и значений вектора b при отрицательных значениях
            for (int row = 0; row < _simplexTable.NRows; row++)
            {
                if (_boundsVector[row] < 0)
                {
                    _inequalities[row] = _inequalities[row] == Sign.Less ? Sign.More : Sign.Less;
                    _boundsVector[row] *= -1;
                    _simplexTable[row] = _simplexTable[row] * (-1.0);
                }
            }

            for (int i = 0; i < _pricesVector.Count; i++)
            {
                _naturalArgsIds.Add(i);
            }

            // Построение искусственного базиса
            int basisArgId = -1;
            int basisArgIdAdditional = -1;
            for (int ineqId = 0; ineqId < _inequalities.Count; ineqId++)
            {
                BuildVirtualBasisCol(ineqId, _inequalities[ineqId], ref basisArgId, ref basisArgIdAdditional);
                _naturalArgsIds.Add(basisArgId);

                if (basisArgIdAdditional != -1)
                {
                    _basisArgs.Add(basisArgIdAdditional);
                    _fModArgs.Add(basisArgIdAdditional);
                }
                else
                {
                    _basisArgs.Add(basisArgId);
                }
            }

            // Добавление столбца ограничений
            for (int row = 0; row < _simplexTable.NRows; row++)
            {
                _simplexTable[row].PushBack(_boundsVector[row]);
            }

            // Построение симплекс-разностей
            Vector sDeltas = new Vector(_simplexTable.NCols);

            if (mode == SimplexProblemType.Max)
            {
                for (int j = 0; j < _simplexTable.NCols; j++)
                {
                    sDeltas[j] = j < _pricesVector.Count ? -_pricesVector[j] : 0.0;
                }
            }
            else
            {
                for (int j = 0; j < _simplexTable.NCols; j++)
                {
                    sDeltas[j] = j < _pricesVector.Count ? _pricesVector[j] : 0.0;
                }
            }

            _simplexTable.AddRow(sDeltas);

            // Проверка модифицированности целевой функции
            if (!IsTargetFuncModified())
            {
                return;
            }

            // Добавление дополнительной строки для модифицированной целевой функции
            Vector sDeltasAdd = new Vector(_simplexTable.NCols);
            for (int j = 0; j < _simplexTable.NCols; j++)
            {
                sDeltasAdd[j] = 0.0;
            }
            foreach (int fModArgsId in _fModArgs)
            {
                sDeltasAdd[fModArgsId] = 1.0;
            }

            _simplexTable.AddRow(sDeltasAdd);
        }

        private bool ExcludeModArgs()
        {
            if (!IsTargetFuncModified())
            {
                return false;
            }

            int last_row_id = _simplexTable.NRows - 1;

            for (int i = 0; i < _fModArgs.Count; i++)
            {
                for (int row = 0; row < _simplexTable.NRows; row++)
                {
                    if (_simplexTable[row][_fModArgs[i]] != 0)
                    {
                        double arg = _simplexTable[last_row_id][_fModArgs[i]] / _simplexTable[row][_fModArgs[i]];

                        _simplexTable[last_row_id] = _simplexTable[last_row_id] - arg * _simplexTable[row];

                        break;
                    }
                }
            }

            return true;
        }

        private bool ValidateSolution()
        {
            double val = 0;

            int numRows = IsTargetFuncModified() ? _simplexTable.NRows - 2 : _simplexTable.NRows - 1;
            int numCols = _simplexTable.NCols - 1;

            for (int i = 0; i < _basisArgs.Count; i++)
            {
                if (_basisArgs[i] < NaturalArgsN())
                {
                    val += _simplexTable[i][numCols] * _pricesVector[_basisArgs[i]];
                }
            }

            double tolerance = 1e-5;

            if (mode == SimplexProblemType.Max)
            {
                if (Math.Abs(val - _simplexTable[numRows][numCols]) < tolerance)
                {
                    if (!IsTargetFuncModified())
                    {
                        return true;
                    }
                    return true & (Math.Abs(_simplexTable[_simplexTable.NRows - 1][_simplexTable.NCols - 1]) < tolerance);
                }
            }
            else
            {
                if (Math.Abs(val + _simplexTable[numRows][numCols]) < tolerance)
                {
                    if (!IsTargetFuncModified())
                    {
                        return true;
                    }
                    return true & (Math.Abs(_simplexTable[_simplexTable.NRows - 1][_simplexTable.NCols - 1]) < tolerance);
                }
            }

            return false;
        }

        public int NaturalArgsN()
        {
            return _pricesVector.Count;
        }

        public Matrix BoundsMatrix()
        {
            return _boundsMatrix;
        }

        public Vector BoundsCoeffs()
        {
            return _boundsVector;
        }

        public Vector PricesCoeffs()
        {
            return _pricesVector;
        }

        public List<Sign> Inequations()
        {
            return _inequalities;
        }

        public List<int> BasisArgsuments()
        {
            return _basisArgs;
        }

        public Matrix SimplexTable()
        {
            return _simplexTable;
        }

        public Vector CurrentSimplexSolution(bool only_natural_args = false)
        {
            int count = only_natural_args ? NaturalArgsN() : _simplexTable.NCols - 1;

            Vector solution = new Vector(count);

            for (int i = 0; i < _basisArgs.Count; i++)
            {
                if (_basisArgs[i] >= count) continue;

                solution[_basisArgs[i]] = _simplexTable[i][_simplexTable.NCols - 1];
            }
            return solution;
        }

        public string SimplexToString()//Matrix table, List<int> basis)
        {
            if (_simplexTable.NRows == 0) return "";

            StringBuilder sb = new StringBuilder();

            int i = 0;

            sb.AppendFormat("{0,-6}", " ");

            for (; i < _simplexTable.NCols - 1; i++)
            {
                sb.AppendFormat("|{0,-12}", " x " + (i + 1).ToString());
            }
            sb.AppendFormat("|{0,-12}", " b");

            sb.Append("\n");

            int n_row = -1;

            foreach (Vector row in _simplexTable.Rows)
            {
                n_row++;

                if (IsTargetFuncModified())
                {
                    if (n_row == _simplexTable.NRows - 2)
                    {
                        sb.AppendFormat("{0,-6}", " d0");
                    }
                    else if (n_row == _simplexTable.NRows - 1)
                    {
                        sb.AppendFormat("{0,-6}", " d1");
                    }
                    else
                    {
                        sb.AppendFormat("{0,-6}", " x " + (_basisArgs[n_row] + 1).ToString());
                    }
                }
                else
                {
                    if (n_row == _simplexTable.NRows - 1)
                    {
                        sb.AppendFormat("{0,-6}", " d");
                    }
                    else
                    {
                        sb.AppendFormat("{0,-6}", " x " + (_basisArgs[n_row] + 1).ToString());
                    }
                }

                for (int col = 0; col < row.Count; col++)
                {
                    if (row[col] >= 0)
                    {
                        sb.AppendFormat("|{0,-12}", " " + NumericUtils.ToRationalStr(row[col]));
                        continue;
                    }
                    sb.AppendFormat("|{0,-12}", NumericUtils.ToRationalStr(row[col]));

                }
                sb.Append("\n");
            }
            sb.Append("\n");

            return sb.ToString();
        }

        public Vector Solve(SimplexProblemType mode = SimplexProblemType.Max)
        {
            this.mode = mode;

            Console.WriteLine($"SimplexProblemType: {mode.ToString()}\n");

            Vector solution = new Vector(NaturalArgsN());

            BuildSimplexTable();

            Console.WriteLine("Start simplex table:");
            Console.WriteLine(SimplexToString());

            if (ExcludeModArgs())
            {
                Console.WriteLine("Simplex table after args exclusion:");
                Console.WriteLine(SimplexToString());
            }

            int mainRow;
            int mainCol;

            while (!IsPlanOptimal())
            {
                mainCol = GetMainCol();

                if (mainCol == -1)
                {
                    break;
                }

                mainRow = GetMainRow(mainCol);

                if (mainRow == -1)
                {
                    Console.WriteLine("Unable to get main row. Simplex is probably unbounded...");
                    return null;
                }

                _basisArgs[mainRow] = mainCol;

                double a_ik = _simplexTable[mainRow][mainCol];

                _simplexTable[mainRow] = _simplexTable[mainRow] * (1.0 / a_ik);

                for (int i = 0; i < _simplexTable.NRows; i++)
                {
                    if (i == mainRow)
                    {
                        continue;
                    }

                    _simplexTable[i] = _simplexTable[i] - _simplexTable[i][mainCol] * _simplexTable[mainRow];
                }

                solution = CurrentSimplexSolution();

#if DEBUG
                Console.WriteLine($"a_main {{{mainRow + 1}, {mainCol + 1}}} = {NumericUtils.ToRationalStr(a_ik)}\n");
                Console.WriteLine(SimplexToString());
                Console.WriteLine($"current solution: {NumericUtils.ToRationalStr(solution)}\n");
#endif
            }

            if (ValidateSolution())
            {
                solution = CurrentSimplexSolution(true);
                Console.WriteLine($"solution: {NumericUtils.ToRationalStr(solution)}\n");
                
                return solution;
            }

            Console.WriteLine("Simplex problem is infeasible");
            
            return null;
        }

        public Simplex() { }

        public Simplex(Matrix a, Vector c, List<Sign> ineq, Vector b)
        {
            if (b.Count != ineq.Count) throw new Exception("Error simplex creation :: b.size() != inequation.size()");
            if (a.NRows != ineq.Count) throw new Exception("Error simplex creation :: A.rows_number() != inequation.size()");
            if (a.NCols != c.Count) throw new Exception("Error simplex creation :: A.cols_number() != price_coeffs.size()");

            _naturalArgsIds = new List<int>();
            _basisArgs = new List<int>();
            _fModArgs = new List<int>();
            _boundsVector = b;
            _boundsMatrix = a;
            _pricesVector = c;
            _inequalities = ineq;
        }
        
        public Simplex(Matrix a, Vector c, Vector b)
        {
            if (b.Count != b.Count) throw new Exception("Error simplex creation :: b.size() != bouns_coeffs.size()");
            if (a.NCols != c.Count) throw new Exception("Error simplex creation :: A.cols_number() != price_coeffs.size()");

            _inequalities = new List<Sign>();
            for (int i = 0; i < b.Count; i++) _inequalities.Add(Sign.Less);
            _naturalArgsIds = new List<int>();
            _basisArgs = new List<int>();
            _fModArgs = new List<int>();
            _boundsVector = b;
            _boundsMatrix = a;
            _pricesVector = c;
        }
    }
}