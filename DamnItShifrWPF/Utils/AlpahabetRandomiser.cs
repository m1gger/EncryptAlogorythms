using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamnItShifrWPF.Utils
{
  public static  class AlpahabetRandomiser
    {
        public static string GetRussianAlphabet() 
        {
            return "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        }

        public static string GetEnglishAlphabet() 
        {
            return "abcdefghijklmnopqrstuvwxyz";
        }
        public static string RandomiseAlphabet(string alphabet)
        {
            alphabet = alphabet.ToLower();
            char[] characters = alphabet.ToCharArray();
            Random random = new Random();

            // Перемешиваем массив символов
            for (int i = characters.Length - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                // Меняем местами characters[i] и characters[j]
                char temp = characters[i];
                characters[i] = characters[j];
                characters[j] = temp;
            }

            // Создаем строку из перемешанных символов
            return new string(characters);
        }

        public static string RandomiseRussianAlphabet() 
        {
            string aplhabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
            return RandomiseAlphabet(aplhabet);
        }

        public static string RandomiseEnglishAlphabet()
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            return RandomiseAlphabet(alphabet);
        }
    }
}
