using DamnItShifrWPF.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using MathNetLinAlg = MathNet.Numerics.LinearAlgebra;

using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using DamnItShifrWPF.Utils;
using System.Windows;

namespace DamnItShifrWPF.Services
{
    public class HillCipherService : IEncrypter
    {
        public string Text { get; set; }
        public Matrix<double> MatrixOfKey { get; private set; }
        public string Alphabet { get; set; } = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        public string EncryptedText { get; set; }

        public HillCipherService(string text,Matrix<double> matrix,string alphabet=null)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Текст не может быть пустым");
            }
            if (!string.IsNullOrEmpty(alphabet)) 
            {
                Alphabet=alphabet;
            }
            Text = text.ToLower();
            MatrixOfKey = matrix;
            var strmatrix= MatrixOfKey.Transpose().ToMatrixString(MatrixOfKey.ColumnCount, MatrixOfKey.ColumnCount);
            MessageBox.Show(strmatrix);
        }

        public  string Encrypt()
        {
          var inputText = PrepareTextToEncrypting(Text, Text.Length);
            var outputText = "";
            var portionSize = MatrixOfKey.RowCount;

            while (inputText.Length % portionSize != 0)
            {
                inputText += 'x';
            }

            for (int i = 0; i < inputText.Length; i += portionSize)
            {
                var portion = inputText.Substring(i, portionSize);
                var arrayOfIndexes = portion.Select(x => (double)Alphabet.IndexOf(x)).ToArray();
                MathNetLinAlg.Vector<double> vector = MathNetLinAlg.Vector<double>.Build.DenseOfArray(arrayOfIndexes);
                foreach (var elem in MatrixOfKey.Multiply(vector))
                {
                    outputText += Alphabet[((int)elem) % Alphabet.Length];
                }
            }
            EncryptedText = outputText;
            return outputText;
        }
        private  string PrepareTextToEncrypting(string original, int length, int startIndex = 0)
        {
            var text = "";
            var symbolsInAlphabet = original.Where(x => Alphabet.Contains(x)).ToArray();
            for (int i = startIndex; i < Math.Min(length + startIndex, symbolsInAlphabet.Length); i++)
            {
                text += symbolsInAlphabet[i];
            }
            return text;
        }

        public string Decrypt()
        {
            // Проверяем, что матрица обратима
            if (MatrixOfKey.Determinant() == 0)
            {
                throw new InvalidOperationException("Матрица ключа необратима.");
            }

            // Вычисляем обратную матрицу по модулю размера алфавита
            var mod = Alphabet.Length;
            var inverseMatrix = MatrixHelper.Inverse(MatrixOfKey, mod); // Получаем обратную матрицу

            var inputText = EncryptedText.ToLower();
            var outputText = new StringBuilder(); // Используем StringBuilder для производительности
            var portionSize = MatrixOfKey.RowCount;

            // Проверяем длину зашифрованного текста
            if (inputText.Length % portionSize != 0)
            {
                throw new ArgumentException("Длина зашифрованного текста должна быть кратна размеру матрицы.");
            }

            for (int i = 0; i < inputText.Length; i += portionSize)
            {
                var portion = inputText.Substring(i, portionSize);
                var arrayOfIndexes = portion.Select(x => (double)Alphabet.IndexOf(x)).ToArray();

                var vector = MathNetLinAlg.Vector<double>.Build.DenseOfArray(arrayOfIndexes);

                // Умножаем вектор на обратную матрицу
                var resultVector = inverseMatrix * vector;

                foreach (var elem in resultVector)
                {
                    // Приводим к индексу в алфавите по модулю
                    int index = (int)Math.Floor(elem) % mod; // Или Math.Ceiling
                    if (index < 0)
                    {
                        index += mod; // Корректируем индекс
                    }
                    outputText.Append(Alphabet[index]);
                }
            }

            return outputText.ToString(); // Преобразуем StringBuilder в строку
        }


    }
}
