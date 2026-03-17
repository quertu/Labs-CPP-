using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

class Program
{
    [DllImport("kernel32.dll")]
    static extern bool SetConsoleCP(uint wCodePageID);

    [DllImport("kernel32.dll")]
    static extern bool SetConsoleOutputCP(uint wCodePageID);
    static void Main()
    {
        SetConsoleCP(1251);
        SetConsoleOutputCP(1251);

        char choice;

        do {
        Console.Clear();

        Console.WriteLine("\n Введіть рядок:");
        string text = Console.ReadLine();

            // Банк слів → цифры (дефолтний клас System.Collections.Generic,
            // працюючий за принципом порівняння ключа та доданих даних деякого типу)
            var wordToDigit = new Dictionary<string, string>()
        {
            {"нуль","0"}, {"один","1"}, {"два","2"}, {"три","3"},
            {"чотири","4"}, {"п'ять","5"}, {"шість","6"},
            {"сім","7"}, {"вісім","8"}, {"дев'ять","9"}
        };

            bool isValid = true;

            // Перевірка на недопустимі символи
            foreach (char c in text)
            {
                if (!(char.IsLetter(c) || c == ' ' || c == '\''))
                {
                    isValid = false;
                    break;
                }
            }

            if (!isValid)
            {
                Console.WriteLine("\n Помилка: рядок містить недопустимі символи!");
                Console.WriteLine(" Дозволено лише українські літери, пробіли та апостроф.");

                Console.WriteLine("\n Бажаєте продовжити? (Y/N):");
                choice = char.ToLower(Console.ReadKey().KeyChar);
                Console.WriteLine();

                continue; // перехід до наступної ітерації
            }


        // Відокремлюємо слова та замінюємо їх на цифри
        string[] words = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < words.Length; i++)
        {
            if (wordToDigit.ContainsKey(words[i]))
            {
                words[i] = wordToDigit[words[i]];
            }
        }

        // Збираєсо новий рядок
        string result = string.Join(" ", words);

        Console.WriteLine("\n Новий рядок:");
        Console.WriteLine(result);

        // Запит на продовження
        Console.WriteLine("\nБажаєте продовжити? (Y/N):");
        choice = char.ToLower(Console.ReadKey().KeyChar);
        Console.WriteLine();

        } while (choice == 'y');
    }
}