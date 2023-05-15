using DNAOrderAPI.Models;
using Newtonsoft.Json;
using System;
using System.Text.Json;
using System.Xml.Serialization;

namespace DNAOrderAPI.Services
{
    public static class OrderService
    {
        public interface IOrderService
        {
            double DiscountedPrice(double amount);
            void SaveOrderToJSON(Order orderToAdd);
        }

        public static double BASE_PRICE = 99.98;
        public static double DiscountedPrice(double amount)
        {
            double totalPrice = amount * BASE_PRICE;
            if (amount >= 10 && amount < 50)
            {
                var discount = BASE_PRICE * 0.05;
                totalPrice -= (amount - 9) * discount;
            }
            else if (amount >= 50)
            {
                var discount = BASE_PRICE * 0.15;
                totalPrice -= (amount - 49) * discount;
            }
            return totalPrice;
        }
        public static void SaveOrderToJSON(Order orderToAdd)
        {
            if (File.Exists("orderList.json"))
            {
                string json = File.ReadAllText("orderList.json");
                var orders = JsonConvert.DeserializeObject<List<Order>>(json);
                orders.Add(orderToAdd);
                json = JsonConvert.SerializeObject(orders, Formatting.Indented);
                File.WriteAllText("orderList.json", json);
            }
            else
            {
                var orders = new List<Order> { orderToAdd };
                string jsonData = JsonConvert.SerializeObject(orders, Formatting.Indented);
                File.WriteAllText("orderList.json", jsonData);
            }


        }
    }
}
