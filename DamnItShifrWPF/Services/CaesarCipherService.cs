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
