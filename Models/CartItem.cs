using System.ComponentModel.DataAnnotations;
namespace WebApplicationIceCreamProject.Models
{
    public class CartItem
    {

        [Key]
        public string ItemId { get; set; }

        public string CartId { get; set; }

        public double Size { get; set; }

        public int Quantity { get; set; }

        public double Price { get; set; }

        public System.DateTime DateCreated { get; set; }

        public int FlavourId { get; set; }

    }
}