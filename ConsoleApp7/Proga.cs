using ConsoleApp1;
using ConsoleApp7;
using System.IO;
using System.Runtime.InteropServices.ObjectiveC;


class Program
{
    async static Task Main(string[] args)
    {

        Matrix[] a = new Matrix[50];
        for (int i = 0; i < 50; i++)
        {
            a[i] = RandomMatrix(500, 100);
        }

        Matrix[] b = new Matrix[50];
        for (int i = 0; i < 50; i++)
        {
            b[i] = RandomMatrix(100, 500);
        }

        if (Directory.Exists("C:\\Users\\User\\source\\repos\\ConsoleApp7\\matrices"))
        {
            Directory.Delete("C:\\Users\\User\\source\\repos\\ConsoleApp7\\matrices", recursive: true);
        }
        Directory.CreateDirectory("C:\\Users\\User\\source\\repos\\ConsoleApp7\\matrices");

        Matrix[] matrices1 = new Matrix[50];
        Matrix[] matrices2 = new Matrix[50];
        Parallel.For(0, 50, i => matrices1[i] = OrderMultiplyMatrix(a[i], MatrixOperations.Transpose(b[i])));
        Parallel.For(0, 50, i => matrices2[i] = OrderMultiplyMatrix(b[i], MatrixOperations.Transpose(a[i])));

        await WriteMatrixInArrayAsync(matrices1, "C:\\Users\\User\\source\\repos\\ConsoleApp7\\matrices", "matrices", ".tsv");

        await WriteMatrixInArrayAsync(matrices2, "C:\\Users\\User\\source\\repos\\ConsoleApp7\\matrices", "matrices_two", ".tsv");

        Matrix[] m1 = ScalarMultiplyOfMatrixArray(a, b.Select(i => MatrixOperations.Transpose(i)).ToArray());
        Matrix[] m2 = ScalarMultiplyOfMatrixArray(b, a.Select(i => MatrixOperations.Transpose(i)).ToArray());

        Directory.CreateDirectory("C:\\Users\\User\\source\\repos\\ConsoleApp7\\matrices\\Binary");

        Directory.CreateDirectory("C:\\Users\\User\\source\\repos\\ConsoleApp7\\matrices\\Stroke");

        Directory.CreateDirectory("C:\\Users\\User\\source\\repos\\ConsoleApp7\\matrices\\Json");

        await Task.WhenAll(
            WriteMatrixInArrayAsync(m1, "C:\\Users\\User\\source\\repos\\ConsoleApp7\\matrices\\Binary", "binary", ".bin"),
            WriteMatrixInArrayAsync(m2, "C:\\Users\\User\\source\\repos\\ConsoleApp7\\matrices\\Stroke", "stroke", ".csv"),
            WriteMatrixInArrayAsync(m2, "C:\\Users\\User\\source\\repos\\ConsoleApp7\\matrices\\Json", "json", ".json")
        );

        Console.WriteLine("Запись завершена.");

        Matrix[] matrices3 = await ReadMatrixInArrayAsync("C:\\Users\\User\\source\\repos\\ConsoleApp7\\matrices\\Binary", "binary", ".bin");
        Matrix[] matrices4 = await ReadMatrixInArrayAsync("C:\\Users\\User\\source\\repos\\ConsoleApp7\\matrices\\Stroke", "stroke", ".csv");
        Matrix[] matrices5 = await ReadMatrixInArrayAsync("C:\\Users\\User\\source\\repos\\ConsoleApp7\\matrices\\Json", "json", ".json");

        Console.WriteLine("Чтение завершено. Тип чтения: " + (matrices4.Length > 0 ? "text" : "json"));

        await Task.Run(() =>
        {
            bool isEqual = EqualsMatrixArrays(matrices1, matrices3);
            Console.WriteLine("Результат сравнения: " + isEqual);
        });

        await Task.WhenAll(
            Task.Run(() =>
            {
                bool isEqual = EqualsMatrixArrays(matrices2, matrices4);
                Console.WriteLine("Результат сравнения: " + isEqual);
            }),
            Task.Run(() =>
            {
                bool isEqual = EqualsMatrixArrays(matrices2, matrices5);
                Console.WriteLine("Результат сравнения: " + isEqual);
            })
        );

        Console.WriteLine("Сравнение завершено.");

    }

    public static Matrix RandomMatrix(int rows, int cols)
    {
        Random rnd = new Random();

        double[,] mat = new double[rows, cols];

        for (int i = 0; i < mat.GetLength(0); i++)
        {
            for (int j = 0; j < mat.GetLength(1); j++)
            {
                mat[i, j] = (rnd.NextDouble() * 20) - 10;
            }
        }

        return new Matrix(mat);
    }

    public static Matrix OrderMultiplyMatrix(Matrix m1, Matrix m2)
    {
        double[,] mat = m1.GetMatrix();
        double[,] mat2 = m2.GetMatrix();
        if (mat.GetLength(0) != mat2.GetLength(0) || mat.GetLength(1) != mat2.GetLength(1))
        {
            throw new Exception();
        }

        for (int i = 0; i < mat.GetLength(0); i++)
        {
            for (int j = 0; j < mat.GetLength(1); j++)
            {
                mat[i, j] *= mat2[i, j];
            }
        }

        return new Matrix(mat);
    }

    public static Matrix[] ScalarMultiplyOfMatrixArray(Matrix[] a, Matrix[] b)
    {
        if (a.Length != b.Length)
        {
            throw new Exception();
        }
        Matrix[] c = new Matrix[a.Length];
        for (int i = 0; i < a.Length; i++)
        {
            c[i] = MatrixOperations.DotMatrix(a[i], b[i]);
        }

        return c;
    }

    public async static Task WriteMatrixInArrayAsync(Matrix[] matrices, string directory, string prefix, string ext)
    {
        for (int i = 0; i < matrices.Length; i++)
        {
            await MatrixIO.WriteFileAsync(directory, prefix + $"{i}" + ext, matrices[i]);
        }
    }

    public async static Task<Matrix[]> ReadMatrixInArrayAsync(string directory, string prefix, string ext)
    {
        Matrix[] m = new Matrix[Directory.GetFiles(directory, "*" + ext, SearchOption.AllDirectories).Length];
        for (int i = 0; i < m.Length; i++)
        {
            m[i] = await MatrixIO.ReadFileAsync(directory, prefix + $"{i}" + ext);
        }
        return m;
    }

    public static bool EqualsMatrixArrays(Matrix[] a, Matrix[] b)
    {
        if (a.Length != b.Length)
        {
            throw new Exception();
        }
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i].Equals(b[i]))
            {
                continue;
            }
            return false;
        }
        return true;
    }
}

public class Matrix
{
    public int r;
    public int c;
    private double[,] matrix;

    public double[,] GetMatrix() { return matrix; }

    public Matrix(double[,] matrix)
    {
        r = matrix.GetLength(0);
        c = matrix.GetLength(1);
        this.matrix = matrix;
    }
    public static Matrix Zero(int n)
    {
        double[,] matrix = new double[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                matrix[i, j] = 0;
            }
        }
        return new Matrix(matrix);
    }
    public static Matrix Zero(int r, int c)
    {
        double[,] matrix = new double[r, c];
        for (int i = 0; i < r; i++)
        {
            for (int j = 0; j < c; j++)
            {
                matrix[i, j] = 0;
            }
        }
        return new Matrix(matrix);
    }
    public static Matrix Identity(int n)
    {
        double[,] matrix = new double[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                matrix[i, j] = 0;
                if (i == j)
                {
                    matrix[i, j] = 1;
                }
            }
        }
        return new Matrix(matrix);
    }

    public override string ToString()
    {
        string stri = "";
        for (int i = 0; i < c; i++)
        {
            string li = "";
            for (int j = 0; j < r; j++)
            {
                li += $"{matrix[j, i]}" + ", ";
            }
            stri += li + "\n";
        }
        return stri;
    }

    public override bool Equals(object? obj)
    {
        Matrix b = obj as Matrix;
        if (b == null)
        {
            return false;
        }
        if (b.c == c & b.r == r)
        {
            bool equal = true;

            for (int i = 0; i < r * c; i++)
            {
                if (b.matrix[i % c, i % r] != matrix[i % c, i % r])
                {
                    equal = false;
                    break;
                }
            }
            return equal;

        }
        return false;
    }


}
