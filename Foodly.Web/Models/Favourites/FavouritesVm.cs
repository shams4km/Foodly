namespace Foodly.Web.Models.Favourites
{
    public class FavouritesVm
    {
        public List<FavouriteDishVm> Dishes { get; set; } = new();
        public List<FavouriteRestaurantVm> Restaurants { get; set; } = new();
    }

    public class FavouriteDishVm
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Image { get; set; } = "";     // путь к изображению
        public string Meta { get; set; } = "";      // «Pie • 15–25 min»
        public string Badge { get; set; } = "";     // «Free delivery»
    }

    public class FavouriteRestaurantVm
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Logo { get; set; } = "";      // путь к лого
        public string Meta { get; set; } = "";      // «Burger • 4.3 km»
        public string Badge { get; set; } = "";     // «Buy 2 get 1 free»
    }
}
