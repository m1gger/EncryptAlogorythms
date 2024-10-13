using DamnItShifrWPF.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using MathNetLinAlg = MathNet.Numerics.LinearAlgebra;

using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;

namespace DamnItShifrWPF.Services
{
    public class HillCipherService : IEncrypter
    {
        public string Text { get; set; }
        public Matrix<double> MatrixOfKey { get; private set; }
        public string Alphabet { get; set; } = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        public string EncryptedText { get; set; }

        public HillCipherService(string text, int matrixSize)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Текст не может быть пустым");
            }

            Text = text.ToLower();
            MatrixOfKey = GenerateKeyMatrix(matrixSize);
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
            //foreach (var ch in original.Where(x => Settings.ALPHABET.Contains(x)).ToArray())
            //{
            //    text += ch;
            //    if (length != null && text.Length == length) break;
            //}
            return text;
        }
    }
}
