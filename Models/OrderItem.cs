namespace shappo0.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public MenuItem MenuItem { get; set; }
        public int Count { get; set; }

        public OrderItem()
        {
        }

        public OrderItem(int id, MenuItem menuItem, int count)
        {
            Id = id;
            MenuItem = menuItem;
            Count = count;
        }
    }
}
