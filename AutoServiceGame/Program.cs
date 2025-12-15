using System;
using System.Linq;

namespace AutoServiceSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "Автосервис Симулятор";

            try
            {
                var connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=AutoServiceDB;Trusted_Connection=True;";
                var service = new AutoService(connectionString);

                RunGame(service);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка: {ex.Message}");
                Console.WriteLine("Убедитесь, что база данных создана и SQL Server запущен.");
                Console.WriteLine("Нажмите любую клавишу для выхода...");
                Console.ReadKey();
            }
        }
        static void RunGame(AutoService service)
        {
            Console.WriteLine("=== АВТОСЕРВИС СИМУЛЯТОР ===");
            Console.WriteLine("Управляйте автосервисом, ремонтируйте машины и зарабатывайте деньги!");
            Console.WriteLine();

            bool isRunning = true;

            while (isRunning)
            {
                Console.Clear();
                ShowHeader(service);

                Console.WriteLine("\n=== ГЛАВНОЕ МЕНЮ ===");
                Console.WriteLine("1. Принять нового клиента");
                Console.WriteLine("2. Просмотреть склад");
                Console.WriteLine("3. Заказать запчасти");
                Console.WriteLine("4. Показать статистику");
                Console.WriteLine("5. Перейти к следующему дню");
                Console.WriteLine("6. Выход");
                Console.Write("\nВыберите действие: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        HandleNewCustomer(service);
                        break;

                    case "2":
                        ShowInventory(service);
                        break;

                    case "3":
                        HandlePurchase(service);
                        break;

                    case "4":
                        ShowStatistics(service);
                        break;

                    case "5":
                        service.NextDay();
                        Console.WriteLine("\nНаступил новый день! Доставки обработаны.");
                        Console.ReadKey();
                        break;

                    case "6":
                        isRunning = false;
                        Console.WriteLine("\nСпасибо за игру! До свидания!");
                        break;

                    default:
                        Console.WriteLine("\nНеверный выбор. Нажмите любую клавишу...");
                        Console.ReadKey();
                        break;
                }
            }
        }