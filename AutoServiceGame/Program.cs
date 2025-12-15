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

        static void ShowHeader(AutoService service)
        {
            var state = service.GetGameState();
            if (state != null)
            {
                Console.WriteLine($"День: {state.DayNumber} | Баланс: {state.Balance:C} | Прибыль: {state.Profit:C}");
                Console.WriteLine(new string('=', 50));
            }
        }

        static void HandleNewCustomer(AutoService service)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== НОВЫЙ КЛИЕНТ ===");

                var (customer, requestedPart, repairCost) = service.GenerateCustomer();

                Console.WriteLine($"\nКлиент: {customer.Name}");
                Console.WriteLine($"Автомобиль: {customer.CarModel}");
                Console.WriteLine($"Поломка: {requestedPart.Name}");
                Console.WriteLine($"Категория: {requestedPart.Category}");
                Console.WriteLine($"Стоимость ремонта: {repairCost:C}");
                Console.WriteLine($"\nЦена детали: {requestedPart.Price:C}");
                Console.WriteLine($"Ваша прибыль: {repairCost - requestedPart.Price:C}");

                Console.WriteLine("\nВаши действия:");
                Console.WriteLine("1. Принять заказ");
                Console.WriteLine("2. Отказать");
                Console.Write("Выберите: ");

                var choice = Console.ReadLine();

                if (choice == "1")
                {
                    var result = service.ProcessRepair(customer, requestedPart, repairCost);

                    Console.WriteLine($"\n{result.message}");

                    if (result.penalty > 0)
                    {
                        Console.WriteLine($"Списано штрафа: {result.penalty:C}");
                    }
                }
                else
                {
                    var penalty = requestedPart.Price * 0.1m;
                    var state = service.GetGameState();
                    if (state != null)
                    {
                        state.Balance -= penalty;
                    }
                    Console.WriteLine($"\nВы отказали клиенту. Штраф: {penalty:C}");
                }

                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка: {ex.Message}");
                Console.ReadKey();
            }
        }