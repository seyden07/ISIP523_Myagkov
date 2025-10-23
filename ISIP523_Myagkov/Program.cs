        Console.Write("\nВ наличии (true/false): ");
        bool isAvailable = Convert.ToBoolean(Console.ReadLine());

        Console.WriteLine("\nВыберите категорию:");
        Console.WriteLine("1. Электроника");
        Console.WriteLine("2. Одежда");
        Console.WriteLine("3. Еда");
        Console.WriteLine("4. Книги");
        Console.WriteLine("5. Спорт");
        Console.Write("\nКатегория (номер): ");

        int categoryChoice = Convert.ToInt32(Console.ReadLine());
        ProductCategory category = (ProductCategory)(categoryChoice - 1);

        products.Add(new Product
        {
            Id = id,
            Name = name,
            Price = price,
            Quantity = quantity,
            IsAvailable = isAvailable,
            Category = category
        });

        Console.WriteLine("\nТовар добавлен!");
    }

    static void Remove(List<Product> products)
    {

    }

    static void ShowAllProducts(List<Product> products)
    {

    }

    static void Order(List<Product> products)
    {

    }

    static void Sell(List<Product> products)
    {

    }

    static void SearchByAnything(List<Product> products)
    {

    }



    static void Main(string[] args)
    {
        List<Product> products = new List<Product>();

        while (true)
        {
            Console.WriteLine("\n========== Меню ==========");
            Console.WriteLine("1. Добавить товар");
            Console.WriteLine("2. Удалить товар");
            Console.WriteLine("3. Показать все товары");
            Console.WriteLine("4. Заказать товар (увеличить количество)");
            Console.WriteLine("5. Продать товары");
            Console.WriteLine("6. Поиск товаров");
            Console.WriteLine("7. Выход");
            Console.Write("\nВыберите действие: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Add(products);
                    break;
                case "2":
                    Remove(products);
                    break;
                case "3":
                    ShowAllProducts(products);
                    break;
                case "4":
                    Order(products);
                    break;
                case "5":
                    Sell(products);
                    break;
                case "6":
                    SearchByAnything(products);
                    break;
                case "7":
                    return;
                default:
                    Console.WriteLine("\nНет такого варианта!");
                    break;
            }
        }
    }
}

public enum ProductCategory
{
    Электроника = 1,
    Одежда = 2,
    Еда = 3,
    Книги = 4,
    Спортивное = 5
}

class Product
{
    public int Id;
    public string Name;
    public int Price;
    public int Quantity;
    public bool IsAvailable;
    public ProductCategory Category;
};