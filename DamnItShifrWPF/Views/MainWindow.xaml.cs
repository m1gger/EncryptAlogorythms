using DamnItShifrWPF.Factories;
using DamnItShifrWPF.Utils;
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

        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            string inputText = InputTextBox.Text;
            string key = KeyTextBox.Text;
            string selectedMethod = (CipherMethodComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string customAlphabet = AlphabetTextBox.Text;  // Алфавит, который пользователь может редактировать
            bool useRandomAlphabet = RandomAlphabetCheckBox.IsChecked == true;
            string result = string.Empty;

            // Шифрование на основе выбранного метода
            var factory = new EncryptorFactory();
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
                // Если выбран случайный алфавит для метода шифрования с двумя массивами
                string alphabetToUse = useRandomAlphabet ? AlpahabetRandomiser.RandomiseAlphabet(customAlphabet) : customAlphabet;
                result = ArrayCipher(inputText, alphabetToUse, customAlphabet);
            }

            ResultTextBox.Text = result;
        }

        // Реализация шифра Цезаря
        private string CaesarCipher(string input, int shift, string alphabet)
        {
            var factory = new EncryptorFactory();
            var encryptor = factory.CreateCaesarCipher(input, shift, alphabet);
            return encryptor.Encrypt();
        }

        // Реализация шифра Тритемиуса
        private string TrithemiusCipher(string input, string key, string alphabet)
        {
            var factory = new EncryptorFactory();
            var encryptor = factory.CreateTrithemiusCipher(input, key, alphabet);
            return encryptor.Encrypt();
        }

        // Реализация шифрования с двумя массивами
        private string ArrayCipher(string input, string key, string alphabet)
        {
            var factory = new EncryptorFactory();
            var encrypter = factory.CreateDictionaryCipher(input, key, alphabet);
            return encrypter.Encrypt();
        }

        // Обработчик изменения выбранного алфавита
        private void AlphabetComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
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
