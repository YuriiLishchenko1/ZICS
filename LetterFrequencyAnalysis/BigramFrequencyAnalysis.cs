using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;

namespace LetterFrequencyAnalysis
{
    class BigramFrequencyAnalysis
    {
        public static void AnalyzeBigramFrequencies(string[] args)
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
                    Console.WriteLine($"\nАналіз біграм для файлу: {file}");
                    string text = CleanText(File.ReadAllText(file));
                    var bigramFrequencies = CalculateBigramFrequency(text);

                    // Виведення таблиці з біграмами за спаданням частоти
                    Console.WriteLine("\nТаблиця з біграмами, відсортована за спаданням частоти:");
                    PrintBigramFrequency(bigramFrequencies.OrderByDescending(kvp => kvp.Value));

                    // Послідовність з 30 найбільш імовірних біграм
                    Console.WriteLine("\n30 найбільш імовірних біграм:");
                    var top30Bigrams = bigramFrequencies.OrderByDescending(kvp => kvp.Value).Take(30);
                    foreach (var bigram in top30Bigrams)
                    {
                        Console.WriteLine($"{bigram.Key}: {bigram.Value:P2}");
                    }

                    // Запис результатів у файл для побудови діаграм
                    string baseFileName = Path.GetFileNameWithoutExtension(file);
                    SaveBigramCsv(top30Bigrams, $"{baseFileName}_top30_bigrams.csv", "Біграма", "Відносна частота (%)");

                    Console.WriteLine($"\nРезультати збережено у файл: {baseFileName}_top30_bigrams.csv");
                }
                else
                {
                    Console.WriteLine($"Файл {file} не знайдено.");
                }
            }
        }

        static string CleanText(string text)
        {
            char[] allowedChars = " абвгґдеєжзиіїйклмнопрстуфхцчшщьюя".ToCharArray();
            return new string(text.ToLower().Where(c => allowedChars.Contains(c)).ToArray());
        }

        static Dictionary<string, double> CalculateBigramFrequency(string text)
        {
            Dictionary<string, int> bigramCounts = new Dictionary<string, int>();
            int totalBigrams = 0;

            for (int i = 0; i < text.Length - 1; i++)
            {
                if (char.IsWhiteSpace(text[i]) || char.IsWhiteSpace(text[i + 1]))
                    continue;

                string bigram = text.Substring(i, 2);
                if (!bigramCounts.ContainsKey(bigram))
                    bigramCounts[bigram] = 0;

                bigramCounts[bigram]++;
                totalBigrams++;
            }

            return bigramCounts.ToDictionary(kvp => kvp.Key, kvp => (double)kvp.Value / totalBigrams);
        }

        static void PrintBigramFrequency(IEnumerable<KeyValuePair<string, double>> frequencies)
        {
            int count = 0;
            foreach (var kvp in frequencies)
            {
                Console.Write($"{kvp.Key}: {kvp.Value:P2}  ");
                count++;
                if (count % 5 == 0) Console.WriteLine(); // Відображення у компактному вигляді по 5 біграм в рядок
            }
            Console.WriteLine();
        }

        static void SaveBigramCsv(IEnumerable<KeyValuePair<string, double>> frequencies, string fileName, string header1, string header2)
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
