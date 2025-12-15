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
}