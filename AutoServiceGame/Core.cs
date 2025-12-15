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