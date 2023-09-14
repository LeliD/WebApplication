namespace WebApplicationIceCreamProject.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public int HouseNumber { get; set; }
        public string IceCream { get; set; }
        public DateTime Date { get; set; }
        public double FeelsLike { get; set; }
        public double Humidity { get; set; }//לחות
        public double Pressure { get; set; }//לחץ אויר
    }
}
