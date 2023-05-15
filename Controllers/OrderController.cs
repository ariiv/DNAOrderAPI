using Microsoft.AspNetCore.Mvc;
using DNAOrderAPI.Models;
using DNAOrderAPI.Services;
using System.Text.Json;

namespace DNAOrderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly List<Order> _orders = new List<Order>();

        public OrdersController()
        {
            _orders = new List<Order>();
        }

        [HttpPost("PlaceOrder")]
        public async Task<ActionResult<Order>> PlaceOrder([FromQuery] int customerID, [FromQuery] DateTime deliveryDate, [FromQuery] int amount)
        {
            if (deliveryDate.Date <= DateTime.Now.Date)
            {
                return BadRequest("Order rejected. Reason: Delivery date must be in the future.");
            }

            if (amount <= 0 || amount > 999 || amount % 1 != 0)
            {
                return BadRequest("Order rejected. Reason: Amount of ordered kits must be round, positive number.");
            }

            var totalPrice = OrderService.DiscountedPrice(amount);

            Order placedOrder = new Order
            {
                CustomerID = customerID,
                DeliveryDate = deliveryDate,
                Amount = amount,
                Price = totalPrice
            };

            _orders.Add(placedOrder);
            OrderService.SaveOrderToJSON(placedOrder);

            return Ok("Order placed successfully. Your total price is: " + totalPrice + ".");
        }

        [HttpGet("GetOrders")]
        public async Task<IActionResult> GetOrders([FromQuery] int customerID)
        {
            if (System.IO.File.Exists("orderList.json"))
            {
                string json = System.IO.File.ReadAllText("orderList.json");
                var orders = JsonSerializer.Deserialize<List<Order>>(json);

                if (orders != null)
                {
                    var customerOrders = orders.Where(o => o.CustomerID == customerID).ToList();

                    if (customerOrders.Count > 0)
                    {
                        return Ok("List of orders: " + JsonSerializer.Serialize(customerOrders));
                    }
                }
            }
            return NotFound("No orders found. Make sure that existing CustomerID is typed in.");
        }

        [HttpGet("GetAllOrders")]
        public IActionResult GetAllOrders()
        {
            if (System.IO.File.Exists("orderList.json"))
            {
                string json = System.IO.File.ReadAllText("orderList.json");
                var orders = JsonSerializer.Deserialize<List<Order>>(json);

                if (orders != null && orders.Count > 0)
                {
                   return Ok("List of orders: " + JsonSerializer.Serialize(orders));
                }
            }
            return NotFound("No orders found.");
        }

        [HttpDelete("DeleteOrders")]
        public IActionResult DeleteOrderList()
        {
            try
            {
                if (System.IO.File.Exists("orderList.json"))
                {
                    System.IO.File.Delete("orderList.json");
                }

                return Ok("Orders deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to delete orders: {ex.Message}");
            }
        }
    }
}
