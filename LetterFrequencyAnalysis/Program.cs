using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;

namespace LetterFrequencyAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Будь ласка, вкажіть шлях до текстових файлів для аналізу.");
                return;
            }

            foreach (string file in args)
            {
                if (File.Exists(file))
                {
                    Console.WriteLine($"\nАналіз для файлу: {file}");
                    string text = CleanText(File.ReadAllText(file));
                    var frequencies = CalculateFrequency(text);
                    int totalLetters = text.Length; // враховує пробіли

                    // a. Діаграма в алфавітному порядку
                    Console.WriteLine("Частота літер (алфавітний порядок):");
                    PrintFrequency(frequencies.OrderBy(kvp => kvp.Key));

                    // b. Діаграма по частотам появи
                    Console.WriteLine("\nЧастота літер (відсортовано по частотам):");
                    PrintFrequency(frequencies.OrderByDescending(kvp => kvp.Value));

                    // c. Послідовність літер по мірі спадання частоти
                    string sortedLetters = string.Concat(frequencies.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key));
                    Console.WriteLine("\nПослідовність літер по мірі спадання частоти:");
                    Console.WriteLine(sortedLetters);

                    // d. Загальна кількість символів
                    Console.WriteLine($"\nЗагальна кількість символів у тексті (включаючи пробіли): {totalLetters}");

                    // e. Запис у файли для побудови діаграм в Excel
                    string baseFileName = Path.GetFileNameWithoutExtension(file);
                    SaveToCsv(frequencies.OrderBy(kvp => kvp.Key), $"{baseFileName}_alphabetical.csv", "Літера", "Відносна частота (%)");
                    SaveToCsv(frequencies.OrderByDescending(kvp => kvp.Value), $"{baseFileName}_by_frequency.csv", "Літера", "Відносна частота (%)");

                    Console.WriteLine($"\nРезультати збережено у файли: {baseFileName}_alphabetical.csv та {baseFileName}_by_frequency.csv");
                }
                else
                {
                    Console.WriteLine($"Файл {file} не знайдено.");
                }
            }
        }

        static string CleanText(string text)
        {
            char[] allowedChars = "абвгґдеєжзиіїйклмнопрстуфхцчшщьюя ".ToCharArray(); // включено пробіл
            return new string(text.ToLower().Where(c => allowedChars.Contains(c)).ToArray());
        }

        static Dictionary<char, double> CalculateFrequency(string text)
        {
            Dictionary<char, int> letterCounts = new Dictionary<char, int>();
            int totalLetters = text.Length; // враховує пробіли

            foreach (char ch in text)
            {
                if (!letterCounts.ContainsKey(ch))
                    letterCounts[ch] = 0;
                letterCounts[ch]++;
            }

            return letterCounts.ToDictionary(kvp => kvp.Key, kvp => (double)kvp.Value / totalLetters);
        }

        static void PrintFrequency(IEnumerable<KeyValuePair<char, double>> frequencies)
        {
            foreach (var kvp in frequencies)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value:P2}");
            }
        }

        static void SaveToCsv(IEnumerable<KeyValuePair<char, double>> frequencies, string fileName, string header1, string header2)
        {
            using (var writer = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                writer.WriteLine($"{header1},{header2}");
                foreach (var kvp in frequencies)
                {
                    writer.WriteLine($"{kvp.Key},{(kvp.Value * 100).ToString("F2", CultureInfo.InvariantCulture)}");
                }
            }
        }
    }
}
