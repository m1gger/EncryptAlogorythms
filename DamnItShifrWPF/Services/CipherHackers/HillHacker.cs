using DamnItShifrWPF.Interfaces;
using DamnItShifrWPF.Utils;
using MathNet.Numerics.LinearAlgebra;
using MathNetLinAlg = MathNet.Numerics.LinearAlgebra;
using System;
using System.Windows;
using System.Linq;
using System.Text;

namespace DamnItShifrWPF.Services.CipherHackers
{
    public class HillHacker : IHacker
    {
        public IEncrypter Encrypter { get; set; }

        public HillHacker(IEncrypter encrypter)
        {
            Encrypter = encrypter;
        }

        public (string, string) Hack(int size)
        {
            // Предположим, что известные фрагменты одинаковой длины
            var keymatrix = CalculateMatrixOfKey(Encrypter.Text, Encrypter.EncryptedText, size);
           string keyString = keymatrix.ToMatrixString(size, size);
            string decryptedtext = DecryptWithKey(keymatrix.Transpose());
            
            //MessageBox.Show(keyString);
            
            return (keyString, decryptedtext);
        }

      


    private string PrepareTextToEncrypting(string original, int length, int startIndex = 0)
        {
            var symbolsInAlphabet = original.Where(x => Encrypter.Alphabet.Contains(x)).ToArray();
            return new string(symbolsInAlphabet.Skip(startIndex).Take(length).ToArray());
        }

        private Matrix<double> CalculateMatrixOfKey(string originalText, string encryptedText, int size)
        {
            Matrix<double> matrixX, matrixY;
            string originalTextPortion, encryptedTextPortion;
            var i = 0;
            do
            {
                originalTextPortion = PrepareTextToEncrypting(originalText, size * size, i);
                encryptedTextPortion = PrepareTextToEncrypting(encryptedText, size * size, i);

                matrixX = MatrixHelper.GetMatrixFromString(originalTextPortion, Encrypter.Alphabet);
                matrixY = MatrixHelper.GetMatrixFromString(encryptedTextPortion, Encrypter.Alphabet);

                i++;
                if (i >= 1000)
                {
                    throw new Exception("Взлом не удался, не найдена обратная матрица.");
                }
            } 
            while (!MatrixHelper.CheckConstraints(matrixX, Encrypter.Alphabet));

            return MatrixHelper.Inverse(matrixX, Encrypter.Alphabet.Length).Multiply(matrixY).Modulus(Encrypter.Alphabet.Length);
        }

        public string DecryptWithKey(Matrix<double> matrixOfKey)
        {
            // Проверяем, что матрица обратима
            if (matrixOfKey.Determinant() == 0)
            {
                throw new InvalidOperationException("Матрица ключа необратима.");
            }

            // Вычисляем обратную матрицу по модулю размера алфавита
            var mod = Encrypter.Alphabet.Length;
            var inverseMatrix = MatrixHelper.Inverse(matrixOfKey, mod); // Получаем обратную матрицу

            var inputText = Encrypter.EncryptedText.ToLower();
            var outputText = new StringBuilder(); // Используем StringBuilder для производительности
            var portionSize = matrixOfKey.RowCount;

            // Проверяем длину зашифрованного текста
            if (inputText.Length % portionSize != 0)
            {
                // Если длина не кратна размеру матрицы, добавляем символы-заполнители
                inputText = inputText.PadRight((inputText.Length / portionSize + 1) * portionSize, 'x');
            }

            for (int i = 0; i < inputText.Length; i += portionSize)
            {
                var portion = inputText.Substring(i, portionSize);
                var arrayOfIndexes = portion.Select(x => (double)Encrypter.Alphabet.IndexOf(x)).ToArray();

                var vector = MathNetLinAlg.Vector<double>.Build.DenseOfArray(arrayOfIndexes);

                // Умножаем вектор на обратную матрицу
                var resultVector = inverseMatrix * vector;

                foreach (var elem in resultVector)
                {
                    // Приводим к индексу в алфавите по модулю
                    int index = ((int)Math.Floor(elem) % mod + mod) % mod; // Корректируем индекс по модулю
                    outputText.Append(Encrypter.Alphabet[index]);
                }
            }

            return outputText.ToString(); // Преобразуем StringBuilder в строку
        }

    }
}