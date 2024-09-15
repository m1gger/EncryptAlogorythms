using DamnItShifrWPF.Factories;
using DamnItShifrWPF.Interfaces;
using DamnItShifrWPF.Services;
using DamnItShifrWPF.Utils;
using DamnItShifrWPF.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Views.DamnItShifrWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private IEncrypter encrypter;

        private void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            string inputText = InputTextBox.Text;
            var textAnalyzer = new TextAnaliser(inputText);

            var frequencies = textAnalyzer.CalculateFrequencies();

            TextAnaliserWindow analysisWindow = new TextAnaliserWindow();
            var analysisText = new StringBuilder();
            foreach (var pair in frequencies)
            {
                analysisText.AppendLine($"{pair.Key}: {pair.Value * 100:F2}%");
            }

            analysisWindow.AnalysisResultTextBlock.Text = analysisText.ToString();
            analysisWindow.Show();
        }

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
                if (selectedMethod == "Шифр Цезаря")
                {
                    result = CaesarCipher(inputText, int.Parse(key), customAlphabet);
                }
                else if (selectedMethod == "Шифр Тритемиуса")
                {
                    result = TrithemiusCipher(inputText, key, customAlphabet);
                }
                else if (selectedMethod == "Шифрование с двумя массивами")
                {
                    if (string.IsNullOrEmpty(customAlphabet))
                    {
                        throw new ArgumentException("Пользовательский алфавит не может быть пустым для метода шифрования с двумя массивами.");
                    }

                    string alphabetToUse = useRandomAlphabet ? AlpahabetRandomiser.RandomiseAlphabet(customAlphabet) : customAlphabet;

                    if (useRandomAlphabet)
                    {
                        KeyTextBox.Text = alphabetToUse;
                        key = alphabetToUse;
                    }

                    result = ArrayCipher(inputText, key, customAlphabet);
                }
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

        private (int, string) HackEncryptedText()
        {
            return encrypter?.Hack() ?? (0, "Ошибка: нет данных для взлома.");
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
            }
            else if (selectedAlphabet == "Английский")
            {
                AlphabetTextBox.Text = AlpahabetRandomiser.GetEnglishAlphabet();
            }
        }
    }
}
