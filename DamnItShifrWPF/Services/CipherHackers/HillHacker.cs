using DamnItShifrWPF.Interfaces;
using DamnItShifrWPF.Utils;
using MathNet.Numerics.LinearAlgebra;
using MathNetLinAlg = MathNet.Numerics.LinearAlgebra;
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

        public (string, string) Hack(int size)
        {
            // Предположим, что известные фрагменты одинаковой длины
            string ketString = CalculateMatrixOfKey(Encrypter.Text,Encrypter.EncryptedText,size).Transpose().ToMatrixString(size, size);
            string dectyptedtext = null;

            return (ketString, dectyptedtext);
        }

        private string PrepareTextToEncrypting(string original, int length, int startIndex = 0)
        {
            var text = "";
            var symbolsInAlphabet = original.Where(x => Encrypter.Alphabet.Contains(x)).ToArray();
            for (int i = startIndex; i < Math.Min(length + startIndex, symbolsInAlphabet.Length); i++)
            {
                text += symbolsInAlphabet[i];
            }
            return text;
        }

        private  Matrix<double> CalculateMatrixOfKey(string originalText, string encryptedText, int size)
        {
            Matrix<double> matrixX, matrixY;
            string originalTextPortion, encryptedTextPortion;
            var i = 0;
            do
            {
                originalTextPortion = PrepareTextToEncrypting(originalText, size * size, i);
                encryptedTextPortion = PrepareTextToEncrypting(encryptedText, size * size, i);

                matrixX = MatrixHelper.GetMatrixFromString(originalTextPortion,Encrypter.Alphabet);
                matrixY = MatrixHelper.GetMatrixFromString(encryptedTextPortion, Encrypter.Alphabet);

                i++;

            } while (!MatrixHelper.CheckConstraints(matrixX, Encrypter.Alphabet));

            return MatrixHelper.Inverse(matrixX,Encrypter.Alphabet.Length).Multiply(matrixY).Modulus(Encrypter.Alphabet.Length);


        }

        private string DecryptWithKey(Matrix<double>  MatrixOfKey)
        {
            // Проверяем, что матрица обратима
            if (MatrixOfKey.Determinant() == 0)
            {
                throw new InvalidOperationException("Матрица ключа необратима.");
            }

            // Вычисляем обратную матрицу по модулю размера алфавита
            var mod = Encrypter.Alphabet.Length;
            var inverseMatrix = MatrixHelper.Inverse(MatrixOfKey, mod); // Получаем обратную матрицу

            var inputText = Encrypter.EncryptedText.ToLower();
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
                var arrayOfIndexes = portion.Select(x => (double)Encrypter.Alphabet.IndexOf(x)).ToArray();

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
                    outputText.Append(Encrypter.Alphabet[index]);
                }
            }

            return outputText.ToString(); // Преобразуем StringBuilder в строку
        }


    }
}

