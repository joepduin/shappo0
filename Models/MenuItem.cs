namespace shappo0.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public bool IsAlcoholic { get; set; }

        public MenuItem()
        {


        }

        public MenuItem(int id, string name, double price, bool isAlcoholic)
        {
            Id = id;
            Name = name;
            Price = price;
            IsAlcoholic = isAlcoholic;
        }
    }
}
