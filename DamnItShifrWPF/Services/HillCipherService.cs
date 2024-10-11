using DamnItShifrWPF.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DamnItShifrWPF.Services
{
    public class HillCipherService : IEncrypter
    {
        public string Text { get; set; }
        public int[,] KeyMatrix { get; private set; }
        public string Alphabet { get; set; } = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        public string EncryptedText { get;  set; }

        public HillCipherService(string text, int matrixSize)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Текст не может быть пустым");
            }

            Text = text.ToLower();
            KeyMatrix = GenerateKeyMatrix(matrixSize);
        }

        private int[,] GenerateKeyMatrix(int size)
        {
            Random rnd = new Random();
            int[,] matrix = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix[i, j] = rnd.Next(Alphabet.Length);
                }
            }
            return matrix;
        }

        public string Encrypt()
        {
            StringBuilder encryptedText = new StringBuilder();
            List<int> textVector = TextToVector(Text, KeyMatrix.GetLength(0));

            for (int i = 0; i < textVector.Count; i += KeyMatrix.GetLength(0))
            {
                var subVector = textVector.Skip(i).Take(KeyMatrix.GetLength(0)).ToArray();
                var encryptedSubVector = MultiplyMatrixAndVector(KeyMatrix, subVector);
                encryptedText.Append(VectorToText(encryptedSubVector.ToList()));
            }

            EncryptedText = encryptedText.ToString();
            return EncryptedText;
        }

        public string Decrypt()
        {
            StringBuilder decryptedText = new StringBuilder();
            int matrixSize = KeyMatrix.GetLength(0);
            List<int> encryptedVector = TextToVector(EncryptedText, matrixSize);
            int[,] inverseMatrix = InverseMatrix(KeyMatrix, Alphabet.Length);

            for (int i = 0; i < encryptedVector.Count; i += matrixSize)
            {
                var subVector = encryptedVector.Skip(i).Take(matrixSize).ToArray();
                var decryptedSubVector = MultiplyMatrixAndVector(inverseMatrix, subVector);
                decryptedText.Append(VectorToText(decryptedSubVector.ToList()));
            }

            return decryptedText.ToString();
        }

        public (string, string) Hack(string knownPlainText = null)
        {
            if (knownPlainText.Length < KeyMatrix.GetLength(0))
            {
                throw new ArgumentException("Известный текст слишком короткий для взлома.");
            }

            int matrixSize = KeyMatrix.GetLength(0);
            List<int> plainTextVector = TextToVector(knownPlainText.Substring(0, matrixSize), matrixSize);
            List<int> encryptedTextVector = TextToVector(EncryptedText.Substring(0, matrixSize), matrixSize);

            int[,] plainTextMatrix = VectorToMatrix(plainTextVector.ToArray(), matrixSize);
            int[,] encryptedTextMatrix = VectorToMatrix(encryptedTextVector.ToArray(), matrixSize);

            int[,] inversePlainTextMatrix = InverseMatrix(plainTextMatrix, Alphabet.Length);
            int[,] guessedKeyMatrix = MultiplyMatrices(inversePlainTextMatrix, encryptedTextMatrix, Alphabet.Length);
            string decryptedText = DecryptWithKeyMatrix(guessedKeyMatrix);

            // Форматирование матрицы для отображения
            var matrixString = new StringBuilder();
            for (int i = 0; i < matrixSize; i++)
            {
                for (int j = 0; j < matrixSize; j++)
                {
                    matrixString.Append(guessedKeyMatrix[i, j]);
                    if (j < matrixSize - 1) // Не добавляем пробел после последнего элемента в строке
                    {
                        matrixString.Append(' ');
                    }
                }
                matrixString.AppendLine(); // Используем AppendLine для перехода на новую строку
            }

            return (matrixString.ToString(), decryptedText);
        }


        private List<int> TextToVector(string text, int blockSize)
        {
            List<int> vector = new List<int>();
            foreach (char c in text)
            {
                int index = Alphabet.IndexOf(c);
                if (index != -1)
                {
                    vector.Add(index);
                }
            }
            // Дополнение текста нулями для кратности размеру матрицы
            while (vector.Count % blockSize != 0)
            {
                vector.Add(0);
            }
            return vector;
        }

        private string VectorToText(List<int> vector)
        {
            StringBuilder text = new StringBuilder();
            foreach (int index in vector)
            {
                text.Append(Alphabet[index % Alphabet.Length]);
            }
            return text.ToString();
        }

        private int[,] InverseMatrix(int[,] matrix, int mod)
        {
            int size = matrix.GetLength(0);
            int[,] result = new int[size, size];
            // Заглушка для обратной матрицы, надо реализовать метод для нахождения обратной матрицы
            return result;
        }

        private int[] MultiplyMatrixAndVector(int[,] matrix, int[] vector)
        {
            int size = matrix.GetLength(0);
            int[] result = new int[size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    result[i] += matrix[i, j] * vector[j];
                }
                result[i] %= Alphabet.Length;
            }
            return result;
        }

        private int[,] VectorToMatrix(int[] vector, int size)
        {
            int[,] matrix = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                matrix[i, 0] = vector[i];
            }
            return matrix;
        }

        private int[,] MultiplyMatrices(int[,] matrix1, int[,] matrix2, int mod)
        {
            int size = matrix1.GetLength(0);
            int[,] result = new int[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    for (int k = 0; k < size; k++)
                    {
                        result[i, j] += matrix1[i, k] * matrix2[k, j];
                    }
                    result[i, j] %= mod;
                }
            }

            return result;
        }

        private string DecryptWithKeyMatrix(int[,] keyMatrix)
        {
            int matrixSize = keyMatrix.GetLength(0);
            int[,] inverseKeyMatrix = InverseMatrix(keyMatrix, Alphabet.Length);
            List<int> encryptedVector = TextToVector(EncryptedText, matrixSize);
            List<int> decryptedVector = new List<int>();

            for (int i = 0; i < encryptedVector.Count; i += matrixSize)
            {
                var subVector = encryptedVector.Skip(i).Take(matrixSize).ToArray();
                var decryptedSubVector = MultiplyMatrixAndVector(inverseKeyMatrix, subVector);
                decryptedVector.AddRange(decryptedSubVector);
            }

            return VectorToText(decryptedVector);
        }
    }
}
