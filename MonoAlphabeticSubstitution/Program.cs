using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class MonoAlphabeticSubstitution
{
    private static string ukrainianAlphabet = "АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ ";

    static void Main()
    {
        Console.Write("Введіть шлях до файлу з текстом: ");
        string filePath = Console.ReadLine();

        // Зчитування тексту з файлу
        string inputText = File.ReadAllText(filePath, Encoding.UTF8);

        // Генеруємо таблицю підстановки
        Dictionary<char, char> substitutionTable = GenerateSubstitutionTable(5);

        // Зашифровуємо текст за допомогою підстановки
        string encryptedText = EncryptWithSubstitution(inputText, substitutionTable);

        // Зберігаємо зашифрований текст у файл
        string encryptedFilePath = "encrypted.txt";
        File.WriteAllText(encryptedFilePath, encryptedText, Encoding.UTF8);

        Console.WriteLine($"Зашифрований текст збережено в файл: {encryptedFilePath}");
        Console.WriteLine("Використана таблиця підстановки:");
        foreach (var pair in substitutionTable)
        {
            Console.WriteLine($"{pair.Key} -> {pair.Value}");
        }
    }

    // Функція для генерації таблиці підстановки
    static Dictionary<char, char> GenerateSubstitutionTable(int shift)
    {
        Dictionary<char, char> table = new Dictionary<char, char>();
        int alphabetLength = ukrainianAlphabet.Length;

        for (int i = 0; i < alphabetLength; i++)
        {
            char originalChar = ukrainianAlphabet[i];
            char substitutedChar = ukrainianAlphabet[(i + shift) % alphabetLength];
            table[originalChar] = substitutedChar;
        }

        return table;
    }

    // Функція шифрування з використанням таблиці підстановки
    static string EncryptWithSubstitution(string text, Dictionary<char, char> substitutionTable)
    {
        StringBuilder encrypted = new StringBuilder();

        foreach (char c in text)
        {
            if (substitutionTable.ContainsKey(char.ToUpper(c)))
            {
                char encryptedChar = substitutionTable[char.ToUpper(c)];
                encrypted.Append(char.IsLower(c) ? char.ToLower(encryptedChar) : encryptedChar);
            }
            else
            {
                encrypted.Append(c); // Символи, що не входять до алфавіту, залишаємо без змін
            }
        }

        return encrypted.ToString();
    }
}
