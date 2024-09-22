using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DamnItShifrWPF.ViewModels;

namespace DamnItShifrWPF.Services
{
    public class FrequencyAnalyzer
    {
        public static List<FrequencyAnalysisResult> AnalyzeFrequency(string text)
        {
            var result = new List<FrequencyAnalysisResult>();
            int totalChars = text.Length;

            // Группируем символы и подсчитываем их количество
            var frequency = text.GroupBy(c => c)
                                .Select(group => new
                                {
                                    Symbol = group.Key,
                                    Count = group.Count()
                                })
                                .OrderByDescending(item => item.Count);  // Добавляем сортировку по убыванию

            // Заполняем результаты с частотой в виде double
            foreach (var item in frequency)
            {
                result.Add(new FrequencyAnalysisResult
                {
                    Symbol = item.Symbol,
                    Frequency = (double)item.Count / totalChars
                });
            }

            return result;
        }
    }
}
