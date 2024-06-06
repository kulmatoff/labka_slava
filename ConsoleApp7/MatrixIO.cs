using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Transactions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConsoleApp7
{
    internal class MatrixIO
    {
        public static Matrix ReadFile(string directory, string filename)
        {
            return ReadFromFile(directory, filename, stream =>
            {
                using (var reader = new BinaryReader(stream))
                {
                    int rows = reader.ReadInt32();
                    int cols = reader.ReadInt32();
                    var matrix = new double[rows, cols];
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            matrix[i, j] = reader.ReadDouble();
                        }
                    }
                    return new Matrix(matrix);
                }
            });
        }

        public static async Task<Matrix> ReadFileAsync(string directory, string filename)
        {
            return await ReadFromFileAsync(directory, filename, async stream =>
            {
                using (var reader = new BinaryReader(stream))
                {
                    int rows = reader.ReadInt32();
                    int cols = reader.ReadInt32();
                    var matrix = new double[rows, cols];
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            matrix[i, j] = reader.ReadDouble();
                        }
                    }
                    return await Task.FromResult(new Matrix(matrix));
                }
            });
        }

        public static void WriteFile(string directory, string filename, Matrix m)
        {
            WriteToFile(directory, filename, stream =>
            {
                using (var writer = new BinaryWriter(stream))
                {
                    double[,] matrix = m.GetMatrix();
                    int rows = matrix.GetLength(0);
                    int cols = matrix.GetLength(1);
                    writer.Write(rows);
                    writer.Write(cols);
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            writer.Write(matrix[i, j]);
                        }
                    }
                }
            });
        }

        public static async Task WriteFileAsync(string directory, string filename, Matrix m)
        {
            await WriteToFileAsync(directory, filename, async stream =>
            {
                using (var writer = new BinaryWriter(stream))
                {
                    double[,] matrix = m.GetMatrix();
                    int rows = matrix.GetLength(0);
                    int cols = matrix.GetLength(1);
                    writer.Write(rows);
                    writer.Write(cols);
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            writer.Write(matrix[i, j]);
                        }
                    }
                    await Task.CompletedTask;
                }
            });
        }

        public static Matrix ReadJson(string directory, string filename)
        {
            var json = File.ReadAllText(Path.Combine(directory, filename));
            return new Matrix(JsonSerializer.Deserialize<double[,]>(json));
        }

        public static async Task<Matrix> ReadJsonAsync(string directory, string filename)
        {
            var json = await File.ReadAllTextAsync(Path.Combine(directory, filename));
            return new Matrix(JsonSerializer.Deserialize<double[,]>(json));
        }

        public static void WriteJson(string directory, string filename, Matrix m)
        {
            double[,] matrix = m.GetMatrix();
            var json = JsonSerializer.Serialize(matrix);
            File.WriteAllText(Path.Combine(directory, filename), json);
        }

        public static async Task WriteJsonAsync(string directory, string filename, Matrix m)
        {
            double[,] matrix = m.GetMatrix();
            var json = JsonSerializer.Serialize(matrix);
            await File.WriteAllTextAsync(Path.Combine(directory, filename), json);
        }

        private static Matrix ReadFromFile(string directory, string filename, Func<Stream, Matrix> readFunc)
        {
            using (var stream = new FileStream(Path.Combine(directory, filename), FileMode.Open, FileAccess.Read))
            {
                return readFunc(stream);
            }
        }

        private static async Task<Matrix> ReadFromFileAsync(string directory, string filename, Func<Stream, Task<Matrix>> readFunc)
        {
            using (var stream = new FileStream(Path.Combine(directory, filename), FileMode.Open, FileAccess.Read))
            {
                return await readFunc(stream);
            }
        }

        private static void WriteToFile(string directory, string filename, Action<Stream> writeAction)
        {
            using (var stream = new FileStream(Path.Combine(directory, filename), FileMode.Create, FileAccess.Write))
            {
                writeAction(stream);
            }
        }

        private static async Task WriteToFileAsync(string directory, string filename, Func<Stream, Task> writeFunc)
        {
            using (var stream = new FileStream(Path.Combine(directory, filename), FileMode.Create, FileAccess.Write))
            {
                await writeFunc(stream);
            }
        }
    }

}
