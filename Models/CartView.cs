namespace WebApplicationIceCreamProject.Models
{

    public class CartView
    {
        public List<CartItem> CartItems { get; set; }
        public List<IceCream> Flavours { get; set; }
    }
}
