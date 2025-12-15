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