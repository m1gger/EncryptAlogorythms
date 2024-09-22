using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamnItShifrWPF.Services
{
    public class TextAnaliser
    {
        public string Text { get; set; }

        public static Dictionary<char, double> RussianFrequencies { get; set; } = new Dictionary<char, double>
        {
            {'о', 0.1097}, {'е', 0.0845}, {'а', 0.0801}, {'и', 0.0735}, {'н', 0.0670},
            {'т', 0.0626}, {'с', 0.0547}, {'р', 0.0473}, {'в', 0.0454}, {'л', 0.0434},
            {'к', 0.0349}, {'м', 0.0321}, {'д', 0.0298}, {'п', 0.0281}, {'у', 0.0262},
            {'я', 0.0201}, {'ы', 0.0189}, {'ь', 0.0174}, {'г', 0.0167}, {'з', 0.0165},
            {'б', 0.0159}, {'ч', 0.0144}, {'й', 0.0121}, {'х', 0.0097}, {'ж', 0.0094},
            {'ш', 0.0073}, {'ю', 0.0064}, {'ц', 0.0048}, {'щ', 0.0036}, {'э', 0.0032},
            {'ф', 0.0026}, {'ъ', 0.0004}, {'ё', 0.0004}
         };

        public static Dictionary<char, double> EnglishFrequencies { get; set; } = new Dictionary<char, double>
        {
            {'e', 0.1270}, {'t', 0.0906}, {'a', 0.0817}, {'o', 0.0751}, {'i', 0.0697},
            {'n', 0.0675}, {'s', 0.0633}, {'h', 0.0609}, {'r', 0.0599}, {'d', 0.0425},
            {'l', 0.0403}, {'c', 0.0278}, {'u', 0.0276}, {'m', 0.0241}, {'w', 0.0236},
            {'f', 0.0223}, {'g', 0.0202}, {'y', 0.0197}, {'p', 0.0193}, {'b', 0.0149},
            {'v', 0.0098}, {'k', 0.0077}, {'j', 0.0015}, {'x', 0.0015}, {'q', 0.0010},
            {'z', 0.0007}
        };

        public TextAnaliser(string text)
        {
            Text = text;
        }

        public Dictionary<char, double> TextFrequencies { get; set; } = new Dictionary<char, double>();

        public Dictionary<char, double> CalculateFrequencies()
        {
            // Инициализируем словарь для частот букв в тексте
            Dictionary<char, double> frequencies = new Dictionary<char, double>();

            // Приводим текст к нижнему регистру и убираем все символы, кроме букв
            string cleanedText = new string(Text.ToLower().Where(c => char.IsLetter(c)).ToArray());

            int totalLetters = cleanedText.Length; // Общее количество букв в тексте

            // Подсчитываем количество вхождений каждой буквы
            foreach (char c in cleanedText)
            {
                if (frequencies.ContainsKey(c))
                {
                    frequencies[c]++;
                }
                else
                {
                    frequencies[c] = 1;
                }
            }

            // Преобразуем количество вхождений в частоты
            foreach (char key in frequencies.Keys.ToList())
            {
                frequencies[key] = frequencies[key] / totalLetters;
            }

            // Сортируем по убыванию частот
            var sortedFrequencies = frequencies.OrderByDescending(pair => pair.Value)
                                               .ToDictionary(pair => pair.Key, pair => pair.Value);

            TextFrequencies = sortedFrequencies; // Сохраняем отсортированный результат анализа

            return sortedFrequencies;
        }



    }
}
