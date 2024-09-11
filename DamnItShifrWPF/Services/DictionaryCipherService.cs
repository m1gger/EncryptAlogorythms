using DamnItShifrWPF.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamnItShifrWPF.Services
{
    public class DictionaryCipherService : IEncrypter
    {

        public string Text { get; set; }
        public string Alphabet { get; set; } = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        public Dictionary<char,char> Dictionary { get; set; } = new Dictionary<char, char>();

        public DictionaryCipherService(string text, string alphabet, string key)
        {
            Text = text;
            if (!string.IsNullOrEmpty(alphabet))
            {
                Alphabet = alphabet.ToLower();
            }
            for (int i = 0; i < Alphabet.Length; i++) 
            {
                Dictionary.Add(Alphabet[i], key[i]);
            }
        }


        public string Encrypt()
        {
            StringBuilder stringBuilder = new StringBuilder();
            Text = Text.ToLower();

            foreach (char c in Text)
            {
                // Проверяем, есть ли символ в словаре
                if (Dictionary.ContainsKey(c))
                {
                    stringBuilder.Append(Dictionary[c]);
                }
                else
                {
                    // Если символ не найден в словаре, добавляем его без изменений
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString();
        }
    }
}
