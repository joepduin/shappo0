namespace shappo0.ViewModels
{
    public class FilterItemsViewModel
    {
        public string? ProductType { get; set; }
        public string? MenuType { get; set; }
        public bool? OnMenu { get; set; }

        public FilterItemsViewModel()
        {

        }

        public FilterItemsViewModel(string? productType, string? menuType, bool? onMenu)
        {
            ProductType = productType;
            MenuType = menuType;
            OnMenu = onMenu;
        }
    }
}
