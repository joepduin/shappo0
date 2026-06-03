using shappo0.Models;

namespace shappo0.ViewModels
{
    public class KitchenBarViewModel
    {
        public List<Bar> Bar { get; set; }
        public List<Kitchen> Kitchen { get; set; }

        public KitchenBarViewModel(List<Bar> bar)
        {
            Bar = bar;
        }

        public KitchenBarViewModel(List<Kitchen> kitchen)
        {
            Kitchen = kitchen;
        }
    }
}
