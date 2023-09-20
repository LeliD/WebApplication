namespace WebApplicationIceCreamProject.Models
{
    public enum DayOfWeek
    {
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday
    }
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public int HouseNumber { get; set; }
        public CartView Products { get; set; }
        public DateTime Date { get; set; }
        public double FeelsLike { get; set; }
        public double Humidity { get; set; }//לחות
        public bool IsHoliday { get; set; }
        public DayOfWeek Day { get; set; }
    }
}
