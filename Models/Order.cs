namespace DNAOrderAPI.Models
{
    public class Order
    {
        public int CustomerID { get; set; }
        public DateTime DeliveryDate { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }
    }
}
