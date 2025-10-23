using System;
using System.Collections.Generic;
using System.Security.Authentication;

class Program
{
    static void Add(List<Product> products)
    {
        Console.WriteLine("\nВведите данные о товаре:");


        int maxId = 0;
        foreach (var product in products)
        {
            if (product.Id > maxId)
            {
                maxId = product.Id;
            }
        }
        int id = maxId + 1;

        Console.Write("\nНазвание: ");
        string name = Console.ReadLine();

        Console.Write("\nЦена: ");
        int price = Convert.ToInt32(Console.ReadLine());

        Console.Write("\nКоличество: ");
        int quantity = Convert.ToInt32(Console.ReadLine());

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
        if (products.Count == 0)
        {
            Console.WriteLine("\nСписок товаров пуст!");
            return;
        }

        Console.WriteLine("\nСписок товаров:");
        foreach (var product in products)
        {
            Console.WriteLine($"\nID: {product.Id}, Название: {product.Name}");
        }

        Console.Write("\nВведите ID товара для удаления: ");
        int idToRemove = Convert.ToInt32(Console.ReadLine());

        Product productToRemove = null;
        foreach (var product in products)
        {
            if (product.Id == idToRemove)
            {
                productToRemove = product;
                break;
            }
    }

        if (productToRemove != null)
        {
            products.Remove(productToRemove);
            Console.WriteLine($"\nТовар '{productToRemove.Name}' удален!");
        }
        else
        {
            Console.WriteLine("\nТовар с таким ID не найден!");
        }
    }

    static void ShowAllProducts(List<Product> products)
    {
        if (products.Count == 0)
        {
            Console.WriteLine("\nТоваров нет!");
            return;
        }

        Console.WriteLine("\n Все товары");
        foreach (var product in products)
        {
            Console.WriteLine($"\nID: {product.Id}, Название: {product.Name}, " +
                            $"\nЦена: {product.Price}, Количество: {product.Quantity}, " +
                            $"\nВ наличии: {product.IsAvailable}, Категория: {product.Category}");
        }
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