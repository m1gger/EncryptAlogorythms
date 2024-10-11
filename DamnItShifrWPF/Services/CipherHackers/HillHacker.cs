using DamnItShifrWPF.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamnItShifrWPF.Services.CipherHackers
{
    public class HillHacker : IHacker
    {
        public IEncrypter Encrypter { get; set; }

        public HillHacker(IEncrypter encrypter)
        {
            Encrypter = encrypter;
        }

        public (string , string ) Hack(params string[] fragments)
        {
            // Предположим, что известные фрагменты одинаковой длины
            if (fragments.Length < 2)
                throw new ArgumentException("Необходимо минимум два фрагмента для взлома.");

            string knownFragment = fragments[0]; // Используем первый фрагмент как известный
            string encryptedFragment = fragments[1]; // Используем второй фрагмент как зашифрованный

            // Предполагаем, что длина фрагментов соответствует размеру матрицы
            int matrixSize = (int)Math.Sqrt(knownFragment.Length);

            var knownVector = ConvertToVector(knownFragment);
            var encryptedVector = ConvertToVector(encryptedFragment);

            // Создание матрицы из известного фрагмента
            var matrix = CreateMatrix(knownVector, encryptedVector, matrixSize);

            // Восстановление ключа
            var keyMatrix = InvertMatrix(matrix);

            // Дешифровка с использованием ключа
            var decryptedText = DecryptWithKey(keyMatrix, knownFragment.Length);

            // Преобразование ключа обратно в строку
            var keyString = ConvertMatrixToString(keyMatrix, matrixSize);

            return (keyString, decryptedText);
        }

        private int[,] CreateMatrix(int[] knownVector, int[] encryptedVector, int size)
        {
            var matrix = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix[i, j] = knownVector[i * size + j]; // Заполнение матрицы
                }
            }
            return matrix;
        }

        private int[,] InvertMatrix(int[,] matrix)
        {
            int size = matrix.GetLength(0);
            int[,] augmentedMatrix = new int[size, size * 2];

            // Создание расширенной матрицы
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    augmentedMatrix[i, j] = matrix[i, j];
                }
                augmentedMatrix[i, i + size] = 1; // Добавление единичной матрицы
            }

            // Применение метода Гаусса-Жордана для инверсии
            for (int i = 0; i < size; i++)
            {
                // Нормализация строки
                int divisor = augmentedMatrix[i, i];
                int divisorInverse = ModularInverse(divisor, 26); // Вычисляем обратный элемент
                if (divisorInverse == -1)
                {
                    throw new InvalidOperationException("Матрица не имеет обратной, так как определитель равен 0.");
                }

                for (int j = 0; j < size * 2; j++)
                {
                    augmentedMatrix[i, j] = (augmentedMatrix[i, j] * divisorInverse) % 26; // Нормализуем строку
                }

                // Обнуление остальных элементов в столбце
                for (int k = 0; k < size; k++)
                {
                    if (k != i)
                    {
                        int factor = augmentedMatrix[k, i];
                        for (int j = 0; j < size * 2; j++)
                        {
                            augmentedMatrix[k, j] = (augmentedMatrix[k, j] - factor * augmentedMatrix[i, j]) % 26;
                            if (augmentedMatrix[k, j] < 0) augmentedMatrix[k, j] += 26; // Обработка отрицательных значений
                        }
                    }
                }
            }

            // Извлечение инвертированной матрицы
            int[,] invertedMatrix = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    invertedMatrix[i, j] = augmentedMatrix[i, j + size]; // Берем правую половину
                }
            }

            return invertedMatrix;
        }

        // Функция для нахождения модульного обратного элемента
        private int ModularInverse(int a, int m)
        {
            a = a % m;
            for (int x = 1; x < m; x++)
            {
                if ((a * x) % m == 1)
                {
                    return x;
                }
            }
            return -1; // Обратного элемента не существует
        }


        private string DecryptWithKey(int[,] key, int length)
        {
            // Дешифровка текста с использованием ключа
            // Заглушка, просто возвращаем пустую строку
            return ""; // Замените на фактическую логику дешифровки
        }

        private int[] ConvertToVector(string fragment)
        {
            // Преобразование строки в вектор чисел (например, A=0, B=1, C=2, и т.д.)
            return fragment.Select(c => c - 'A').ToArray(); // Пример для английского алфавита
        }

        private string ConvertMatrixToString(int[,] matrix, int size)
        {
            var result = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    result.Append((char)(matrix[i, j] + 'A')); // Преобразуем обратно в символы
                }
            }
            return result.ToString();

            кто запретил тот гей 
        }

    }
}

