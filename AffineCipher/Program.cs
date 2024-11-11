using System;
using System.Text;

namespace AffineCipher
{
    class Program
    {
        private static readonly string UpperAlphabet = "АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ ";
        private static readonly string LowerAlphabet = "абвгґдеєжзиіїйклмнопрстуфхцчшщьюя ";
        private static readonly int AlphabetSize = UpperAlphabet.Length;

        static void Main()
        {
            // Встановлюємо кодування консолі для коректного відображення українських символів
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;

            string text = "Степан нарешті підібрав собі нове чудове помешкання, устаткував його меблями, але ніяк не міг до нього звикнути, воно було для нього ніби чуже. Нудьга його не кидала. Якось зустрів односельця Левка. Він закінчив своє навчання і збирався їхати працювати на Херсонщину. Левко сказав йому, що він так і не призвичаївся до життя у великому місті.";

            // Генеруємо випадкові значення для 'a' та 'b'
           // int a = GenerateRandomCoprime(AlphabetSize);
            //Random random = new Random();
            //int b = random.Next(0, AlphabetSize);

            Console.WriteLine($"Випадкові значення ключів для шифрування: a={31}, b={29}\n");

            // Шифруємо текст
            string encryptedText = Encrypt(text, 31, 29);
            Console.WriteLine("Зашифрований текст:");
            Console.WriteLine(encryptedText);

            // Додаємо можливість розшифрувати текст
            Console.WriteLine("\nВведіть зашифрований текст для дешифрування:");
            string encryptedInput = Console.ReadLine();

            Console.WriteLine("Введіть значення ключа 'a':");
            int aKey = int.Parse(Console.ReadLine());

            Console.WriteLine("Введіть значення ключа 'b':");
            int bKey = int.Parse(Console.ReadLine());

            string decryptedText = Decrypt(encryptedInput, aKey, bKey);

            if (decryptedText != null)
            {
                Console.WriteLine("Розшифрований текст:");
                Console.WriteLine(decryptedText);
            }
            else
            {
                Console.WriteLine("Не вдалося розшифрувати текст. Перевірте значення ключів.");
            }
        }

        // Функція для генерації випадкового 'a', яке взаємно просте з 'm'
        private static int GenerateRandomCoprime(int m)
        {
            Random random = new Random();
            int a;
            do
            {
                a = random.Next(1, m);
            } while (GCD(a, m) != 1);
            return a;
        }

        // Функція для шифрування
        public static string Encrypt(string data, int a, int b)
        {
            StringBuilder encryptedText = new StringBuilder();
            foreach (char charData in data)
            {
                int index;
                if (UpperAlphabet.Contains(charData))
                {
                    index = UpperAlphabet.IndexOf(charData);
                    int encryptedIndex = (a * index + b) % AlphabetSize;
                    encryptedText.Append(UpperAlphabet[encryptedIndex]);
                }
                else if (LowerAlphabet.Contains(charData))
                {
                    index = LowerAlphabet.IndexOf(charData);
                    int encryptedIndex = (a * index + b) % AlphabetSize;
                    encryptedText.Append(LowerAlphabet[encryptedIndex]);
                }
                else
                {
                    encryptedText.Append(charData); // Залишаємо інші символи без змін
                }
            }

            return encryptedText.ToString();
        }

        // Функція для дешифрування
        public static string Decrypt(string cipherText, int a, int b)
        {
            if (!IsCoprime(a, AlphabetSize))
            {
                Console.WriteLine("Значення 'a' не є взаємно простим з розміром алфавіту.");
                return null;
            }

            StringBuilder decryptedText = new StringBuilder();
            int aInverse = ModInverse(a, AlphabetSize);

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

            return decryptedText.ToString();
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
