using DamnItShifrWPF.Factories;
using DamnItShifrWPF.Interfaces;
using DamnItShifrWPF.Services;
using DamnItShifrWPF.Utils;
using DamnItShifrWPF.Views;
using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using DamnItShifrWPF.Models;
using OxyPlot;
using OxyPlot.Series;
using System.Windows.Input;

namespace Views.DamnItShifrWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private IEncrypter encrypter;
        private IHacker hacker;
        private CipherTypeEnum CipherType;
        private TextAnaliserWindow textAnaliserWindow = null;
        public int MatrixSize=-1;
        string language;

        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            string inputText = InputTextBox.Text;
            string key = KeyTextBox.Text;
            string selectedMethod = (CipherMethodComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string customAlphabet = AlphabetTextBox.Text;
            bool useRandomAlphabet = RandomAlphabetCheckBox.IsChecked == true;
            string result = string.Empty;

            var factory = new EncryptorFactory();

            try
            {

                switch (CipherType)
                {
                    case CipherTypeEnum.CaesarCipher:
                        result = CaesarCipher(inputText, int.Parse(key), customAlphabet);
                        break;

                    case CipherTypeEnum.TrithemiusCipher:
                        result = TrithemiusCipher(inputText, key, customAlphabet);
                        break;

                    case CipherTypeEnum.DictionaryCipher:
                        if (string.IsNullOrEmpty(customAlphabet))
                        {
                            throw new ArgumentException("Пользовательский алфавит не может быть пустым для метода шифрования с двумя массивами.");
                        }

                        // Используем случайный алфавит, если выбран
                        string alphabetToUse = useRandomAlphabet ? AlpahabetRandomiser.RandomiseAlphabet(customAlphabet) : customAlphabet;

                        if (useRandomAlphabet)
                        {
                            KeyTextBox.Text = alphabetToUse;  // Отображаем алфавит в TextBox
                            key = alphabetToUse;
                        }

                        result = ArrayCipher(inputText, key, customAlphabet);
                        break;

                    case CipherTypeEnum.HillCipher:
                        // Логика для шифра Хилла (если требуется)
                        result = HillCipher(inputText, int.Parse(key), customAlphabet);
                        break;

                    default:
                        throw new InvalidOperationException("Неизвестный тип шифра.");
                }



                // Обновляем окно анализа
                UpdateAnalysis(inputText, result, selectedMethod);

                ResultTextBox.Text = result;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неизвестная ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CipherMethodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Получаем выбранный элемент
            var selectedComboBoxItem = CipherMethodComboBox.SelectedItem as ComboBoxItem;

            if (selectedComboBoxItem != null)
            {
                // Получаем текст выбранного элемента
                string selectedMethod = selectedComboBoxItem.Content.ToString();

                // В зависимости от выбора устанавливаем значение поля класса и изменяем текст в TextBlock
                switch (selectedMethod)
                {
                    case "Шифр Цезаря":
                        CipherType = CipherTypeEnum.CaesarCipher;
                        KeyTextBlock.Text = "Введите ключ:"; // Изменяем текст на "Введите ключ"
                        break;
                    case "Шифр Тритемиуса":
                        CipherType = CipherTypeEnum.TrithemiusCipher;
                        KeyTextBlock.Text = "Введите ключевое слово:"; // Изменяем текст на "Введите ключ"
                        break;
                    case "Шифрование с двумя массивами":
                        CipherType = CipherTypeEnum.DictionaryCipher;
                        KeyTextBlock.Text = "Введите алфавит для шифрования:"; // Изменяем текст на "Введите ключ"
                        break;
                    case "Шифр Хилла":
                        CipherType = CipherTypeEnum.HillCipher;
                        KeyTextBlock.Text = "Введите размер матрицы:";
                        // Изменяем текст на "Введите размер матрицы"
                        break;
                    default:
                        CipherType = CipherTypeEnum.None;
                        KeyTextBlock.Text = "Введите ключ:"; // Возвращаем текст на "Введите ключ"
                        break;
                }
            }
        }



        // Метод для обновления анализа
        private void UpdateAnalysis(string originalText, string encryptedText, string method)
        {
            // Анализ зашифрованного текста
            var encryptedTextAnalyzer = new TextAnaliser(encryptedText);
            var frequenciesEncrypted = encryptedTextAnalyzer.CalculateFrequencies();

            // Проверка окна анализа
            if (textAnaliserWindow == null || !textAnaliserWindow.IsVisible)
            {
                textAnaliserWindow = new TextAnaliserWindow();
                textAnaliserWindow.Show();
            }

            // Преобразование данных для русского языка (частоты символов русского алфавита)
            var alphabetFreqList = (language == "ru" ? TextAnaliser.RussianFrequencies : TextAnaliser.EnglishFrequencies)
                .Select(pair => new KeyValuePair<char, double>(pair.Key, Math.Round(pair.Value * 100, 2)))
                .ToList();

            // Преобразование данных для зашифрованного текста
            var encryptedTextFrequencyList = frequenciesEncrypted
                .Select(pair => new KeyValuePair<char, double>(pair.Key, Math.Round(pair.Value * 100, 2)))
                .ToList();

            // Обновляем таблицы
            textAnaliserWindow.OriginalTextFrequencyDataGrid.ItemsSource = alphabetFreqList; // Частоты русского алфавита
            textAnaliserWindow.EncryptedTextFrequencyDataGrid.ItemsSource = encryptedTextFrequencyList; // Частоты зашифрованного текста

            // Построение общего графика
            DrawCombinedFrequencyGraph(alphabetFreqList, encryptedTextFrequencyList,method);
        }


        private void DrawCombinedFrequencyGraph(List<KeyValuePair<char, double>> alphabetFreqList, List<KeyValuePair<char, double>> encryptedTextFrequencyList, string method)
        {
            PlotModel plotModel;

            if (textAnaliserWindow.PlotView.Model == null)
            {
                plotModel = new PlotModel { Title = "Частоты алфавита и зашифрованного текста" };
                textAnaliserWindow.PlotView.Model = plotModel; // Задаем модель, если она не существует
            }
            else
            {
                plotModel = textAnaliserWindow.PlotView.Model;
            }

            // Создаем серии для алфавита и зашифрованного текста
            OxyColor color;
            switch (method) 
            {
                case "Шифр Тритемиуса":
                    color = OxyColors.Green; break;

                case "Шифрование с двумя массивами":
                    color = OxyColors.Blue; break;
                    break;
                case "Шифр Цезаря":
                    color = OxyColors.Orange; break;

                default : color = OxyColors.Black;
                    break;

            }
            var alphabetSeries = new LineSeries { Title = "Частоты алфавита", Color = OxyColors.Red };
            var encryptedSeries = new LineSeries { Title = $"Частоты {method}",Color=color };
            

            int index = 0;

            // Добавляем данные для алфавита
            foreach (var pair in alphabetFreqList)
            {
                alphabetSeries.Points.Add(new DataPoint(index++, pair.Value));
            }

            index = 0; // Сброс индекса для зашифрованного текста

            // Добавляем данные для зашифрованного текста
            foreach (var pair in encryptedTextFrequencyList)
            {
                encryptedSeries.Points.Add(new DataPoint(index++, pair.Value));
            }

            // Добавляем новые серии в модель графика
            plotModel.Series.Add(alphabetSeries);
            plotModel.Series.Add(encryptedSeries);

            // Принудительная перерисовка графика
            textAnaliserWindow.PlotView.InvalidatePlot(true);
        }









        private string CaesarCipher(string input, int shift, string alphabet)
        {
            try
            {
                var factory = new EncryptorFactory();
                encrypter = factory.CreateCaesarCipher(input, shift, alphabet);
                return encrypter.Encrypt();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неизвестная ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }
        }

        private string TrithemiusCipher(string input, string key, string alphabet)
        {
            try
            {
                var factory = new EncryptorFactory();
                encrypter = factory.CreateTrithemiusCipher(input, key, alphabet);
                return encrypter.Encrypt();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неизвестная ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }
        }

        private string ArrayCipher(string input, string key, string alphabet)
        {
            try
            {
                var factory = new EncryptorFactory();
                encrypter = factory.CreateDictionaryCipher(input, key, alphabet);
                return encrypter.Encrypt();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неизвестная ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }
        }

        private string HillCipher(string text, int matrixsize, string alphabet) 
        {
            try
            {
                var factory = new EncryptorFactory();
                encrypter = factory.CreateHillCipher(text, matrixsize, alphabet);
                return encrypter.Encrypt();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неизвестная ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }
        }

        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            if (encrypter != null)
            {
                ResultTextBox.Text = encrypter.Decrypt();
            }
            else
            {
                ResultTextBox.Text = "Нет данных для расшифровки.";
            }
        }

        private (string, string) HackEncryptedText()
        {
            switch (CipherType) 
            {
                case CipherTypeEnum.HillCipher:
                 
                    hacker=HackerFactory.CreateHacker(CipherTypeEnum.HillCipher,encrypter);
                    break;
                case CipherTypeEnum.CaesarCipher:
                    hacker = HackerFactory.CreateHacker(CipherTypeEnum.CaesarCipher, encrypter);
                    break;
                default:
                    break;
            }
            return hacker?.Hack() ?? ("0", "Ошибка: нет данных для взлома.");
        }

        private void HackButton_Click(object sender, RoutedEventArgs e)
        {
            var (key, decryptedText) = HackEncryptedText();
            ResultTextBox.Text = $"Ключ: {key}, Текст: {decryptedText}";
        }

        private void AlphabetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedAlphabet = (AlphabetComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (selectedAlphabet == "Русский")
            {
                AlphabetTextBox.Text = AlpahabetRandomiser.GetRussianAlphabet();
                language = "ru";
               
            }
            else if (selectedAlphabet == "Английский")
            {
                AlphabetTextBox.Text = AlpahabetRandomiser.GetEnglishAlphabet();
                language = "en";

            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                InputTextBox.Text = File.ReadAllText(openFileDialog.FileName);
            }
        }

        // Обработчик кнопки "Экспорт"
        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, ResultTextBox.Text);
            }
        }

    }
}
