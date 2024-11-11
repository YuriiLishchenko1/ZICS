using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Text;

namespace TrigramFrequencyAnalysisProject
{
    class TrigramFrequencyAnalysis
    {
        public static void Main(string[] args)
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
                    Console.WriteLine($"\nАналіз триграм для файлу: {file}");
                    string text = CleanText(File.ReadAllText(file));
                    var trigramFrequencies = CalculateTrigramFrequency(text);

                    // Виведення таблиці з триграмами за спаданням частоти
                    Console.WriteLine("\nТаблиця з триграмами, відсортована за спаданням частоти:");
                    PrintTrigramFrequency(trigramFrequencies.OrderByDescending(kvp => kvp.Value));

                    // Послідовність з 30 найбільш імовірних триграм
                    Console.WriteLine("\n30 найбільш імовірних триграм:");
                    var top30Trigrams = trigramFrequencies.OrderByDescending(kvp => kvp.Value).Take(30);
                    foreach (var trigram in top30Trigrams)
                    {
                        Console.WriteLine($"{trigram.Key}: {trigram.Value:P4}");
                    }

                    // Запис результатів у файли для побудови діаграм та звіту
                    string baseFileName = Path.GetFileNameWithoutExtension(file);
                    SaveTrigramCsv(top30Trigrams, $"{baseFileName}_top30_trigrams.csv", "Триграма", "Відносна частота (%)");
                    SaveAllTrigramsCsv(trigramFrequencies, $"{baseFileName}_all_trigrams.csv");

                    Console.WriteLine($"\nРезультати збережено у файли: {baseFileName}_top30_trigrams.csv та {baseFileName}_all_trigrams.csv");

                    // Виведення 30 найбільш імовірних триграм в один рядок без частот
                    Console.WriteLine("\n30 найбільш імовірних триграм в один рядок:");
                    Console.WriteLine(string.Join(" ", top30Trigrams.Select(t => t.Key)));
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

        static Dictionary<string, double> CalculateTrigramFrequency(string text)
        {
            Dictionary<string, int> trigramCounts = new Dictionary<string, int>();
            int totalTrigrams = 0;

            for (int i = 0; i < text.Length - 2; i++)
            {
                if (char.IsWhiteSpace(text[i]) || char.IsWhiteSpace(text[i + 1]) || char.IsWhiteSpace(text[i + 2]))
                    continue;

                string trigram = text.Substring(i, 3);
                if (!trigramCounts.ContainsKey(trigram))
                    trigramCounts[trigram] = 0;

                trigramCounts[trigram]++;
                totalTrigrams++;
            }

            return trigramCounts.ToDictionary(kvp => kvp.Key, kvp => (double)kvp.Value / totalTrigrams);
        }

        static void PrintTrigramFrequency(IEnumerable<KeyValuePair<string, double>> frequencies)
        {
            int count = 0;
            foreach (var kvp in frequencies)
            {
                Console.Write($"{kvp.Key}: {kvp.Value:P4}  ");
                count++;
                if (count % 5 == 0) Console.WriteLine(); // Відображення у компактному вигляді по 5 триграм в рядок
            }
            Console.WriteLine();
        }

        static void SaveTrigramCsv(IEnumerable<KeyValuePair<string, double>> frequencies, string fileName, string header1, string header2)
        {
            using (var writer = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                writer.WriteLine($"{header1},{header2}");
                foreach (var kvp in frequencies)
                {
                    writer.WriteLine($"{kvp.Key},{(kvp.Value * 100).ToString("F4", CultureInfo.InvariantCulture)}");
                }
            }
        }

        static void SaveAllTrigramsCsv(Dictionary<string, double> trigramFrequencies, string fileName)
        {
            using (var writer = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                writer.WriteLine("Триграма,Відносна частота (%)");
                foreach (var kvp in trigramFrequencies.OrderByDescending(kvp => kvp.Value))
                {
                    writer.WriteLine($"{kvp.Key},{(kvp.Value * 100).ToString("F4", CultureInfo.InvariantCulture)}");
                }
            }
        }
    }
}
