using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

class JaggedArray_V3
{
    // Імпорт функцій WinAPI для зміни кодування консолі
    [DllImport("kernel32.dll")]
    static extern bool SetConsoleCP(uint wCodePageID); // Встановлення кодування вводу

    [DllImport("kernel32.dll")]
    static extern bool SetConsoleOutputCP(uint wCodePageID); // Встановлення кодування виводу


    // Функція для обчислення суми елементів одного рядка рваного масиву
    static int RowSum(int[] row)
    {
        int sum = 0;
        for (int i = 0; i < row.Length; i++) // Цикл по всіх елементах рядка
            sum += row[i]; // Додаємо поточний елемент до суми
        return sum;
    }

    // Функція безпечного вводу цілих чисел із перевіркою
    static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
    {
        int value;
        while (true) // Цикл повторного вводу
        {
            Console.Write(prompt);
            string input = Console.ReadLine();

            // Перевірка на порожній ввід
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Помилка: ввід не може бути порожнім.");
                continue; // Повертаємось на початок циклу
            }

            // Перевірка, що всі символи числа допустимі (цифри та знак мінус на початку)
            bool validChars = true;
            for (int i = 0; i < input.Length; i++)
            {
                if (i == 0 && input[i] == '-') continue; // Мінус на початку дозволений
                if (!char.IsDigit(input[i]))
                {
                    validChars = false; // Недопустимий символ
                    break;
                }
            }

            if (!validChars)
            {
                Console.WriteLine("Помилка: введено не число. Використовуйте тільки арабські цифри.");
                continue;
            }

            // Спроба перетворення рядка у число
            if (!int.TryParse(input, out value))
            {
                Console.WriteLine("Помилка: число занадто велике або мале.");
                continue;
            }

            // Перевірка на межі допустимого значення
            if (value < min || value > max)
            {
                Console.WriteLine($"Помилка: число має бути в межах [{min}, {max}].");
                continue;
            }

            return value; // Успішний ввід, повертаємо число
        }
    }


    static void Main()
    {
        // Встановлюємо кодування консолі для кирилиці
        SetConsoleCP(1251);
        SetConsoleOutputCP(1251);

        string choice;

        do
        {
            Console.Clear();

            // ----- Вибір режиму заповнення рваного масиву -----
            Console.WriteLine("Оберіть режим:");
            Console.WriteLine("1 - Ввід вручну");
            Console.WriteLine("2 - Випадкове заповнення");
            int mode = ReadInt("Ваш вибір: ", 1, 2); // Вибір режиму

            int n = ReadInt("Введіть кількість рядків: ", 1); // Кількість рядків рваного масиву

            // ----- Створення рваного масиву -----
            int[][] arr = new int[n][];

            if (mode == 1) // Ручне заповнення
            {
                for (int i = 0; i < n; i++) // Проходимо по кожному рядку
                {
                    int m = ReadInt($"Введіть кількість елементів у рядку {i}: ", 1); // Довжина рядка
                    arr[i] = new int[m]; // Ініціалізація рядка

                    Console.WriteLine($"Введіть елементи рядка {i}:");
                    for (int j = 0; j < m; j++) // Цикл по елементах рядка
                    {
                        arr[i][j] = ReadInt($"arr[{i}][{j}] = "); // Заповнення рядка
                    }
                }
            }
            else // Випадкове заповнення
            {
                Random rnd = new Random();

                int minLen = ReadInt("Мінімальна довжина рядка: ", 1);
                int maxLen = ReadInt("Максимальна довжина рядка: ", minLen);

                int minVal = ReadInt("Мінімальне значення: ");
                int maxVal = ReadInt("Максимальне значення: ", minVal);

                for (int i = 0; i < n; i++) // Проходимо по кожному рядку
                {
                    int m = rnd.Next(minLen, maxLen + 1); // Випадкова довжина рядка
                    arr[i] = new int[m]; // Ініціалізація рядка

                    for (int j = 0; j < m; j++) // Заповнення рядка випадковими числами
                    {
                        arr[i][j] = rnd.Next(minVal, maxVal + 1);
                    }
                }
            }

            // ===== ОСНОВНА ЛОГІКА ОБРОБКИ =====

            int[] sums = new int[arr.Length]; // Масив для сум рядків
            int minSum = 0; // Мінімальна сума рядка

            // Обчислення сум рядків і пошук мінімальної
            for (int i = 0; i < arr.Length; i++)
            {
                sums[i] = RowSum(arr[i]); // Виклик функції RowSum
                if (i == 0 || sums[i] < minSum)
                    minSum = sums[i]; // Збереження мінімальної суми
            }

            // Підрахунок кількості рядків з мінімальною сумою
            int countMin = 0;
            for (int i = 0; i < sums.Length; i++)
                if (sums[i] == minSum)
                    countMin++;

            // Створення нового рваного масиву з нульовими рядками після мінімальних
            int[][] result = new int[arr.Length + countMin][];
            int index = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                result[index++] = arr[i]; // Копіюємо рядок

                if (sums[i] == minSum) // Якщо рядок має мінімальну суму
                {
                    result[index] = new int[arr[i].Length]; // Створюємо нульовий рядок
                    for (int j = 0; j < result[index].Length; j++)
                        result[index][j] = 0; // Заповнюємо нулями
                    index++;
                }
            }

            // ===== Вивід рваного масиву =====
            Console.WriteLine("\nПочатковий масив:");
            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = 0; j < arr[i].Length; j++)
                    Console.Write(arr[i][j] + " "); // Вивід елемента
                Console.WriteLine(); // Перехід на новий рядок
            }

            Console.WriteLine("\nРезультат:");
            for (int i = 0; i < result.Length; i++)
            {
                for (int j = 0; j < result[i].Length; j++)
                    Console.Write(result[i][j] + " "); // Вивід елемента результатуючого масиву
                Console.WriteLine();
            }

            // Вивід сум рядків результатуючого масиву
            Console.WriteLine("\nСуми рядків результатуючого масиву:");
            for (int i = 0; i < result.Length; i++)
            {
                int sum = RowSum(result[i]); // Обчислення суми рядка
                Console.WriteLine($"Рядок {i}: сума = {sum}");
            }

            // ===== ПОВТОР ПРОГРАМИ =====
            Console.Write("\nБажаєте повторити? (y/n): ");
            choice = Console.ReadLine(); // Введення рішення користувача

        } while (choice == "y" || choice == "Y"); // Повтор програми, якщо користувач ввів y або Y
    }
}