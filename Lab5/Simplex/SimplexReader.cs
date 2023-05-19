using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace OptimizationMethods
{
    public static class SimplexReader
    {
        private static double[] ReadArray(JObject node)
        {
            if (!node.ContainsKey("shape"))
                throw new Exception("shape is not defined");

            var shape = node["shape"].ToObject<int[]>();

            if (!node.ContainsKey("data"))
                throw new Exception("data is not defined");

            var data = node["data"].ToObject<double[]>();

            return data;
        }

        public static List<Simplex> ReadSimplexes(string pathToFile)
        {
            string json = File.ReadAllText(pathToFile);
            var raw_data = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);

            List<Simplex> simplexes = new List<Simplex>();

            if (raw_data.ContainsKey("problems"))
            {
                var problems = raw_data["problems"];
                foreach (var problem in problems)
                {
                    var simplex = ReadSimplex(problem);
                    simplexes.Add(simplex);
                }
            }
            else
            {
                var simplex = ReadSimplex(raw_data);
                simplexes.Add(simplex);
            }

            return simplexes;
        }

        private static Simplex ReadSimplex(dynamic jsonNode)
        {
            int solveType = jsonNode.ContainsKey("solve_type") ? Convert.ToInt32(jsonNode["solve_type"]) : 0;

            if (!jsonNode.ContainsKey("weights"))
                throw new Exception("weights is not defined");

            var weights = ReadArray(jsonNode["weights"]);

            if (!jsonNode.ContainsKey("bounds"))
                throw new Exception("bounds is not defined");

            var bounds = ReadArray(jsonNode["bounds"]);

            if (!jsonNode.ContainsKey("bounds_matrix"))
                throw new Exception("bounds_matrix is not defined");

            var boundsMatrix = ReadArray(jsonNode["bounds_matrix"]);
            int nRows = Convert.ToInt32(jsonNode["bounds_matrix"]["shape"][0]);
            int nCols = Convert.ToInt32(jsonNode["bounds_matrix"]["shape"][1]);
            double[,] boundsMatrix2D = new double[nRows, nCols];

            int index = 0;
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    boundsMatrix2D[i, j] = boundsMatrix[index];
                    index++;
                }
            }

            Vector[] rows = new Vector[boundsMatrix2D.GetLength(0)];
            for (int i = 0; i < boundsMatrix2D.GetLength(0); i++)
            {
                double[] rowData = new double[boundsMatrix2D.GetLength(1)];
                for (int j = 0; j < boundsMatrix2D.GetLength(1); j++)
                {
                    rowData[j] = boundsMatrix2D[i, j];
                }
                rows[i] = new Vector(rowData);
            }

            int[] inequalities;
            if (jsonNode.ContainsKey("inequalities"))
            {
                inequalities = ((JArray)jsonNode["inequalities"]).ToObject<int[]>();
            }
            else
            {
                inequalities = new int[boundsMatrix.GetLength(0)];
                for (int i = 0; i < inequalities.Length; i++)
                {
                    inequalities[i] = -1;
                }
            }

            List<Sign> signList = new List<Sign>();

            foreach (int value in inequalities)
            {
                Sign sign = (Sign)value;
                signList.Add(sign);
            }

            return new Simplex(
                new Matrix(rows),
                new Vector(weights),
                signList,
                new Vector(bounds)
            );
        }
    }
}
