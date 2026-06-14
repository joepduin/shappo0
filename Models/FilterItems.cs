namespace shappo0.Models
{
    public class FilterItems
    {
        public string? ProductType { get; set; }
        public string? MenuType { get; set; }
        public bool? OnMenu { get; set; }

        public FilterItems()
        {

        }

        public FilterItems(string? productType, string? menuType, bool? onMenu)
        {
            ProductType = productType;
            MenuType = menuType;
            OnMenu = onMenu;
        }
    }
}
