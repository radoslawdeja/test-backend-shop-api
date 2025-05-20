namespace Test.Shop.Application.DTO
{
    public class ShopDetailsUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int CategoryId { get; set; }
    }
}
