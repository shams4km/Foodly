namespace Foodly.Domain.Entities
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CoverUrl { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
