using System.ComponentModel.DataAnnotations;

namespace WebApplicationIceCreamProject.Models
{
    public class CartItem
    {
        [Key]
        public string ItemId { get; set; }

        public string CartId { get; set; }

        public int Quantity { get; set; }

        public System.DateTime DateCreated { get; set; }

        public int ProductId { get; set; }

        public virtual IceCream Flavor { get; set; }

    }
}
