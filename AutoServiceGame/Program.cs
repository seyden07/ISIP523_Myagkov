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
                var connectionString = @"Server=DESKTOP-CMIFD16\SEYDEH;Database=AutoServiceDB;Trusted_Connection=True;";
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

        static void ShowInventory(AutoService service)
        {
            Console.Clear();
            Console.WriteLine("=== СКЛАД ЗАПЧАСТЕЙ ===");

            var inventory = service.GetCurrentInventory();

            if (!inventory.Any())
            {
                Console.WriteLine("Склад пуст!");
            }
            else
            {
                Console.WriteLine($"\n{"Название",-25} {"Категория",-15} {"Цена",-10} {"На складе",-10} {"Доступно",-10}");
                Console.WriteLine(new string('-', 80));

                foreach (var item in inventory)
                {
                    Console.WriteLine($"{item.Part.Name,-25} {item.Part.Category,-15} {item.Part.Price,-10:C} " +
                                    $"{item.Quantity,-10} {item.AvailableQuantity,-10}");
                }
            }

            Console.WriteLine("\nНажмите любую клавишу для возврата...");
            Console.ReadKey();
        }

        static void HandlePurchase(AutoService service)
        {
            Console.Clear();
            Console.WriteLine("=== ЗАКАЗ ЗАПЧАСТЕЙ ===");

            var parts = service.GetAvailableParts();

            if (!parts.Any())
            {
                Console.WriteLine("Нет доступных деталей для заказа!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\n{"ID",-5} {"Название",-25} {"Категория",-15} {"Цена",-10}");
            Console.WriteLine(new string('-', 60));

            foreach (var part in parts)
            {
                Console.WriteLine($"{part.Id,-5} {part.Name,-25} {part.Category,-15} {part.Price,-10:C}");
            }

            Console.WriteLine("\n0. Отмена");
            Console.Write("\nВведите ID детали для заказа: ");

            if (int.TryParse(Console.ReadLine(), out int partId) && partId > 0)
            {
                var part = parts.FirstOrDefault(p => p.Id == partId);
                if (part == null)
                {
                    Console.WriteLine("Деталь не найдена!");
                    Console.ReadKey();
                    return;
                }

                Console.Write($"Введите количество {part.Name} для заказа: ");

                if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                {
                    var result = service.PurchaseParts(partId, quantity);
                    Console.WriteLine($"\n{result.message}");
                }
                else
                {
                    Console.WriteLine("\nНекорректное количество!");
                }
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        static void ShowStatistics(AutoService service)
        {
            Console.Clear();
            Console.WriteLine(service.GetStatistics());
            Console.WriteLine("\nНажмите любую клавишу для возврата...");
            Console.ReadKey();
        }
    }
}