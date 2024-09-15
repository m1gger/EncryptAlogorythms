using DamnItShifrWPF.Interfaces;
using System;
using System.Text;

namespace DamnItShifrWPF.Services
{
    public class TrithemiusCipherService : IEncrypter
    {
        public string Alphabet { get; set; } = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        public string Text { get; set; }
        public string Key { get; set; }

        public string EncryptedText { get; set; }

        public TrithemiusCipherService(string text, string key, string alphabet)
        {
            if (!string.IsNullOrEmpty(alphabet))
            {
                Alphabet = alphabet;
            }
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Текст  не может быть пустым");
            }// Приведение текста к нижнему регистру
            Text = text.ToLower();
            if (string.IsNullOrEmpty(key)) 
            {
                throw new ArgumentException("Ключ не может быть пустым");
            }// Приведение текста к нижнему регистру
            Key = key.ToLower(); // Приведение ключа к нижнему регистру
        }

        public string Encrypt()
        {
            StringBuilder sb = new StringBuilder();
            int keyLength = Key.Length;
            for (int i = 0; i < Text.Length; i++)
            {
                char c = Text[i];
                if (c == ' ')
                {
                    sb.Append(c); // Пропускаем пробелы, добавляем их в результат
                }
                else
                {
                    sb.Append(ReplaceSymbol(c, Key[i % keyLength]));
                }
            }
            EncryptedText = sb.ToString();
            return EncryptedText;
        }

        public string Decrypt()
        {
            StringBuilder sb = new StringBuilder();
            int keyLength = Key.Length;
            for (int i = 0; i < EncryptedText.Length; i++)
            {
                char c = EncryptedText[i];
                if (c == ' ')
                {
                    sb.Append(c); // Пропускаем пробелы, добавляем их в результат
                }
                else
                {
                    sb.Append(ReplaceSymbol(c, Key[i % keyLength], isDecrypt: true));
                }
            }
            return sb.ToString();
        }

        public (int, string) Hack()
        {
            string str = "Взлом доступен только для алгоритма Цезаря";
            return (0, str);
        }

        private char ReplaceSymbol(char oldChar, char keyChar, bool isDecrypt = false)
        {
            // Проверяем, есть ли символ в алфавите
            int oldCharIndex = Alphabet.IndexOf(oldChar);
            if (oldCharIndex == -1)
            {
                // Если символ не найден в алфавите, возвращаем его без изменений
                return oldChar;
            }

            int keyCharIndex = Alphabet.IndexOf(keyChar);
            if (keyCharIndex == -1)
            {
                // Если символ ключа не найден в алфавите, возвращаем исходный символ
                return oldChar;
            }

            // Считаем смещение для символа (с учётом позиции в тексте)
            int shift = keyCharIndex;

            // Для дешифрования вычитаем смещение
            int newCharIndex = isDecrypt
                ? (oldCharIndex - shift + Alphabet.Length) % Alphabet.Length
                : (oldCharIndex + shift) % Alphabet.Length;

            return Alphabet[newCharIndex];
        }
    }
}
