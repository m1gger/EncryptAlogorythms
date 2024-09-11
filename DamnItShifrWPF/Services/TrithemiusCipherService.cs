using DamnItShifrWPF.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamnItShifrWPF.Services
{
    public class TrithemiusCipherService : IEncrypter
    {

        public string Alphabet { get; set; } = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        public string Text { get; set; }
        public string Key { get; set; }

        public TrithemiusCipherService(string text,string key, string aplhabet)
        {
            if (!string.IsNullOrEmpty(aplhabet)) 
            {
                Alphabet = aplhabet;
            }
            Text = text;
            Key = key;
        }

        public string Encrypt() 
        {
            StringBuilder sb = new StringBuilder();
            Text=Text.ToLower();
            foreach (char c in Text) 
            {
                sb.Append(ReplaceSymbol(c,Key));
            }
            return sb.ToString();
        }

        private char ReplaceSymbol(char oldChar, string key)
        {
            // Проверяем, есть ли символ в алфавите
            int oldCharIndex = Alphabet.IndexOf(oldChar);
            if (oldCharIndex == -1)
            {
                // Если символ не найден в алфавите, возвращаем его без изменений
                return oldChar;
            }

            // Определяем длину ключа
            int keyLength = key.Length;

            // Считаем смещение для текущего символа по ключу
            int keyIndex = oldCharIndex % keyLength;
            char keyChar = key[keyIndex];

            // Считаем новое смещение для символа (с учётом позиции в тексте)
            int shift = Alphabet.IndexOf(keyChar);
            int newCharIndex = (oldCharIndex + shift) % Alphabet.Length;

            return Alphabet[newCharIndex];
        }

    }
}
