using DamnItShifrWPF.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamnItShifrWPF.Services
{
    public class DictionaryCipherService : IEncrypter
    {

        public string Text { get; set; }
        public string Alphabet { get; set; } = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
       // public Dictionary<char,char> Dictionary { get; set; } = new Dictionary<char, char>();
       public string EncryptedText { get; set; }
        public string Key { get; set; }

        public DictionaryCipherService(string text, string alphabet, string key)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Текст  не может быть пустым");
            }// Приведение текста к нижнему регистру
            Text = text.ToLower();
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Ключ не может быть пустым");
            }

            Key = key;
            Text = text.ToLower();
           

            if (!string.IsNullOrEmpty(alphabet))
            {
                Alphabet = alphabet.ToLower();
            }
            if (Alphabet.Length > Key.Length)
            {
                for (int j = key.Length - 1; j < Alphabet.Length; j++) 
                {
                    Key.Append(Alphabet[j]);
                }
            }
        }

        public (int, string) Hack() 
        {
            string str= "Взлом доступен только для алгоримтма Цезаря";
            return (0, str);
        }

        public string Encrypt()
        {
            StringBuilder stringBuilder = new StringBuilder();
            Text = Text.ToLower();
            char[] alphabet = Alphabet.ToCharArray();

            foreach (char c in Text)
            { int index = -1;
                for (int i = 0; i < alphabet.Length; i++)
                {
                    if (alphabet[i] == c) 
                    {
                        index=i; break;
                    }
                }
                if (index == -1)
                {
                    stringBuilder.Append(c);
                }
                else
                {
                    stringBuilder.Append(Key[index]);
                }
            }
            EncryptedText= stringBuilder.ToString();
            return EncryptedText;
        }

        
       public string Decrypt()
        {
            StringBuilder stringBuilder= new StringBuilder();
            EncryptedText = EncryptedText.ToLower();
            char[] alphabet=Alphabet.ToCharArray();

            foreach (char c in EncryptedText) 
            {
                int index = -1;
                for (int i = 0; i < Key.Length; i++) 
                {
                    if (Key[i] == c) 
                    {
                        index=i; break; 
                    }
                }
                if (index == -1)
                {
                    stringBuilder.Append(c);
                }
                else
                {
                    stringBuilder.Append(Alphabet[index]);
                }
            }
            return stringBuilder.ToString();
        }

    }
}
