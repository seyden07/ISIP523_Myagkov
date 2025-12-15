using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AutoServiceSimulation
{
    public enum OrderStatus { Pending, InProgress, Completed, Failed, Cancelled }
    public enum PurchaseStatus { Pending, Delivered, Cancelled }

    public class Part
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public bool IsCritical { get; set; }

        public Part(int id, string name, decimal price, string category, bool isCritical)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Название детали не может быть пустым");
            if (price <= 0) throw new ArgumentException("Цена должна быть положительной");

            Id = id;
            Name = name;
            Price = price;
            Category = category;
            IsCritical = isCritical;
        }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CarModel { get; set; }
        public int SatisfactionLevel { get; set; }

        public Customer(int id, string name, string carModel, int satisfactionLevel = 100)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Имя клиента не может быть пустым");

            Id = id;
            Name = name;
            CarModel = carModel;
            SatisfactionLevel = satisfactionLevel;
        }
    }
    public class InventoryItem
    {
        public int Id { get; set; }
        public Part Part { get; set; }
        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int AvailableQuantity => Quantity - ReservedQuantity;
    }

    public class RepairOrder
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public Part RequestedPart { get; set; }
        public Part UsedPart { get; set; }
        public decimal RepairCost { get; set; }
        public decimal PenaltyPaid { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
    }

    public class PurchaseOrder
    {
        public int Id { get; set; }
        public Part Part { get; set; }
        public int Quantity { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime OrderDate { get; set; }
        public int DeliveryDay { get; set; }
        public PurchaseStatus Status { get; set; }
    }

    public class GameState
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
        public int DayNumber { get; set; }
        public int TotalCustomers { get; set; }
        public int SuccessfulRepairs { get; set; }
        public int FailedRepairs { get; set; }
        public int Refusals { get; set; }
        public decimal Profit => Balance - 10000;
    }
    public interface IPartRepository
    {
        List<Part> GetAllParts();
        List<InventoryItem> GetInventory();
        bool UpdateInventory(int partId, int quantity);
        bool ReservePart(int partId, int quantity);
        bool UsePart(int partId, int quantity);
        int GetAvailableQuantity(int partId);
    }

    public interface IOrderRepository
    {
        int CreateRepairOrder(Customer customer, Part requestedPart, decimal repairCost);
        List<PurchaseOrder> GetPendingPurchaseOrders();
        int CreatePurchaseOrder(Part part, int quantity, decimal totalCost, int deliveryDay);
        bool DeliverPurchaseOrder(int orderId);
    }

    public interface IGameStateRepository
    {
        GameState GetCurrentGameState();
        bool UpdateBalance(decimal newBalance);
        bool UpdateDay(int newDay);
        void UpdateStatistics(int successful, int failed, int refusals);
        void AddCustomer();
    }

    public class DatabaseRepository : IPartRepository, IOrderRepository, IGameStateRepository
    {
        private readonly string _connectionString;

        public DatabaseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // IPartRepository implementation
        public List<Part> GetAllParts()
        {
            var parts = new List<Part>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Parts ORDER BY Name", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        parts.Add(new Part(
                            id: (int)reader["Id"],
                            name: (string)reader["Name"],
                            price: (decimal)reader["Price"],
                            category: (string)reader["Category"],
                            isCritical: (bool)reader["IsCritical"]
                        ));
                    }
                }
            }
            return parts;
        }

        public List<InventoryItem> GetInventory()
        {
            var inventory = new List<InventoryItem>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = @"
                    SELECT i.Id, i.Quantity, i.ReservedQuantity, 
                           p.Id as PartId, p.Name, p.Price, p.Category, p.IsCritical
                    FROM Inventory i
                    INNER JOIN Parts p ON i.PartId = p.Id
                    WHERE i.Quantity > 0";

                var command = new SqlCommand(sql, connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var part = new Part(
                            id: (int)reader["PartId"],
                            name: (string)reader["Name"],
                            price: (decimal)reader["Price"],
                            category: (string)reader["Category"],
                            isCritical: (bool)reader["IsCritical"]
                        );

                        inventory.Add(new InventoryItem
                        {
                            Id = (int)reader["Id"],
                            Part = part,
                            Quantity = (int)reader["Quantity"],
                            ReservedQuantity = (int)reader["ReservedQuantity"]
                        });
                    }
                }
            }
            return inventory;
        }

        public bool UpdateInventory(int partId, int quantity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = @"UPDATE Inventory SET Quantity = Quantity + @Quantity WHERE PartId = @PartId";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Quantity", quantity);
                command.Parameters.AddWithValue("@PartId", partId);
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool ReservePart(int partId, int quantity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = @"
                    UPDATE Inventory 
                    SET ReservedQuantity = ReservedQuantity + @Quantity
                    WHERE PartId = @PartId 
                    AND (Quantity - ReservedQuantity) >= @Quantity";

                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Quantity", quantity);
                command.Parameters.AddWithValue("@PartId", partId);
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool UsePart(int partId, int quantity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = @"
                    UPDATE Inventory 
                    SET Quantity = Quantity - @Quantity,
                        ReservedQuantity = ReservedQuantity - @Quantity
                    WHERE PartId = @PartId 
                    AND ReservedQuantity >= @Quantity";

                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Quantity", quantity);
                command.Parameters.AddWithValue("@PartId", partId);
                return command.ExecuteNonQuery() > 0;
            }
}

        public int GetAvailableQuantity(int partId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = "SELECT Quantity - ReservedQuantity FROM Inventory WHERE PartId = @PartId";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@PartId", partId);
                var result = command.ExecuteScalar();
                return result != DBNull.Value ? (int)result : 0;
            }
        }

        public int CreateRepairOrder(Customer customer, Part requestedPart, decimal repairCost)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var customerSql = @"
                    INSERT INTO Customers (Name, CarModel, SatisfactionLevel) 
                    VALUES (@Name, @CarModel, @SatisfactionLevel);
                    SELECT CAST(SCOPE_IDENTITY() as int)";

                var customerCommand = new SqlCommand(customerSql, connection);
                customerCommand.Parameters.AddWithValue("@Name", customer.Name);
                customerCommand.Parameters.AddWithValue("@CarModel", customer.CarModel);
                customerCommand.Parameters.AddWithValue("@SatisfactionLevel", customer.SatisfactionLevel);
                var customerId = (int)customerCommand.ExecuteScalar();

                var orderSql = @"
                    INSERT INTO RepairOrders (CustomerId, PartId, RequestedPartId, RepairCost, Status) 
                    VALUES (@CustomerId, @PartId, @RequestedPartId, @RepairCost, 'Pending');
                    SELECT CAST(SCOPE_IDENTITY() as int)";

                var orderCommand = new SqlCommand(orderSql, connection);
                orderCommand.Parameters.AddWithValue("@CustomerId", customerId);
                orderCommand.Parameters.AddWithValue("@PartId", requestedPart.Id);
                orderCommand.Parameters.AddWithValue("@RequestedPartId", requestedPart.Id);
                orderCommand.Parameters.AddWithValue("@RepairCost", repairCost);

                return (int)orderCommand.ExecuteScalar();
            }
        }

        public List<PurchaseOrder> GetPendingPurchaseOrders()
        {
            var orders = new List<PurchaseOrder>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = @"
                    SELECT po.*, p.Id as PartId, p.Name, p.Price, p.Category, p.IsCritical
                    FROM PurchaseOrders po
                    INNER JOIN Parts p ON po.PartId = p.Id
                    WHERE po.Status = 'Pending'";

                var command = new SqlCommand(sql, connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var part = new Part(
                            id: (int)reader["PartId"],
                            name: (string)reader["Name"],
                            price: (decimal)reader["Price"],
                            category: (string)reader["Category"],
                            isCritical: (bool)reader["IsCritical"]
                        );

                        orders.Add(new PurchaseOrder
                        {
                            Id = (int)reader["Id"],
                            Part = part,
                            Quantity = (int)reader["Quantity"],
                            TotalCost = (decimal)reader["TotalCost"],
                            OrderDate = (DateTime)reader["OrderDate"],
                            DeliveryDay = (int)reader["DeliveryDay"],
                            Status = (PurchaseStatus)Enum.Parse(typeof(PurchaseStatus), (string)reader["Status"])
                        });
                    }
                }
            }
            return orders;
        }

        public int CreatePurchaseOrder(Part part, int quantity, decimal totalCost, int deliveryDay)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = @"
                    INSERT INTO PurchaseOrders (PartId, Quantity, TotalCost, DeliveryDay, Status) 
                    VALUES (@PartId, @Quantity, @TotalCost, @DeliveryDay, 'Pending');
                    SELECT CAST(SCOPE_IDENTITY() as int)";

                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@PartId", part.Id);
                command.Parameters.AddWithValue("@Quantity", quantity);
                command.Parameters.AddWithValue("@TotalCost", totalCost);
                command.Parameters.AddWithValue("@DeliveryDay", deliveryDay);

                return (int)command.ExecuteScalar();
            }
        }

        public bool DeliverPurchaseOrder(int orderId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = "UPDATE PurchaseOrders SET Status = 'Delivered' WHERE Id = @Id";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", orderId);
                return command.ExecuteNonQuery() > 0;
            }
        }

        public GameState GetCurrentGameState()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = "SELECT TOP 1 * FROM GameStates ORDER BY Id DESC";
                var command = new SqlCommand(sql, connection);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new GameState
                        {
                            Id = (int)reader["Id"],
                            Balance = (decimal)reader["Balance"],
                            DayNumber = (int)reader["DayNumber"],
                            TotalCustomers = (int)reader["TotalCustomers"],
                            SuccessfulRepairs = (int)reader["SuccessfulRepairs"],
                            FailedRepairs = (int)reader["FailedRepairs"],
                            Refusals = (int)reader["Refusals"]
                        };
                    }
                }
            }
            return null;
        }

        public bool UpdateBalance(decimal newBalance)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = "UPDATE GameStates SET Balance = @Balance WHERE Id = (SELECT TOP 1 Id FROM GameStates ORDER BY Id DESC)";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Balance", newBalance);
                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool UpdateDay(int newDay)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = "UPDATE GameStates SET DayNumber = @DayNumber WHERE Id = (SELECT TOP 1 Id FROM GameStates ORDER BY Id DESC)";
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@DayNumber", newDay);
                return command.ExecuteNonQuery() > 0;
            }
        }

        public void UpdateStatistics(int successful, int failed, int refusals)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = @"
                    UPDATE GameStates 
                    SET SuccessfulRepairs = SuccessfulRepairs + @Successful,
                        FailedRepairs = FailedRepairs + @Failed,
                        Refusals = Refusals + @Refusals
                    WHERE Id = (SELECT TOP 1 Id FROM GameStates ORDER BY Id DESC)";

                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Successful", successful);
                command.Parameters.AddWithValue("@Failed", failed);
                command.Parameters.AddWithValue("@Refusals", refusals);
                command.ExecuteNonQuery();
            }
        }

        public void AddCustomer()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = "UPDATE GameStates SET TotalCustomers = TotalCustomers + 1 WHERE Id = (SELECT TOP 1 Id FROM GameStates ORDER BY Id DESC)";
                var command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
            }
        }
    }

    public class AutoService
    {
        private readonly DatabaseRepository _repository;
        private readonly Random _random = new Random();

        private const decimal WORK_COST_MULTIPLIER = 1.5m;
        private const decimal PENALTY_MULTIPLIER = 2.0m;
        private const decimal REFUSAL_PENALTY_MULTIPLIER = 0.1m;
        private const int DELIVERY_DELAY = 2;

        private readonly string[] _customerNames = { "Иван Петров", "Анна Смирнова", "Сергей Иванов", "Ольга Кузнецова" };
        private readonly string[] _carModels = { "Toyota Camry", "Lada Vesta", "Kia Rio", "Hyundai Solaris" };

        public AutoService(string connectionString)
        {
            _repository = new DatabaseRepository(connectionString);
        }

        public (Customer customer, Part requestedPart, decimal repairCost) GenerateCustomer()
        {
            var parts = _repository.GetAllParts();
            if (!parts.Any()) throw new InvalidOperationException("Нет доступных деталей");

            var requestedPart = parts[_random.Next(parts.Count)];
            var repairCost = requestedPart.Price * WORK_COST_MULTIPLIER;

            var customer = new Customer(
                id: 0, // Будет присвоен при сохранении
                name: _customerNames[_random.Next(_customerNames.Length)],
                carModel: _carModels[_random.Next(_carModels.Length)]
            );

            _repository.AddCustomer();

            return (customer, requestedPart, repairCost);
        }

        public (bool success, string message, decimal penalty) ProcessRepair(Customer customer, Part requestedPart, decimal repairCost)
        {
            var gameState = _repository.GetCurrentGameState();
            if (gameState == null) return (false, "Ошибка: состояние игры не найдено", 0);

            var availableQuantity = _repository.GetAvailableQuantity(requestedPart.Id);

            if (availableQuantity > 0)
            {
                if (_repository.ReservePart(requestedPart.Id, 1))
                {
                    var orderId = _repository.CreateRepairOrder(customer, requestedPart, repairCost);

                    if (_repository.UsePart(requestedPart.Id, 1))
                    {
                        gameState.Balance += repairCost;
                        _repository.UpdateBalance(gameState.Balance);
                        _repository.UpdateStatistics(1, 0, 0);

                        return (true, $"Ремонт выполнен успешно! Получено {repairCost:C}", 0);
                    }
                }
            }
            else
            {
                var inventory = _repository.GetInventory();
                var availableParts = inventory
                    .Where(i => i.AvailableQuantity > 0 && i.Part.Id != requestedPart.Id)
                    .ToList();

                if (availableParts.Any())
                {
                    var replacement = availableParts[_random.Next(availableParts.Count)];
                    var penalty = replacement.Part.Price * PENALTY_MULTIPLIER;

                    if (_repository.ReservePart(replacement.Part.Id, 1) &&
                        _repository.UsePart(replacement.Part.Id, 1))
                    {
                        gameState.Balance -= penalty;
                        _repository.UpdateBalance(gameState.Balance);
                        _repository.UpdateStatistics(0, 1, 0);

                        return (false,
                            $"Клиент недоволен! Пришлось поставить {replacement.Part.Name}. " +
                            $"Штраф: {penalty:C}",
                            penalty);
                    }
                }
                else
                {
                    var penalty = requestedPart.Price * REFUSAL_PENALTY_MULTIPLIER;
                    gameState.Balance -= penalty;
                    _repository.UpdateBalance(gameState.Balance);
                    _repository.UpdateStatistics(0, 0, 1);

                    return (false, $"Отказано в обслуживании. Штраф: {penalty:C}", penalty);
                }
            }

            return (false, "Ошибка при обработке заказа", 0);
        }

        public (bool success, string message) PurchaseParts(int partId, int quantity)
        {
            var parts = _repository.GetAllParts();
            var part = parts.FirstOrDefault(p => p.Id == partId);
            if (part == null) return (false, "Деталь не найдена");

            if (quantity <= 0) return (false, "Количество должно быть положительным");

            var totalCost = part.Price * quantity;
            var gameState = _repository.GetCurrentGameState();

            if (gameState.Balance < totalCost)
                return (false, $"Недостаточно средств. Нужно: {totalCost:C}, есть: {gameState.Balance:C}");

            gameState.Balance -= totalCost;
            _repository.UpdateBalance(gameState.Balance);

            var deliveryDay = gameState.DayNumber + DELIVERY_DELAY;
            _repository.CreatePurchaseOrder(part, quantity, totalCost, deliveryDay);

            return (true, $"Заказ создан. Доставка через {DELIVERY_DELAY} дня(ей). Стоимость: {totalCost:C}");
        }

        public void ProcessDeliveries()
        {
            var gameState = _repository.GetCurrentGameState();
            var pendingOrders = _repository.GetPendingPurchaseOrders();

            foreach (var order in pendingOrders.Where(o => o.DeliveryDay <= gameState.DayNumber))
            {
                _repository.UpdateInventory(order.Part.Id, order.Quantity);
                _repository.DeliverPurchaseOrder(order.Id);
            }
        }

        public void NextDay()
        {
            var gameState = _repository.GetCurrentGameState();
            _repository.UpdateDay(gameState.DayNumber + 1);
            ProcessDeliveries();
        }

        public string GetStatistics()
        {
            var gameState = _repository.GetCurrentGameState();
            if (gameState == null) return "Статистика недоступна";

            return $@"=== СТАТИСТИКА ===
День: {gameState.DayNumber}
Баланс: {gameState.Balance:C}
Всего клиентов: {gameState.TotalCustomers}
Успешных ремонтов: {gameState.SuccessfulRepairs}
Неудачных ремонтов: {gameState.FailedRepairs}
Отказов: {gameState.Refusals}
Прибыль: {gameState.Profit:C}
==================";
        }

        public List<Part> GetAvailableParts()
        {
            return _repository.GetAllParts();
        }

        public List<InventoryItem> GetCurrentInventory()
        {
            return _repository.GetInventory();
        }

        public GameState GetGameState()
        {
            return _repository.GetCurrentGameState();
        }
    }
}