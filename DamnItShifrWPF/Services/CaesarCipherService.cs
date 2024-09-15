using DamnItShifrWPF.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamnItShifrWPF.Services
{
    public class CaesarCipherService : IEncrypter
    {
        public string Text { get; set; }

        public int Key { get; set; }

        public string Alphabet { get; set; } = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";

        public string EncryptedText { get; set; }

        public CaesarCipherService(string text,int key, string alphabet)
        {
            if (!string.IsNullOrEmpty(alphabet)) 
            {
                Alphabet = alphabet.ToLower();
            }
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Текст  не может быть пустым");
            }// Приведение текста к нижнему регистру
            Text = text.ToLower();
          
            
            Key = key;
        }

        public string Encrypt()
        {
            StringBuilder encryptedText = new StringBuilder();
            Text = Text.ToLower();

            foreach (char c in Text)
            {
                encryptedText.Append(ReplaceSymbol(c, Key));
            }
            EncryptedText = encryptedText.ToString();
            return EncryptedText;
        }

        public string Decrypt()
        {
            StringBuilder decryptedText = new StringBuilder();

            foreach (char c in EncryptedText)
            {
                decryptedText.Append(ReplaceSymbol(c, -Key)); // Обратный сдвиг
            }

            return decryptedText.ToString();
        }

        public (int, string) Hack()
        {
            string originalEncryptedText = EncryptedText.ToLower();  // Работать с исходным зашифрованным текстом
            string bestAttempt = string.Empty;
            int bestKey = 0;
            double bestScore = double.MinValue;  // Оценка дешифровки будет теперь числом с плавающей точкой

            // Перебор всех возможных ключей
            for (int possibleKey = 0; possibleKey < Alphabet.Length; possibleKey++)
            {
                string attempt = DecryptWithKey(possibleKey, originalEncryptedText);

                // Оценим вероятность правильной дешифровки
                double score = ScoreDecryption(attempt);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestAttempt = attempt;
                    bestKey = possibleKey;
                }
            }

            if (bestScore > 0)
            {
                return (bestKey, bestAttempt);  // Возвращаем лучший ключ и дешифрованный текст
            }

            return (0, "Взлом не удался");
        }

        private double ScoreDecryption(string decryptedText)
        {
            // Частоты букв русского языка для оценки
            Dictionary<char, double> frequencies = new Dictionary<char, double>
    {
        {'о', 0.1097}, {'е', 0.0845}, {'а', 0.0801}, {'и', 0.0735}, {'н', 0.0670},
        {'т', 0.0626}, {'с', 0.0547}, {'р', 0.0473}, {'в', 0.0454}, {'л', 0.0434},
        {'к', 0.0349}, {'м', 0.0321}, {'д', 0.0298}, {'п', 0.0281}, {'у', 0.0262},
        {'я', 0.0201}, {'ы', 0.0189}, {'ь', 0.0174}, {'г', 0.0167}, {'з', 0.0165},
        {'б', 0.0159}, {'ч', 0.0144}, {'й', 0.0121}, {'х', 0.0097}, {'ж', 0.0094},
        {'ш', 0.0073}, {'ю', 0.0064}, {'ц', 0.0048}, {'щ', 0.0036}, {'э', 0.0032},
        {'ф', 0.0026}, {'ъ', 0.0004}, {'ё', 0.0004}
    };

            double score = 0;

            foreach (char c in decryptedText)
            {
                if (frequencies.ContainsKey(c))
                {
                    score += frequencies[c];  // Добавляем оценку частоты каждой буквы
                }
            }

            return score;
        }


        private string DecryptWithKey(int key, string encryptedText)
        {
            StringBuilder decryptedText = new StringBuilder();

            foreach (char c in encryptedText)
            {
                decryptedText.Append(ReplaceSymbol(c, -key));  // Обратный сдвиг
            }

            return decryptedText.ToString();
        }



        private char ReplaceSymbol(char c, int key)
        {
            int index = Alphabet.IndexOf(c);

            // Если символ не найден в алфавите, возвращаем его без изменений
            if (index == -1)
            {
                return c;
            }

            // Рассчитываем новый индекс с учётом сдвига
            int newIndex = (index + key) % Alphabet.Length;

            // Корректируем новый индекс, если он стал отрицательным
            if (newIndex < 0)
            {
                newIndex += Alphabet.Length;
            }

            return Alphabet[newIndex];
        }



        
    }

}
