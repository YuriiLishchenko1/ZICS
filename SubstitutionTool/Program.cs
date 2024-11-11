using System;
using System.Text;
using System.Collections.Generic;

namespace SubstitutionTool
{
    class Program
    {
        private static readonly string UpperAlphabet = "АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ ";
        private static readonly string LowerAlphabet = "абвгґдеєжзиіїйклмнопрстуфхцчшщьюя ";
        private static readonly int AlphabetSize = UpperAlphabet.Length;

        static void Main()
        {
            // Встановлюємо кодування консолі для підтримки українських символів
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;

            Console.WriteLine("Введіть шифротекст для брутфорс-дешифрування:");
            string cipherText = Console.ReadLine();

            Console.WriteLine("\nПочинаємо брутфорс-дешифрування...");

            List<string> possibleDecryptions = BruteForceDecrypt(cipherText);

            Console.WriteLine("\nБрутфорс-дешифрування завершено. Можливі варіанти розшифрування:");

            int count = 1;
            foreach (var decryption in possibleDecryptions)
            {
                Console.WriteLine($"\nВаріант {count}:");
                Console.WriteLine(decryption);
                count++;
            }
        }

        // Функція для брутфорс-дешифрування
        public static List<string> BruteForceDecrypt(string cipherText)
        {
            List<string> decryptions = new List<string>();

            for (int a = 1; a < AlphabetSize; a++)
            {
                if (!IsCoprime(a, AlphabetSize)) continue;

                int aInverse = ModInverse(a, AlphabetSize);
                for (int b = 0; b < AlphabetSize; b++)
                {
                    StringBuilder decryptedText = new StringBuilder();

                    foreach (char charData in cipherText)
                    {
                        int index;
                        if (UpperAlphabet.Contains(charData))
                        {
                            index = UpperAlphabet.IndexOf(charData);
                            int decryptedIndex = (aInverse * (index - b + AlphabetSize)) % AlphabetSize;
                            decryptedText.Append(UpperAlphabet[decryptedIndex]);
                        }
                        else if (LowerAlphabet.Contains(charData))
                        {
                            index = LowerAlphabet.IndexOf(charData);
                            int decryptedIndex = (aInverse * (index - b + AlphabetSize)) % AlphabetSize;
                            decryptedText.Append(LowerAlphabet[decryptedIndex]);
                        }
                        else
                        {
                            decryptedText.Append(charData); // Залишаємо інші символи без змін
                        }
                    }

                    // Додаємо розшифрований текст до списку можливих варіантів
                    decryptions.Add($"Ключі a={a}, b={b}:\n{decryptedText}");
                }
            }

            return decryptions;
        }

        // Перевірка, чи є числа взаємно простими
        private static bool IsCoprime(int a, int b)
        {
            return GCD(a, b) == 1;
        }

        // Функція для обчислення найбільшого спільного дільника (НСД)
        private static int GCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        // Обчислення оберненого значення по модулю
        private static int ModInverse(int a, int m)
        {
            int m0 = m, y = 0, x = 1;

            if (m == 1)
                return 0;

            while (a > 1)
            {
                int q = a / m;
                int t = m;

                m = a % m;
                a = t;
                t = y;

                y = x - q * y;
                x = t;
            }

            if (x < 0)
                x += m0;

            return x;
        }
    }
}
