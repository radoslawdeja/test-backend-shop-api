namespace Test.Shop.Application.DTO
{
    public class ShopDetailsAddDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int CategoryId { get; set; }
        public int CreatedById { get; set; }
    }
}
