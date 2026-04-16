using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Базовий абстрактний клас для товарів з основними властивостями та методами
abstract class Product
{
    public string Name { get; set; }
    public decimal Price { get; set; }

    public Product() { }

    public Product(string name, decimal price)
    {
        Name = name;
        Price = price;
    }

    // Повертає межі цін: (low, mid)
    protected abstract Tuple<decimal, decimal> GetPriceLimits();

    // Визначає колір для відображення ціни (ANSI-коди) залежно від порогів
    public string GetPriceColor()
    {
        var l = GetPriceLimits();
        decimal low = l.Item1;
        decimal mid = l.Item2;

        if (Price < low * 0.5m) return "\x1b[36m";
        if (Price < low) return "\x1b[32m";
        if (Price <= mid) return "\x1b[33m";
        if (Price <= mid * 1.5m) return "\x1b[35m";
        return "\x1b[31m";
    }

    // Категорія товару (можна перевизначити у підкласах)
    public virtual string Category { get { return "Інше"; } }

    // 🔹 КЛЮЧОВЕ: динамічні характеристики
    // Повертає словник специфікацій (ключ -> значення). Нащадки доповнюють його.
    public virtual Dictionary<string, string> GetSpecs()
    {
        return new Dictionary<string, string>();
    }
}

// Електроніка: базовий продукт для електронних пристроїв
class Electronics : Product
{
    // Виробник
    public string Brand { get; set; }

    public Electronics() { }
    public Electronics(string n, decimal p) : base(n, p) { }

    protected override Tuple<decimal, decimal> GetPriceLimits()
        => Tuple.Create(300m, 1000m);

    public override string Category => "Електроніка";

    // Базові специфікації для електроніки
    public override Dictionary<string, string> GetSpecs()
    {
        return new Dictionary<string, string>()
        {
            { "Brand", string.IsNullOrEmpty(Brand) ? "-" : Brand }
        };
    }
}

// Смартфон: додає ОС до специфікацій
class Smartphone : Electronics
{
    public string OS { get; set; }

    public Smartphone() { }
    public Smartphone(string n, decimal p) : base(n, p) { }

    protected override Tuple<decimal, decimal> GetPriceLimits()
        => Tuple.Create(300m, 1200m);

    public override string Category => "Смартфони";

    // Додаємо поле ОС до специфікацій
    public override Dictionary<string, string> GetSpecs()
    {
        var d = base.GetSpecs();
        d["OS"] = OS ?? "-";
        return d;
    }
}

// Ноутбук: додає RAM до специфікацій
class Laptop : Electronics
{
    public int RAM { get; set; }

    public Laptop() { }
    public Laptop(string n, decimal p) : base(n, p) { }

    protected override Tuple<decimal, decimal> GetPriceLimits()
        => Tuple.Create(800m, 2000m);

    public override string Category => "Ноутбуки";

    // Додаємо RAM як специфікацію
    public override Dictionary<string, string> GetSpecs()
    {
        var d = base.GetSpecs();
        d["RAM"] = RAM.ToString();
        return d;
    }
}

// Ігровий ноутбук: додає GPU
class GamingLaptop : Laptop
{
    public string GPU { get; set; }

    public GamingLaptop() { }
    public GamingLaptop(string n, decimal p) : base(n, p) { }

    protected override Tuple<decimal, decimal> GetPriceLimits()
        => Tuple.Create(1500m, 3000m);

    public override string Category => "Ігрові ноутбуки";

    // Додаємо GPU до специфікацій
    public override Dictionary<string, string> GetSpecs()
    {
        var d = base.GetSpecs();
        d["GPU"] = GPU ?? "-";
        return d;
    }
}

// Фотоапарат: додає мегапікселі
class Camera : Electronics
{
    public int MegaPixels { get; set; }

    public Camera() { }
    public Camera(string n, decimal p) : base(n, p) { }

    protected override Tuple<decimal, decimal> GetPriceLimits()
        => Tuple.Create(500m, 3000m);

    public override string Category => "Фотоапарати";

    // Додаємо MP (мегапікселі) до специфікацій
    public override Dictionary<string, string> GetSpecs()
    {
        var d = base.GetSpecs();
        d["MP"] = MegaPixels.ToString();
        return d;
    }
}

// Планшет: додає розмір екрану
class Tablet : Electronics
{
    public int ScreenSize { get; set; }

    public Tablet() { }
    public Tablet(string n, decimal p) : base(n, p) { }

    protected override Tuple<decimal, decimal> GetPriceLimits()
        => Tuple.Create(300m, 1500m);

    public override string Category => "Планшети";

    // Додаємо розмір екрану до специфікацій
    public override Dictionary<string, string> GetSpecs()
    {
        var d = base.GetSpecs();
        d["Screen"] = ScreenSize.ToString();
        return d;
    }
}

// Навушники: вказує, чи безпровідні
class Headphones : Electronics
{
    public bool Wireless { get; set; }

    public Headphones() { }
    public Headphones(string n, decimal p) : base(n, p) { }

    protected override Tuple<decimal, decimal> GetPriceLimits()
        => Tuple.Create(50m, 300m);

    public override string Category => "Навушники";

    // Додаємо інформацію про безпровідність
    public override Dictionary<string, string> GetSpecs()
    {
        var d = base.GetSpecs();
        d["Wireless"] = Wireless ? "Yes" : "No";
        return d;
    }
}

// Аксесуар: простий товар без додаткових специфікацій
class Accessory : Product
{
    public Accessory() { }
    public Accessory(string n, decimal p) : base(n, p) { }

    protected override Tuple<decimal, decimal> GetPriceLimits()
        => Tuple.Create(20m, 100m);

    public override string Category => "Аксесуари";
}

class Program
{
    static readonly Dictionary<string, Func<Product>> termBank =
        new Dictionary<string, Func<Product>>()
    {
        { "смартфон", () => new Smartphone() },
        { "ноутбук", () => new Laptop() },
        { "ігровий ноутбук", () => new GamingLaptop() },
        { "фотоапарат", () => new Camera() },
        { "планшет", () => new Tablet() },
        { "навушники", () => new Headphones() },
        { "аксесуар", () => new Accessory() }
    };

    // Точка входу програми: меню для автогенерації або ручного введення
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        char choice = 'n';

        do
        {
            Console.Clear();

            Console.WriteLine("1 - Автогенерація");
            Console.WriteLine("2 - Ручне введення");
            Console.Write("Ваш вибір: ");

            string option = Console.ReadLine();

            List<Product> products;

            if (option == "1")
                products = GenerateProducts();
            else if (option == "2")
                products = ManualInput();
            else
            {
                ShowError("Невірний вибір!");
                continue;
            }

            PrintTable(products);

            Console.Write("\nПовторити? (Y/N): ");
            choice = Console.ReadKey().KeyChar;

        } while (choice == 'y' || choice == 'Y');
    }

    // Приклад автогенерації товарів для демонстрації програми
    static List<Product> GenerateProducts()
    {
        return new List<Product>()
        {
            new Smartphone("Samsung",1200){ OS="Android", Brand="Samsung"},
            new Laptop("Dell",1800){ RAM=16, Brand="Dell"},
            new GamingLaptop("Asus",2500){ RAM=32, GPU="RTX", Brand="Asus"},
            new Camera("Canon",1800){ MegaPixels=24, Brand="Canon"}
        };
    }

    // Динамічна таблиця: будує та виводить таблицю з динамічними стовпцями специфікацій
    static void PrintTable(List<Product> products)
    {
        int margin = 4;
        string pad = new string(' ', margin);

        // Збираємо всі унікальні ключі специфікацій серед усіх продуктів
        var allSpecs = products.SelectMany(p => p.GetSpecs().Keys).Distinct().ToList();


        // Обчислюємо ширину колонок для вирівнювання
        int nameW = products.Max(p => p.Name.Length) + 2;
        int catW = products.Max(p => p.Category.Length) + 2;

        var specW = new Dictionary<string, int>();
        foreach (var s in allSpecs)
        {
            specW[s] = Math.Max(s.Length,
                products.Max(p => p.GetSpecs().ContainsKey(s)
                    ? p.GetSpecs()[s].Length : 1)) + 2;
        }

        // ===== TOP BORDER =====
        Console.Write(pad + "┌────┬" + new string('─', nameW) + "┬" + new string('─', catW));

        foreach (var s in allSpecs)
            Console.Write("┬" + new string('─', specW[s]));

        Console.WriteLine("┬──────────┐");

        // ===== HEADER =====
        Console.Write(pad +
            "│ №  │ " + "Назва".PadRight(nameW - 1) +
            "│ " + "Категорія".PadRight(catW - 1));

        foreach (var s in allSpecs)
            Console.Write("│ " + s.PadRight(specW[s] - 1));

        Console.WriteLine("│ Ціна     │");

        // ===== SEPARATOR =====
        Console.Write(pad + "├────┼" + new string('─', nameW) + "┼" + new string('─', catW));

        foreach (var s in allSpecs)
            Console.Write("┼" + new string('─', specW[s]));

        Console.WriteLine("┼──────────┤");

        // ===== ROWS =====: виводимо кожен продукт та його специфікації
        int i = 1;

        foreach (var p in products)
        {
            var d = p.GetSpecs();

            Console.Write(pad +
                "│ " + i.ToString().PadRight(2) +
                " │ " + p.Name.PadRight(nameW - 1) +
                "│ " + p.Category.PadRight(catW - 1));

            foreach (var s in allSpecs)
            {
                string val = d.ContainsKey(s) ? d[s] : "-";
                Console.Write("│ " + val.PadRight(specW[s] - 1));
            }

            Console.Write("│ " + p.GetPriceColor() +
                p.Price.ToString().PadRight(8) + "\x1b[0m │");

            Console.WriteLine();
            i++;
        }

        // ===== BOTTOM BORDER =====
        Console.WriteLine(pad + "└────┴" + new string('─', nameW) +
                          "┴" + new string('─', catW) +
                          string.Concat(allSpecs.Select(s => "┴" + new string('─', specW[s]))) +
                          "┴──────────┘");
    }

    // Місце для реалізації ручного введення товарів. Поки що повертає порожній список.
    static List<Product> ManualInput() => new List<Product>();

    // Відображає повідомлення про помилку червоним кольором
    static void ShowError(string msg)
    {
        Console.WriteLine("\x1b[31m[ПОМИЛКА]: " + msg + "\x1b[0m");
    }
}